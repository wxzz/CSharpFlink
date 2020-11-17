using CSharpFlink.Core.Calculate;
using CSharpFlink.Core.Expression.Operator;
using CSharpFlink.Core.Model;
using CSharpFlink.Core.Window.Operator;
using System;
using System.Runtime.InteropServices;

namespace TestCommon
{
    public class Calc
    {
        static Random _random = new Random();
        public static int GetWindowInterval(WindowIntervalType wit)
        {
            int interval = 5;
            switch (wit)
            {
                case WindowIntervalType.C_5S:
                    interval = 5;
                    break;
                case WindowIntervalType.C_1M:
                    interval = 60;
                    break;
                case WindowIntervalType.C_5M:
                    interval = 60 * 5;
                    break;
                case WindowIntervalType.C_1H:
                    interval = 60 * 60;
                    break;
            }
            return interval;
        }

        public static int GetRandomWindowInterval()
        {
            int num = _random.Next(0, 4);
            return GetWindowInterval((WindowIntervalType)num);
        }

        public static ICalculate GetAggCalculate(AggregateCalculateType act)
        {
            ICalculate co=null;
            if (act == AggregateCalculateType.C_Avg)
            {
                co = new Avg();
            }
            else if (act == AggregateCalculateType.C_Sum)
            {
                co = new Sum();
            }
            else if (act == AggregateCalculateType.C_Max)
            {
                co = new Max();
            }
            else if (act == AggregateCalculateType.C_Min)
            {
                co = new Min();
            }

            if(co==null)
            {
                co = new Avg();
            }    
            return co;
        }

        public static ICalculate GetAggRandomCalculate()
        {
            int num = _random.Next(0, 4);
            return GetAggCalculate((AggregateCalculateType)num);
        }

        public static ICalculate GetExpCalculate()
        {
            return new ExpressionCalculate();
        }

        public static IMetaData GetMetaData(string id, DataType dataType,int delayWindowCount,int windowInterval)
        {
            IMetaData md;
            double val = _random.NextDouble() * 1000;
            if (DataType.RtData == dataType)
            {
                md = new MetaData()
                {
                    window_id=id,
                    tag_id = id,
                    tag_time = DateTime.Now,
                    tag_value = val.ToString()
                };
            }
            else if (DataType.HisData == dataType)
            {
                int sub = 0 - (_random.Next(0, ((delayWindowCount-1) * windowInterval)));
                DateTime dt = DateTime.Now.AddSeconds(sub);

                md = new MetaData()
                {
                    window_id = id,
                    tag_id = id,
                    tag_time = dt,
                    tag_value = val.ToString()
                };
            }
            else
            {
                md = new MetaData()
                {
                    window_id = id,
                    tag_id = id,
                    tag_time = DateTime.Now,
                    tag_value = val.ToString()
                };
            }
            return md;
        }

        public static IMetaData GetRandomMetaData(string id,int delayWindowCount, int windowInterval)
        {
            int num = _random.Next(0, 2);

            return GetMetaData(id, (DataType)num, delayWindowCount, windowInterval);
        }
    }
}
