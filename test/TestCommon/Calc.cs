using CSharpFlink.Core.Calculate;
using CSharpFlink.Core.Expression.Operator;
using CSharpFlink.Core.Model;
using CSharpFlink.Core.Window;
using CSharpFlink.Core.Window.Operator;
using System;
using System.Collections.Generic;
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

        public static ICalculate GetAggRandomCalculate(string resultId)
        {
            int num = _random.Next(0, 7);
            return WindowTaskUtil.GetAggregateCalculate(resultId,(AggregateCalculateType)num);
        }

        public static List<ICalculate> GetAggRandomCalculateList(string[] resultIdList)
        {
            List<ICalculate> list = new List<ICalculate>();
            foreach(string s in resultIdList)
            {
                list.Add(GetAggRandomCalculate(s));
            }
            return list;
        }

        public static ICalculate GetExpCalculate(string resultId)
        {
            return new ExpressionCalculate(resultId);
        }

        public static IMetaData GetMetaData(string id, DataType dataType,int delayWindowCount,int windowInterval)
        {
            IMetaData md;
            double val = _random.NextDouble() * 1000;
            if (DataType.RtData == dataType)
            {
                md = new MetaData()
                {
                    WindowId=id,
                    TagId = id,
                    TagTime = DateTime.Now,
                    TagValue = val.ToString()
                };
            }
            else if (DataType.HisData == dataType)
            {
                int sub = 0 - (_random.Next(0, ((delayWindowCount-1) * windowInterval)));
                DateTime dt = DateTime.Now.AddSeconds(sub);

                md = new MetaData()
                {
                    WindowId = id,
                    TagId = id,
                    TagTime = dt,
                    TagValue = val.ToString()
                };
            }
            else
            {
                md = new MetaData()
                {
                    WindowId = id,
                    TagId = id,
                    TagTime = DateTime.Now,
                    TagValue = val.ToString()
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
