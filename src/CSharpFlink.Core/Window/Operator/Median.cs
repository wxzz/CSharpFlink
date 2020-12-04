using CSharpFlink.Core.Calculate;
using CSharpFlink.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSharpFlink.Core.Window.Operator
{
    /// <summary>
    /// 中位数计算
    /// </summary>  
    public class Median : Calculate.Calculate
    {
        public Median(string resultId) : base(resultId)
        {

        }

        public override ICalculateOutput Calc(ICalculateInpute input)
        {
            if (input.DataSource.Any())
            {
                int lstCount = input.DataSource.Count();
                double result;
                if (lstCount == 1)
                {
                    double.TryParse(input.DataSource.FirstOrDefault().TagValue, out result);
                }
                else
                {
                    //从小到大排序
                    //var sourceData = input.DataSource.ToList();
                    //sourceData.Sort((IMetaData p1, IMetaData p2) =>
                    //{
                    //    double.TryParse(p1.TagValue, out double value1);
                    //    double.TryParse(p2.TagValue, out double value2);
                    //    return value1.CompareTo(value2);
                    //});  
                    var sourceData = input.DataSource.OrderBy(t =>
                    {
                        double.TryParse(t.TagValue, out double value);
                        return value;
                    }).ToList();
                    int index = lstCount / 2;
                    int remainder = lstCount % 2;
                    double.TryParse(sourceData[index].TagValue, out result);
                    if (remainder.Equals(0))
                    {
                        double.TryParse(sourceData[index - 1].TagValue, out double prevResult);
                        result = (prevResult + result) / 2;
                    }
                }

                IMetaData md = input.DataSource.First();

                return new CalculateOutput(input.SessinId, DateTime.Now,
                       new MetaData[] {
                           new MetaData(){
                              TagTime=input.InputeDateTime,
                              TagValue=result.ToString(),
                              TagId=ResultId,
                              TagName=md.TagName,
                              Code=md.Code,
                              WindowId=md.WindowId,
                              ExtValue=md.ExtValue
                           }
                      });
            }
            else
            {
                return null;
            }
        }
    }
}


