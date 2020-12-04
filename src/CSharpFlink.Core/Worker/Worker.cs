using CSharpFlink.Core.Calculate;
using CSharpFlink.Core.Log;
using CSharpFlink.Core.Sink;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace CSharpFlink.Core.Worker
{
    public class Worker :  IWorker
    {
        public string Id { get; set; }
        public string Name { get; set; }

        internal readonly object SyncLock = new object();

        public event PublishCalculateCompleted PublishCalculateCompleted;

        public Worker(string id, string name)
        {
            Id = id;
            Name = name;
        }

        public void DoWork(ICalculateContext context)
        {
            ICalculateOutput output = context.CalculateOperator.Calc(context.CalculateInpute);

            if (output != null)
            {
                foreach (SinkFunction sf in context.Sinks)
                {
                    try
                    {
                        sf.Open();

                        sf.Invoke(output.DataSource, null);

                        sf.Close();
                    }
                    catch (Exception ex)
                    {
                        Logger.Log.Error(true, sf.GetType().ToString (), ex);
                    }
                }

                if (PublishCalculateCompleted != null)
                {
                    context.CalculateOutput = output;

                    PublishCalculateCompleted(context);
                }
            }

            if (output != null)
            {
                Logger.Log.Info(false, $"{context.Name}_{this.Name}_{context.Desc}-线程({Thread.CurrentThread.ManagedThreadId.ToString("0000")})：【{context.LeftTime.ToString()}-{context.RightTime.ToString()}】,【Result】:{output.DataSource[0].TagValue}");
            }
            else
            {
                Logger.Log.Info(false, $"{context.Name}_{this.Name}_{context.Desc}-线程({Thread.CurrentThread.ManagedThreadId.ToString("0000")})：【{context.LeftTime.ToString()}-{context.RightTime.ToString()}】,【Result】:计算结果为空");
            }
        }

        public static IWorker GetDefaultWorker()
        {
            return new Worker(Guid.NewGuid().ToString("n"), "Default Worker");
        }
    }
}
