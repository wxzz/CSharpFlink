using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSharpFlink.Core.Log
{
    public class LogFactory:ILogFactory
    {
        /// <summary>
        /// 创建日志实例
        /// </summary>
        /// <param name="name"></param>
        /// <param name="logContainer"></param>
        /// <returns></returns>
        public ILogger GetLog(string name,ILogContainer logContainer=null)
        {
            return new Logger(name,logContainer);
        }
    }
}
