using CSharpFlink.Core.Common;
using CSharpFlink.Core.Execution;
using CSharpFlink.Core.Log;
using CSharpFlink.Core.Node;
using Microsoft.CodeAnalysis.CSharp;
using System;

namespace CSharpFlink
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                ExecutionEnvironment env = (ExecutionEnvironment)ExecutionEnvironment.GetExecutionEnvironment(args);

                bool wait = false;
                if (args.Length > 0)
                {
                    if(args[0]=="-h")
                    {
                        Helper.PrintHelper();
                    }
                    else
                    {
                        wait = true;
                        env.TaskAssembly.ExcuteMain(args);
                    }
                }
                else
                {
                    wait = true;
                    env.TaskAssembly.ExcuteMain(args);
                }

                env.ExcuteSource();

                if (wait)
                {
                    while (true)
                    {
                        string cmd = Console.ReadLine().ToLowerInvariant();
                        if (cmd == "exit"
                            || cmd == "quit"
                            || cmd == "stop")
                        {
                            env.Stop();
                            env = null;
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Info(true, "", ex);
            }
        }

        #region 不用
//         try
//            {
//                ExecutionEnvironment executionEnvironment = ExecutionEnvironment.GetExecutionEnvironment();
//                if (args != null)
//                {
//                    if (args.Length >= 1)
//                    {
//                        if (args[0] == NodeType.Master.ToString()
//                          || args[0] == NodeType.Slave.ToString())
//                        {
//                            NodeType nt = (NodeType)Enum.Parse(typeof(NodeType), args[0]);
//        executionEnvironment.Start(nt);
//                    }
//                    else
//                        {
//                            Logger.Log.Info(false, "命令参数无法识别");
//                        }
//                    }
//                    else
//{
//    Logger.Log.Info(false, "没有可识别的命令参数");
//}
//                }
//                else
//{
//    executionEnvironment.Start(NodeType.Master);
//    executionEnvironment.Start(NodeType.Slave);
//}

//while (true)
//{
//    string cmd = Console.ReadLine();
//    if (cmd.ToLower() == "stop")
//    {
//        executionEnvironment.Stop();
//    }
//}
//            }
//            catch (Exception ex)
//{
//    Logger.Log.Info(true, "", ex);
//}
        #endregion
    }
}
