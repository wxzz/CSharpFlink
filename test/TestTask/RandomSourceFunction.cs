using CSharpFlink.Core.Log;
using CSharpFlink.Core.Model;
using CSharpFlink.Core.Source;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading;
using TestCommon;

namespace TestTask
{
    public class RandomSourceFunction : SourceFunction
    {
        private Random _random;
        private bool _isRun = true;
        private int _windowInterval = 5;
        private int _delayWindowCount = 2;
        private int _interval = 1000;

        public static string tag_id = "tag_001";

        public override void Cancel()
        {
            _isRun = false;
        }

        public override void Init()
        {
            Interval = _interval;
            _random = new Random();
        }

        public override void Run(object context)
        {
            while(_isRun)
            {
                try
                {
                    SourceContext sc = (SourceContext)context;
                    for (int i = 0; i < Program.TaskCount; i++)
                    {
                        string key = i.ToString("0000");

                        IMetaData md = Calc.GetMetaData(key, TestCommon.DataType.RtData, _delayWindowCount, _windowInterval);

                        sc.Collect((new IMetaData[] { md }).ToArray());
                    }
                }
                catch(ThreadInterruptedException ex)
                {
                    Logger.Log.Error(true, "RandomSourceFunction:", ex);
                    break;
                }
                catch(Exception ex)
                {
                    Logger.Log.Error(true, "RandomSourceFunction:", ex);
                }

                Thread.Sleep(Interval);
            }
        }
    }
}
