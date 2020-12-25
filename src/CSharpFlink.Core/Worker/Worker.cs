using CSharpFlink.Core.Calculate;
using CSharpFlink.Core.Log;
using CSharpFlink.Core.Model;
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
            List<ICalculateOutput> outputs = new List<ICalculateOutput>();
            foreach(ICalculate calc in context.CalculateOperators)
            {
                ICalculateOutput output=calc.Calc(context.CalculateInpute);
                if(output!=null)
                {
                    outputs.Add(output);
                }
            }
#if DEBUG
            List<string> vals = new List<string>();
#endif
            if (outputs != null && outputs.Count>0)
            {
                foreach (ICalculateOutput output in outputs)
                {
                    foreach (SinkFunction sf in context.Sinks)
                    {
                        try
                        {
                            sf.Open();

                            sf.Invoke(output.DataSource, null);

                            sf.Close();
#if DEBUG
                            foreach (IMetaData md in output.DataSource)
                            {
                                vals.Add(md.TagValue);
                            }
#endif
                        }
                        catch (Exception ex)
                        {
                            Logger.Log.Error(true, sf.GetType().ToString(), ex);
                        }
                    }
                }

                if (PublishCalculateCompleted != null)
                {
                    context.CalculateOutputs = outputs;

                    PublishCalculateCompleted(context);
                }
#if DEBUG
                string resultVals=String.Join(",", vals);
                Logger.Log.Info(false, $"{context.Name}_{this.Name}_{context.Desc}-线程({Thread.CurrentThread.ManagedThreadId.ToString("0000")})：【{context.LeftTime.ToString()}-{context.RightTime.ToString()}】,【Result】:{resultVals}");
#else
                Logger.Log.Info(false, $"{context.Name}_{this.Name}_{context.Desc}-线程({Thread.CurrentThread.ManagedThreadId.ToString("0000")})：【{context.LeftTime.ToString()}-{context.RightTime.ToString()}】,【Result】:{outputs.Count.ToString()}");
#endif

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
