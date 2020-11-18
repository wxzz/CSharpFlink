using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSharpFlink.Core.Log
{
    public interface ILogFactory
    {
        ILogger GetLog(string name, ILogContainer logContainer);
    }
}
