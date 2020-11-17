using System;
using System.Collections.Generic;
using System.Text;

namespace CSharpFlink.Core.Source
{
    public abstract class SourceFunction
    {
        internal SourceContext Context; 

        public SourceFunction()
        {
            Context = new SourceContext();
            Interval = 1000;
        }

        public int Interval { get; set; }

        public abstract void Init();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context">SourceFunction</param>
        public abstract void Run(object context);

        public abstract void Cancel();
    }
}
