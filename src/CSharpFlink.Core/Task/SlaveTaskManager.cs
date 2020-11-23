using CSharpFlink.Core.Cache;
using CSharpFlink.Core.Calculate;
using CSharpFlink.Core.Channel;
using CSharpFlink.Core.Common;
using CSharpFlink.Core.Config;
using CSharpFlink.Core.Log;
using CSharpFlink.Core.Node;
using CSharpFlink.Core.Protocol;
using CSharpFlink.Core.Worker;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

namespace CSharpFlink.Core.Task
{
    public class SlaveTaskManager : ISlaveTaskManager
    {
        private bool _disposed = false;
        private ConcurrentDictionary<string, DownTransmission> _slaveCacheList;

        private SlaveCache _slaveCache;

        private Thread _excuteCalcThread;

        private bool _excuteCalcExit = false;

        private IWorker _defaultWorker;

        private IChannelMessageHandler _channelMessageHandler;

        public SlaveTaskManager(IChannelMessageHandler channelMessageHandler)
        {
            _channelMessageHandler = channelMessageHandler;

            _defaultWorker = Worker.Worker.GetDefaultWorker();
            _slaveCacheList = new ConcurrentDictionary<string, DownTransmission>();
            _slaveCache = new SlaveCache();

            LoadCache();

            _excuteCalcThread = new Thread(new ThreadStart(ExcuteCalcThread));
            _excuteCalcThread.Name = "ExcuteCalcThread";
            _excuteCalcThread.IsBackground = true;
            _excuteCalcThread.Start();
        }

        ~SlaveTaskManager()
        {
            Dispose(false);
        }

        private void ExcuteCalcThread()
        {
            while (true)
            {
                if (_excuteCalcExit)
                {
                    break;
                }

                try
                {
                    if (_slaveCacheList.Count > 0)
                    {
                        int parallelNum = GlobalConfig.Config.MaxDegreeOfParallelism;
                        List<DownTransmission> list = new List<DownTransmission>();
                        foreach (KeyValuePair<string, DownTransmission> kv in _slaveCacheList)
                        {
                            list.Add(kv.Value);
                            if (list.Count >= parallelNum)
                            {
                                break;
                            }
                        }

                        Parallel.ForEach(list, (t) =>
                        {
                            _defaultWorker.DoWork(t.CalculateContext);

                            DownTransmission downTransmission = null;

                            _slaveCacheList.TryRemove(t.Key, out downTransmission);

                            _slaveCache.DeleteCache(t.Key);

                            if (downTransmission != null)
                            {
                                Logger.Log.Info(false, $"{downTransmission.Key},处理任务，内存计算任务数量:" + _slaveCacheList.Count.ToString());

                                if (t.CalculateContext.CalculateInpute != null)
                                    t.CalculateContext.CalculateInpute.DataSource = null;

                                if (t.CalculateContext.CalculateOutput != null)
                                    t.CalculateContext.CalculateOutput.DataSource = null;

                                t.CalculateContext = null;
                                t = null;
                            }

                            downTransmission = null;
                        });

                        list.Clear();
                        list = null;
                    }
                }
                catch(ThreadInterruptedException)
                {
                    break;
                }
                catch(Exception ex)
                {
                    Logger.Log.Info(true, "子节点任务执行异常:", ex);
                }
                
                Thread.Sleep(GlobalConfig.Config.SlaveExcuteCalculateInterval);
            }
        }

        private void LoadCache()
        {
            if (GlobalConfig.Config.NodeType == NodeType.Slave)
            {
                ConcurrentDictionary<string, byte[]> dic = _slaveCache.GetCacheDictionary();

                foreach (KeyValuePair<string, byte[]> kv in dic)
                {
                    if (kv.Value!=null && kv.Value.Length>0)
                    {
                        try
                        {

                            DownTransmission[] downTrans=TransmissionUtil.DecompressAndDeserialize<DownTransmission>(kv.Value);

                            foreach(DownTransmission down in downTrans)
                            {
                                _slaveCacheList.TryAdd(kv.Key, down);
                            }

                            downTrans = null;
                        }
                        catch (Exception ex)
                        {
                            Logger.Log.Info(true, "加载子节点任务:", ex);
                        }
                    }
                }

                dic.Clear();
                dic = null;
            }
        }

        public void AddTask(byte[] taskMsg)
        {
            try
            {
                DownTransmission[] downTrans = TransmissionUtil.DecompressAndDeserialize<DownTransmission>(taskMsg);

                foreach(DownTransmission down in downTrans)
                {
                    byte[] data = TransmissionUtil.SerializeAndCompress<DownTransmission>(down);

                    _slaveCache.WriteCache(down.Key, data);

                    _slaveCacheList.TryAdd(down.Key, down);

                    SendBackOk(down);

                    Logger.Log.Info(false, $"{down.Key},增加任务，内存计算任务数量:" + _slaveCacheList.Count.ToString());

                    data = null;
                }

                downTrans = null;
                taskMsg = null;
            }
            catch (Exception ex)
            {
                Logger.Log.Error(true, "任务解析异常:", ex);
            }
        }

        private void SendBackOk(DownTransmission downTrans)
        {
            UpTransmisstion upTransmisstion = new UpTransmisstion()
            {
                Key = downTrans.Key,
                Code = UpTransmisstionCode.Ok,
                Desc = UpTransmisstion.GetDesc(UpTransmisstionCode.Ok),
            };

            byte[] data = TransmissionUtil.SerializeAndCompress<UpTransmisstion>(upTransmisstion);

            string remoteInfo;
            _channelMessageHandler.Send(String.Empty, data, out remoteInfo);

            upTransmisstion = null;
            data = null;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                _excuteCalcExit = true;

                _excuteCalcThread.Join(GlobalConfig.Config.SlaveExcuteCalculateInterval*2);

                try
                {
                    _excuteCalcThread.Interrupt();
                }
                catch
                { }
             
                _slaveCacheList.Clear();
            }

            _disposed = true;
        }
    }
}
