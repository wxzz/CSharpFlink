using CSharpFlink.Core.Execution;
using CSharpFlink.Core.Node;
using System;

namespace TestSlaveNode
{
    class Program
    {
        static ExecutionEnvironment execution;
        static void Main(string[] args)
        {
            args = new string[] { "-c", System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "cfg", "slave.cfg") };
            execution = (ExecutionEnvironment)ExecutionEnvironment.GetExecutionEnvironment(args);
            TestSlaveClient();
            Console.ReadLine();
        }

        static void TestSlaveClient()
        {
            while (true)
            {
                var line = Console.ReadLine();
                if (line == "exit")
                {
                    execution.Stop();
                    break;
                }
            }
        }
    }
}
