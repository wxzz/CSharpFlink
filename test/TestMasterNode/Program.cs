using CSharpFlink.Core.Calculate;
using CSharpFlink.Core.Channel;
using CSharpFlink.Core.Execution;
using CSharpFlink.Core.Model;
using CSharpFlink.Core.Node;
using CSharpFlink.Core.Protocol;
using CSharpFlink.Core.Window;
using CSharpFlink.Core.Window.Operator;
using System;
using System.Collections.Generic;
using System.Threading;
using TestCommon;

namespace TestMasterNode
{
    class Program
    {
        static int _initTagNum = 1;
        static int _windowInterval = 5;
        static int _delayWindowCount = 0;
        static ICalculate _calculate = null;

        static IExecutionEnvironment execution;
        static void Main(string[] args)
        {
            args = new string[] { "-c", System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "cfg", "master.cfg") };
            execution = ExecutionEnvironment.GetExecutionEnvironment(args);

            TestMasterNode();
            Console.ReadLine();
        }

        static void TestMasterNode()
        {
            Console.WriteLine("请输入任务数:");

            _initTagNum = int.Parse(Console.ReadLine());

            while (true)
            {
                for (int i = 0; i < _initTagNum; i++)
                {
                    string key = i.ToString("0000");
                    if (!execution.TaskManager.ContainsWindow(key))
                    {
                        _windowInterval = Calc.GetRandomWindowInterval();
                        _calculate = Calc.GetAggRandomCalculate(key + "_result");

                        execution.TaskManager.AddOrUpdateWindowTask(key, $"窗口{key}", true, _windowInterval, _delayWindowCount, new List<ICalculate>() { new Avg($"{key}_avg"),new Max($"{key}_max") });

                        IMetaData md = Calc.GetMetaData(key, TestCommon.DataType.RtData, _delayWindowCount, _windowInterval);

                        //execution.AddSink(new TestTask.SinkFunction());

                        execution.TaskManager.AddMetaData(key, md);
                    }
                    else
                    {
                        IMetaData md = Calc.GetMetaData(key, TestCommon.DataType.RtData,_delayWindowCount, _windowInterval);
                        execution.TaskManager.AddMetaData(key, md);
                    }
                }
                Thread.Sleep(1000);
            }
        }
    }
}
