using CSharpFlink.Core.Calculate;
using CSharpFlink.Core.Execution;
using CSharpFlink.Core.Log;
using CSharpFlink.Core.Node;
using CSharpFlink.Core.Protocol;
using CSharpFlink.Core.Window;
using CSharpFlink.Core.Window.Operator;
using System;
using System.Collections.Generic;
using TestCommon;
using System.Linq;

namespace TestTask
{
    class Program
    {
        static IExecutionEnvironment executionEnvironment;
        static string Path = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "tasks", "tasks.cfg");

        public static int TaskCount = 1;
        static int _windowInterval = 5;
        static int _delayWindowCount = 0;
        static List<ICalculate> _calculates = null;
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

            List<string> tasks = new List<string>();
            for (int i = 0; i < TaskCount; i++)
            {
                string key = i.ToString("0000");
                if (!executionEnvironment.TaskManager.ContainsWindow(key))
                {
                    _windowInterval = Calc.GetRandomWindowInterval();
                    _windowInterval = 5;
                    _calculates = Calc.GetAggRandomCalculateList(new string[] { key + "_result1", key + "_result2" });

                    executionEnvironment.TaskManager.AddOrUpdateWindowTask(key, $"窗口{key}", true, _windowInterval, _delayWindowCount, _calculates);

                    string t = String.Format("{0},{1},{2}", key,_windowInterval,_calculates.Count.ToString ());

                    tasks.Add(t);
                }
            }

            List<string> arr = new List<string>();
            arr.Add(Environment.NewLine);
            var cls = tasks.GroupBy(t => t.Split(",")[2]).Select(t => (new { Key = t.Key, Count = t.Count()}));

            foreach(var ele in cls)
            {
                arr.Add("计算统计:"+ele.Key + ":" + ele.Count.ToString());
            }

            var wls = tasks.GroupBy(t => t.Split(",")[1]).Select(t => (new { Key = t.Key, Count = t.Count() }));

            foreach (var ele in wls)
            {
                arr.Add("窗口统计:" + ele.Key + ":" + ele.Count.ToString());
            }

            tasks.AddRange(arr);

            string path = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "tasks", "current_tasks.txt");

            if(System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }

            System.IO.File.AppendAllLines(path, tasks.ToArray());

            executionEnvironment.AddSource(new RandomSourceFunction());

            executionEnvironment.AddSink(new SinkFunction());

            executionEnvironment.ExcuteSource();

            while (true)
            {
                var line = Console.ReadLine();
                if (line == "exit")
                {
                    ((ExecutionEnvironment)executionEnvironment).Stop();
                    break;
                }
            }
        }
    }
}
