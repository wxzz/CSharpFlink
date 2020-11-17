using CSharpFlink.Core.Calculate;
using CSharpFlink.Core.Execution;
using CSharpFlink.Core.Log;
using CSharpFlink.Core.Node;
using CSharpFlink.Core.Protocol;
using CSharpFlink.Core.Window;
using CSharpFlink.Core.Window.Operator;
using System;
using TestCommon;

namespace TestTask
{
    class Program
    {
        static ExecutionEnvironment executionEnvironment;
        static string Path = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "tasks", "tasks.cfg");

        public static int TaskCount = 1;
        static int _windowInterval = 5;
        static int _delayWindowCount = 0;
        static ICalculate _calculate = new Avg();
        static void Main(string[] args)
        {
            if (System.IO.File.Exists(Path))
            {
                try
                {
                    TaskCount = int.Parse(System.IO.File.ReadAllText(Path).Trim());
                }
                catch(Exception ex)
                {
                    Logger.Log.Error(true, "读TaskCount异常:", ex);
                }
            }

            executionEnvironment = ExecutionEnvironment.GetExecutionEnvironment(args);

            for (int i = 0; i < TaskCount; i++)
            {
                string key = i.ToString("0000");
                if (!executionEnvironment.TaskManager.ContainsWindow(key))
                {
                    _windowInterval = Calc.GetRandomWindowInterval();
                    _calculate = Calc.GetAggRandomCalculate();

                    executionEnvironment.TaskManager.AddWindowTask(key, $"窗口{key}", _windowInterval, _delayWindowCount, _calculate);
                }
            }

            executionEnvironment.AddSource(new RandomSourceFunction());

            executionEnvironment.AddSink(new SinkFunction());

            executionEnvironment.ExcuteSource();

            //while (true)
            //{
            //    var line = Console.ReadLine();
            //    if (line == "exit")
            //    {
            //        executionEnvironment.Stop();
            //        break;
            //    }
            //}
        }
    }
}
