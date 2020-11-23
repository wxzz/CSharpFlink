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
    }
}
