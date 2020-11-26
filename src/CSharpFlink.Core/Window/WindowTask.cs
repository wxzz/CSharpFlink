using CSharpFlink.Core.Calculate;
using CSharpFlink.Core.Common;
using CSharpFlink.Core.Log;
using CSharpFlink.Core.Model;
using CSharpFlink.Core.Task;
using CSharpFlink.Core.Window.Operator;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Timers;

namespace CSharpFlink.Core.Window
{
    public class WindowTask : IWindowTask
    {
        private bool _disposed = false;

        private DataPool _cache;

        private ConcurrentQueue<DataPool> _delayCache;

        private System.Timers.Timer _timer;

        private readonly object SyncLock = new object();

        public string Id { get; set; }

        public string Name { get; set; }

        public IMetaData Current { get; set; }
        /// <summary>
        /// 当前窗口时间范围，秒
        /// </summary>
        public int WindowInterval { get; set; }

        public ICalculate CalculateOperator { get; set; }

        /// <summary>
        /// 延迟多少窗口，防止后来数据，需要重新计算
        /// </summary>
        public int DelayWindowCount { get; set; }

        public IGlobalContext GlobalContext { get; internal set; }

        public WindowTask(string tagWindowId, string tagWindowName, ICalculate calc)
        {
            Id = tagWindowId;
            Name = tagWindowName;
            DelayWindowCount = 0;
            WindowInterval = 5;

            if (calc == null)
            {
                CalculateOperator = new Avg();
            }
            else
            {
                CalculateOperator = calc;
            }

            _cache = new DataPool();
            
            _delayCache = new ConcurrentQueue<DataPool>();

            SlidTimeWindow(DateTime.Now);

            _timer = new System.Timers.Timer(1000);
            _timer.Elapsed += OnTimedEvent;
            _timer.AutoReset = true;
            _timer.Enabled = true;
        }

        public WindowTask(string tagWindowId, string tagWindowName, int windowInterval,int delayWindowCount, ICalculate calc) 
        {
            Id = tagWindowId;
            Name = tagWindowName;
            WindowInterval = windowInterval;
            DelayWindowCount = delayWindowCount;

            if (calc == null)
            {
                CalculateOperator = new Avg();
            }
            else
            {
                CalculateOperator = calc;
            }

            _cache = new DataPool();

            _delayCache = new ConcurrentQueue<DataPool>();

            SlidTimeWindow(DateTime.Now);

            _timer = new System.Timers.Timer(1000);
            _timer.Elapsed += OnTimedEvent;
            _timer.AutoReset = true;
            _timer.Enabled = true;
        }

        ~WindowTask()
        {
            Dispose(false);
        }

        private void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            DateTime nowTime = DateTime.Now;
            long curLong = DateTimeUtil.DateTimeToTimeStamp(nowTime);
            if ((curLong % WindowInterval) == 0)
            {
                lock (SyncLock)
                {
                    if (DelayWindowCount > 0)
                    {
                        this.CheckDelayCache();
                    }

                    DateTime leftDT = _cache.LeftTime;
                    DateTime rightDT = _cache.RightTime;

                    SlidTimeWindow(nowTime);

                    PublishCalculate(Id, DateTime.Now, leftDT, rightDT, _cache.Pool.ToArray(), "实时数据");

                    _cache.Clear();
                }
            }
        }

        private void PublishCalculate(string sessinId, DateTime nowTime, DateTime leftTime,DateTime rightTime,IMetaData[] ds, string desc)
        {
            if (ds.Length > 0)
            {
                ICalculateContext calculateContext = new CalculateContext(Name, desc + "_" + CalculateOperator.GetType().ToString(), leftTime, rightTime, CalculateType.Aggregate, new CalculateInpute(sessinId, "", rightTime, ds), null, CalculateOperator);

                GlobalContext.ActionBlock.Post(calculateContext);

                ds = null;
            }
            else
            {
                Logger.Log.Info(true, Name + " 没有要发布的窗口任务");
            }
        }

        private void SlidTimeWindow(DateTime dateTime)
        {
            long curLong = DateTimeUtil.DateTimeToTimeStamp(dateTime);

            int offsetTime = (int)(curLong % WindowInterval);

            _cache.LeftTime = DateTimeUtil.TimeStampToDateTime(curLong - offsetTime);
            _cache.RightTime = _cache.LeftTime.AddSeconds(WindowInterval);

            Console.WriteLine($"{Name}-SlidTimeWindow:{CalculateOperator.GetType().ToString ()}_{WindowInterval.ToString()}:{_cache.LeftTime.ToString()}-{_cache.RightTime.ToString()}");
        }

        private void CheckDelayCache()
        {
            //复制当前数据，到延迟组中。
            DataPool cacheClone = (DataPool)this._cache.Clone();

            _delayCache.Enqueue(cacheClone);

            int removeNum = _delayCache.Count - DelayWindowCount;
            for (int i = 0; i < removeNum; i++)
            {
                DataPool removeCache;
                _delayCache.TryDequeue(out removeCache);
                if (removeCache != null)
                {
                    removeCache.Dispose();
                    removeCache = null;
                }
            }
        }

        public void AddMeteData(IMetaData md)
        {
            //this.CheckCalculateOperator();

            this.Current = md;
            if (md.tag_time >= _cache.LeftTime
                && md.tag_time < _cache.RightTime)
            {
                IMetaData oldMD = _cache.Pool.Where(t => t.tag_time.ToString() == md.tag_time.ToString()).FirstOrDefault();
                if (oldMD == null)
                {
                    _cache.Pool.Add(md);
                }
                else
                {
                    oldMD.tag_value = md.tag_value;
                }
            }
            else
            {
                foreach (DataPool dp in _delayCache)
                {
                    if (md.tag_time >= dp.LeftTime
                     && md.tag_time < dp.RightTime)
                    {
                        IMetaData oldMD = dp.Pool.Where(t => t.tag_time.ToString () == md.tag_time.ToString ()).FirstOrDefault();
                        if (oldMD == null)
                        {
                            dp.Pool.Add(md);
                        }
                        else
                        {
                            oldMD.tag_value = md.tag_value;
                        }

                        //重新计算
                        PublishCalculate(Id, DateTime.Now, dp.LeftTime,dp.RightTime, dp.Pool.ToArray(), "补发数据");

                        break;
                    }
                }
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
                if(_delayCache!=null)
                {
                    foreach(DataPool dp in _delayCache)
                    {
                        dp.Dispose();
                    }

                    _delayCache.Clear();
                }

                if(_cache!=null)
                {
                    _cache.Dispose();
                }

                if(_timer!=null)
                {
                    _timer.Stop();
                    _timer.Dispose();
                }
            }

            _disposed = true;
        }
    }
}
