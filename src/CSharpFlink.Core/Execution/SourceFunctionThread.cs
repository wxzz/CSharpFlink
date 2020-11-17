using CSharpFlink.Core.Source;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace CSharpFlink.Core.Execution
{
    internal class SourceFunctionThread
    {
        internal SourceFunction SourceFunction { get; set; }

        internal Thread Thread { get; set; }

        internal SourceFunctionThread(SourceFunction sf,Thread t)
        {
            SourceFunction = sf;
            Thread = t;
        }
    }
}
