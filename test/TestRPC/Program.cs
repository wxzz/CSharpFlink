using CSharpFlink.Core.Calculate;
using CSharpFlink.Core.Common;
using CSharpFlink.Core.Config;
using CSharpFlink.Core.Execution;
using CSharpFlink.Core.Expression.Operator;
using CSharpFlink.Core.Log;
using CSharpFlink.Core.Model;
using CSharpFlink.Core.RPC;
using CSharpFlink.Core.Task;
using CSharpFlink.Core.Window;
using CSharpFlink.Core.Window.Operator;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using TestCommon;

namespace TestRPC
{
    class Program
    {
        static IExecutionEnvironment executionEnvironment;
        static string Path = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "tasks", "tasks.cfg");
        public static int TaskCount = 5;
        static RpcClient rpcClient;
        static List<RpcContext> rpcContexts;
        static RpcContext rpcContext;
        static void Main(string[] args)
        {
            rpcContexts = new List<RpcContext>();
            if (System.IO.File.Exists(Path))
            {
                try
                {
                    TaskCount = int.Parse(System.IO.File.ReadAllText(Path).Trim());
                }
                catch (Exception ex)
                {
                    Logger.Log.Error(true, "读TaskCount异常:", ex);
                }
            }

            executionEnvironment = ExecutionEnvironment.GetExecutionEnvironment(args);

            rpcClient = new RpcClient(GlobalConfig.Config.RpcIp, GlobalConfig.Config.RpcListenPort);

            ClientTaskAdd();

            executionEnvironment.AddSource(new RandomSourceFunction(rpcClient));

            executionEnvironment.AddSink(new SinkFunction());

            executionEnvironment.ExcuteSource();

            ClientTaskRemove();

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

        static void ClientTaskAdd()
        {
            rpcContexts.Clear();
            int[] windowInterval = new int[] { 5, 60 };
            for (int i=0;i< TaskCount;i++)
            {
                string key = i.ToString("0000");
                rpcContext = new RpcContext
                {
                    TaskId = key,
                    CalculateType = CalculateType.Aggregate,
                    IsOpenWindow=true,
                    //WindowInterval = Calc.GetRandomWindowInterval(),
                    WindowInterval = 5,
                    TaskName = $"窗口{i:0000}计算",
                    AggregateCalculateTypeList =new AggregateCalculateType[] { (AggregateCalculateType)new Random().Next(0, 7) },
                    DelayWindowCount = 2,
                    ResultIdList=new string[] { $"{key}_result" }
                };
                rpcContexts.Add(rpcContext);
            }                 
            rpcClient.InsertOrUpdate(rpcContexts.ToArray());            
            Logger.Log.Info(false, "RPC客户端本次添加任务完毕" + new string('=', 50));
        }
        static void ClientTaskRemove()
        {
            Thread.Sleep(TimeSpan.FromSeconds(100));
            rpcContexts.Clear();

            for (int i = 0; i < TaskCount-1; i++)
            {
                rpcContext = new RpcContext
                {
                    TaskId = i.ToString("0000"),
                    CalculateType = CalculateType.Aggregate
                };
                rpcContexts.Add(rpcContext);
            }
            rpcClient.Remove(rpcContexts.ToArray());
            Logger.Log.Info(false, "RPC客户端本次删除任务完毕"+new string('=',50));
            ClientTaskAdd();
        }
    }
}
