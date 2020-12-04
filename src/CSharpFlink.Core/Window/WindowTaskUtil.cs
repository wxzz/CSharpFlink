using CSharpFlink.Core.Window.Operator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpFlink.Core.Window
{
    public class WindowTaskUtil
    {
        public static Calculate.Calculate GetAggregateCalculate(string resultId,AggregateCalculateType calType)
        {
            Calculate.Calculate calEntity;
            switch (calType)
            {
                case AggregateCalculateType.C_Avg:
                    calEntity = new Avg(resultId);
                    break;
                case AggregateCalculateType.C_Max:
                    calEntity = new Max(resultId);
                    break;
                case AggregateCalculateType.C_Min:
                    calEntity = new Min(resultId);
                    break;
                case AggregateCalculateType.C_Mode:
                    calEntity = new Mode(resultId);
                    break;
                case AggregateCalculateType.C_Sum:
                    calEntity = new Sum(resultId);
                    break;
                case AggregateCalculateType.C_Variance:
                    calEntity = new Variance(resultId);
                    break;
                case AggregateCalculateType.C_Median:
                    calEntity = new Median(resultId);
                    break;
                default:
                    calEntity = new Avg(resultId);
                    break;
            }
            return calEntity;
        }
    }
}
