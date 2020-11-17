using System;
using System.Collections.Generic;
using System.Text;

namespace CSharpFlink.Core.Common
{
    internal static class FormatArgs
    {
        /// <summary>
        /// 配置文件参数
        /// </summary>
        internal readonly static string c_c="-c";

        /// <summary>
        /// 指定任务文件名
        /// </summary>
        internal readonly static string c_t = "-t";

        internal static Dictionary<string,string> GetArgsDictionary(string[] args)
        {
            if (args == null || args.Length <= 0)
                return null;

            Dictionary<string, string> dic = new Dictionary<string, string>();
            for(int i=0;i<args.Length;i++)
            {
                if(((i+1) % 2)==0)
                {
                    dic.Add(args[i - 1], args[i]);
                }
            }
            return dic;
        }
    }
}
