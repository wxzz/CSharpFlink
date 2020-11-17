using CSharpFlink.Core.Calculate;
using CSharpFlink.Core.Model;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace CSharpFlink.Core.Window
{
    public class DataPool : ICloneable,IDisposable
    {
        private bool _disposed = false;

        public ConcurrentBag<IMetaData> Pool { get; set; }

        public DateTime LeftTime { get; set; }

        public DateTime RightTime { get; set; }

        public DataPool()
        {
            Pool = new ConcurrentBag<IMetaData>();
            LeftTime = DateTime.Now;
            RightTime = DateTime.Now;
        }

        ~DataPool()
        {
            Dispose(false);
        }

        public object Clone()
        {
            DataPool dp = new DataPool();
            dp.LeftTime = this.LeftTime;
            dp.RightTime = this.RightTime;
            IMetaData[] metaDatas = this.Pool.ToArray();
            foreach (MetaData md in metaDatas)
            {
                dp.Pool.Add(md);
            }
            metaDatas = null;
            return dp;
        }

        public void Clear()
        {
            Pool.Clear();
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
               
            }

            this.Pool.Clear();
            this.Pool = null;

            this._disposed = true;
        }
    }
}
