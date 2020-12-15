using CSharpFlink.Core.Cache;
using CSharpFlink.Core.Calculate;
using CSharpFlink.Core.Channel;
using CSharpFlink.Core.Common;
using CSharpFlink.Core.Config;
using CSharpFlink.Core.Expression;
using CSharpFlink.Core.Log;
using CSharpFlink.Core.Model;
using CSharpFlink.Core.Node;
using CSharpFlink.Core.Protocol;
using CSharpFlink.Core.Window;
using CSharpFlink.Core.Worker;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace CSharpFlink.Core.Task
{
    public class MasterTaskManager : IMasterTaskManager
    {
        private bool _disposed = false;

        private ConcurrentDictionary<string, IWindowTask> _winList;

        private ConcurrentDictionary<string, IExpressionTask> _expList;

        private ConcurrentDictionary<string, IWorker> _workList;

        private ConcurrentDictionary<string, byte[]> _masterCacheList;

        private MasterCache _masterCache;

        private IGlobalContext _context;

        private volatile int _currentWorkerIndex = -1;

        private IWorker _defaultWorker = null;

        private Thread _remoteInvokeThread;

        private bool _remoteInvokeExit = false;

        private IChannelMessageHandler _channelMessageHandler;

        public MasterTaskManager(IChannelMessageHandler channelMessageHandler)
        {
            _channelMessageHandler = channelMessageHandler;

            _winList = new ConcurrentDictionary<string, IWindowTask>();
            _expList = new ConcurrentDictionary<string, IExpressionTask>();
            _workList = new ConcurrentDictionary<string, IWorker>();
            _masterCacheList = new ConcurrentDictionary<string, byte[]>();
            _masterCache = new MasterCache();
            _context = new GlobalContext(new ActionBlock<ICalculateContext>(ParallelCalculate, new ExecutionDataflowBlockOptions() { MaxDegreeOfParallelism = GlobalConfig.Config.MaxDegreeOfParallelism }));

            LoadCache();

            _remoteInvokeThread = new Thread(new ThreadStart(RemoteInvokeThread));
            _remoteInvokeThread.Name = "RemoteInvokeThread";
            _remoteInvokeThread.IsBackground = true;
            _remoteInvokeThread.Start();
        }

        private void RemoteInvokeThread()
        {
            while(true)
            {
                if(_remoteInvokeExit)
                {
                    break;
                }

                try
                {
                    if (_workList.Count > 0)
                    {
                        if (_masterCacheList.Count > 0)
                        {
                            int taskNum = _masterCacheList.Count;
                            int workerNum = _workList.Count;
                            int parallelNum = 0;
                            if (taskNum >= workerNum)
                            {
                                parallelNum = workerNum;
                            }
                            else
                            {
                                parallelNum = taskNum % workerNum;
                            }

                            if (GlobalConfig.Config.WorkerPower > 1)
                            {
                                parallelNum *= GlobalConfig.Config.WorkerPower;
                            }

                            if (parallelNum > 0)
                            {
                                List<byte[]> parallelTasks = new List<byte[]>();
                                #region
                                foreach (KeyValuePair<string, byte[]> kv in _masterCacheList)
                                {
                                    if (kv.Value != null && kv.Value.Length > 0)
                                    {
                                        if (!MemoryCacheHelper.Exists(kv.Key))
                                        {
                                            parallelTasks.Add(kv.Value);

                                            MemoryCacheHelper.Set(kv.Key, DateTime.Now, TimeSpan.FromMilliseconds(GlobalConfig.Config.RepeatRemoteInvokeInterval), false);
                                        }

                                        if (parallelTasks.Count >= parallelNum)
                                        {
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        Logger.Log.Info(false, "发送任务,为空:"+kv.Key);
                                    }
                                }
                                #endregion

                                #region
                                if (parallelTasks.Count > 0)
                                {
                                    Parallel.ForEach(parallelTasks, (data) =>
                                    {
                                        IWorker worker = GetPollWorker();

                                        if (worker == null)
                                        {
                                            return;
                                        }

                                        lock (((Worker.Worker)worker).SyncLock)
                                        {
                                            if (data != null && data.Length > 0)
                                            {
                                                string remoteInfo = String.Empty;

                                                _channelMessageHandler.Send(worker.Id, data, out remoteInfo);

                                                data = null;

                                                Logger.Log.Info(false, "发送任务到:" + remoteInfo + ",子节点编号:" + worker.Id);
                                            }
                                            else
                                            {
                                                Logger.Log.Info(false, "发送任务,为空" );
                                            }
                                        }
                                    });
                                }
                                #endregion

                                parallelTasks.Clear();
                                parallelTasks = null;
                            }
                        }
                    }
                }
                catch(ThreadInterruptedException)
                {
                    break;
                }
                catch(Exception ex)
                {
                    Logger.Log.Error(true, "远程调用子节点:", ex);
                }

                Thread.Sleep(GlobalConfig.Config.RemoteInvokeInterval);
            }
        }

        public void RemoveCache(byte[] upMsg)
        {
            UpTransmisstion[] upTrans=TransmissionUtil.DecompressAndDeserialize<UpTransmisstion>(upMsg);
            foreach(UpTransmisstion up in upTrans)
            {
                if (up.Code == UpTransmisstionCode.Ok)
                {
                    string key = up.Key;

                    try
                    {
                        byte[] data;
                        _masterCacheList.TryRemove(key, out data);
                        data = null;
                    }
                    catch (Exception ex)
                    {
                        Logger.Log.Error(true, "删除任务缓存异常,key:" + key, ex);
                    }

                    try
                    {
                        _masterCache.DeleteCache(up.Key);
                    }
                    catch (Exception ex)
                    {
                        Logger.Log.Error(true, "删除目录缓存异常,key:" + key, ex);
                    }

                    Logger.Log.Info(false, $"{up.Key},删除任务，内存计算任务数量:" + _masterCacheList.Count.ToString());
                }
            }
            upTrans = null;
            upMsg = null;
        }

        private void LoadCache()
        {
            if(GlobalConfig.Config.NodeType==NodeType.Master)
            {
                ConcurrentDictionary<string,byte[]> dic = _masterCache.GetCacheDictionary();

                foreach(KeyValuePair<string, byte[]> kv in dic)
                {
                    if (kv.Value!=null && kv.Value.Length>0)
                    {
                        _masterCacheList.TryAdd(kv.Key, kv.Value);
                    }
                }

                dic.Clear();
                dic = null;
            }
        }

        public void AddOrUpdateWindowTask(string windowId, string windowName, bool isOpenWindow, int windowInterval, int delayWindowCount,ICalculate calc)
        {
            if (!_winList.ContainsKey(windowId))
            {
                IWindowTask window = new Core.Window.WindowTask(windowId, windowName, isOpenWindow,windowInterval,delayWindowCount, calc)
                {
                    GlobalContext = _context
                };
                _winList.TryAdd(windowId, window);
            }
            else
            {
                IWindowTask window;
                if (_winList.TryGetValue(windowId, out window))
                {
                    window.Name = windowName;
                    window.WindowInterval = windowInterval;
                    window.DelayWindowCount = delayWindowCount;
                    window.CalculateOperator = calc;
                    //window.AggregateCalculateType = ct;
                }
            }
        }

        public void RemoveWindowTask(string windowId)
        {
            if (_winList.ContainsKey(windowId))
            {
                IWindowTask windowTask;
                if(_winList.TryRemove(windowId,out windowTask))
                {
                    if(windowTask!=null)
                    {
                        windowTask.Dispose();
                        windowTask = null;
                    }
                }
            }
        }

        public IWindowTask GetWindow(string windowId)
        {
            return _winList[windowId];
        }
        public string[] GetAllWindowId()
        {
            return _winList.Keys.ToArray();
        }

        public bool ContainsWindow(string windowId)
        {
            return _winList.ContainsKey(windowId);
        }

        public bool ContainsExpression(string expId)
        {
            return _expList.ContainsKey(expId);
        }

        public IExpressionTask GetExpression(string expId)
        {
            return _expList[expId];
        }
        public string[] GetAllExpressionId()
        {
            return _expList.Keys.ToArray();
        }

        public void AddMetaData(string windowId, IMetaData md)
        {
            if (_winList.ContainsKey(windowId))
            {
                _winList[windowId].AddMeteData(md);

                foreach(KeyValuePair<string, IExpressionTask> kv in _expList)
                {
                    if (kv.Value.ExpressionCalculateType == ExpressionCalculateType.ValueChangedCalculate)
                    {
                        if (kv.Value.PatternDataList.Contains(md.TagId))
                        {
                            ((ExpressionTask)kv.Value).PublishCalculate();
                        }
                    }
                }
            }
        }

        public void AddOrUpdateExpressionTask(string expId, string expName, ExpressionCalculateType expCalculateType, int timerInterval, string script,ICalculate calc)
        {
            if (!_expList.ContainsKey(expId))
            {
                IExpressionTask expTask = new ExpressionTask(expId, expName, expCalculateType, timerInterval,script,calc)
                {
                    GlobalContext = _context
                };

                _expList.TryAdd(expId, expTask);
            }
            else
            {
                IExpressionTask expTask;
                if (_expList.TryGetValue(expId, out expTask))
                {
                    expTask.Name = expName;
                    expTask.Script = script;
                    expTask.ExpressionCalculateType = expCalculateType;
                    expTask.TimerInterval = timerInterval;
                    expTask.CalculateOperator = calc;
                }
            }
        }

        public void RemoveExpressionTask(string expId)
        {
            if (_expList.ContainsKey(expId))
            {
                IExpressionTask expTask;
                if(_expList.TryRemove(expId, out expTask))
                {
                    if(expTask!=null)
                    {
                        expTask.Dispose();
                        expTask = null;
                    }
                }
            }
        }

        public void AddWorker(string id,string name,PublishCalculateCompleted calculateCallback)
        {
            lock (_workList)
            {
                if (!_workList.ContainsKey(id))
                {
                    IWorker work = new Worker.Worker(id, name);
                    work.PublishCalculateCompleted += calculateCallback;
                    _workList.TryAdd(id, work);
                }
                else
                {
                    IWorker worker;
                    if (_workList.TryGetValue(id, out worker))
                    {
                        worker.Name = name;
                    }
                }

            }        }

        public void RemoveWorker(string id)
        {
            lock (_workList)
            {
                if (_workList.ContainsKey(id))
                {
                    IWorker worker;
                    if (_workList.TryRemove(id, out worker))
                    {
                        if (worker != null)
                        {
                            worker.PublishCalculateCompleted -= null;
                            worker = null;
                        }
                    }
                }
            }
        }

        private void ParallelCalculate(ICalculateContext context)
        {
            if(context==null)
            {
                Logger.Log.Info(true, "ParallelCalculate参数(context)为空");
                return;
            }

            try
            {
                bool isCalc = true;
                if (context.CalculateType == CalculateType.Expression)
                {
                    foreach (IMetaData md in context.CalculateInpute.DataSource)
                    {
                        if (_winList.ContainsKey(md.TagId))
                        {
                            md.TagName = _winList[md.TagId].Current.TagName;
                            md.TagTime = _winList[md.TagId].Current.TagTime;
                            md.TagValue = _winList[md.TagId].Current.TagValue;
                        }
                        else
                        {
                            isCalc = false;
                            break;
                        }
                    }
                }

                if (isCalc)
                {
                    if (_channelMessageHandler.ClientCount > 0)
                    {
                        #region
                        CalculateContext calcContext=(CalculateContext)context;

                        DownTransmission downTrans = new DownTransmission(DownTransmission.GetNewTransmissionId(calcContext.Name, calcContext.LeftTime,context.RightTime),calcContext);

                        byte[] data=TransmissionUtil.SerializeAndCompress<DownTransmission>(downTrans);

                        if(data!=null && data.Length>0)
                        {
                            _masterCache.WriteCache(downTrans.Key, data);

                            _masterCacheList.TryAdd(downTrans.Key, data);

                            Logger.Log.Info(false, $"{downTrans.Key},增加任务，内存计算任务数量:" + _masterCacheList.Count.ToString());
                        }

                        calcContext = null;
                        downTrans = null;
                        data = null;
                        #endregion
                    }
                    else
                    {
                        #region
                        if (_workList.Count <= 0)
                        {
                            if (_defaultWorker == null)
                            {
                                _defaultWorker = Worker.Worker.GetDefaultWorker();
                            }

                            _defaultWorker.DoWork(context);
                        }
                        else
                        {
                            IWorker worker = GetPollWorker();
                            if (worker != null)
                            {
                                worker.DoWork(context);
                            }
                        }
                        #endregion
                    }
                }
                context.CalculateInpute.DataSource = null;
                context = null;
            }
            catch (Exception ex)
            {
                Logger.Log.Info(true,$"{context.Name}-{context.Desc}-线程({Thread.CurrentThread.ManagedThreadId.ToString("0000")})：【{context.LeftTime.ToString()}-{context.RightTime.ToString()}】,异常:",ex);
            }
        }

        private IWorker GetPollWorker()
        {
            int curIndex = GetNextWorkerIndex();

            if (curIndex < 0)
            {
                Logger.Log.Info(true, "GetPollWorker:为空");
                return null;
            }
            else
            {
                return _workList.ElementAt(curIndex).Value;
            }
        }

        private int GetNextWorkerIndex()
        {
            lock (_workList)
            {
                int count = _workList.Count;

                if (count <= 0)
                {
                    return -1;
                }
                else
                {
                    if (_currentWorkerIndex == -1)
                    {
                        Interlocked.Exchange(ref _currentWorkerIndex, 0);
                    }
                    else
                    {
                        Interlocked.Increment(ref _currentWorkerIndex);

                        if (_currentWorkerIndex >= count)
                        {
                            Interlocked.Exchange(ref _currentWorkerIndex, 0);
                        }
                    }
                }

                return _currentWorkerIndex;
            }
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
                _remoteInvokeExit = true;

                _remoteInvokeThread.Join(GlobalConfig.Config.RemoteInvokeInterval*2);

                try
                {
                    _remoteInvokeThread.Interrupt();
                }
                catch
                { }

                foreach(KeyValuePair<string,IWindowTask> kv in _winList)
                {
                    kv.Value.Dispose();
                }
                _winList.Clear();
                foreach (KeyValuePair<string, IExpressionTask> kv in _expList)
                {
                    kv.Value.Dispose();
                }
                _expList.Clear();
                _workList.Clear();
                _masterCacheList.Clear();
                _context = null;
            }

            _disposed = true;
        }
       
    }
}
