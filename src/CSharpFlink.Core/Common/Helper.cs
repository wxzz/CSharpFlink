using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace CSharpFlink.Core.Common
{
    public static class Helper
    {
        readonly static string Path = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "help.txt");
        public static void PrintHelper()
        {
            if(System.IO.File.Exists(Path))
            {
                string txt = System.IO.File.ReadAllText(Path,Encoding.UTF8);

                Console.Write(txt);
            }
            //Console.WriteLine("CSharpFlink 程序集版本:" + Assembly.GetExecutingAssembly().GetName().Version.ToString());
            //Console.WriteLine();
            //Console.WriteLine("命令行:");
            //Console.WriteLine(" -h         显示命令行帮助。");
            //Console.WriteLine(" -c         加载指定配置文件。例如:CSharpFlink -c c:/my.cfg");
            //Console.WriteLine(" -t         加载任务程序集。例如:CSharpFlink -t c:/mytask.dll");
            //Console.WriteLine();
            //Console.WriteLine("欢迎使用:CSharpFlink!!!祝您好运!!!");
        }
    }
}