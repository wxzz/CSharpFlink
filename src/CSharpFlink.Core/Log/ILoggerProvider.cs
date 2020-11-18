using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSharpFlink.Core.Log
{
    public interface ILoggerProvider
    {
        ILogger Logger { get; }
    }
}
