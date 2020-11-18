using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSharpFlink.Core.Log
{
    public class ConsoleContainer:ILogContainer
    {
        public void ShowLog(string log)
        {
            Console.WriteLine(log);
        }
    }
}
