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
        public override ICalculateOutput Calc(ICalculateInpute input)
        {
            if (input.DataSource.Any())
            {
                int lstCount = input.DataSource.Count();
                double result;
                if (lstCount == 1)
                {
                    double.TryParse(input.DataSource.FirstOrDefault().tag_value, out result);
                }
                else
                {
                    //从小到大排序
                    //var sourceData = input.DataSource.ToList();
                    //sourceData.Sort((IMetaData p1, IMetaData p2) =>
                    //{
                    //    double.TryParse(p1.tag_value, out double value1);
                    //    double.TryParse(p2.tag_value, out double value2);
                    //    return value1.CompareTo(value2);
                    //});  
                    var sourceData = input.DataSource.OrderBy(t =>
                    {
                        double.TryParse(t.tag_value, out double value);
                        return value;
                    }).ToList();
                    int index = lstCount / 2;
                    int remainder = lstCount % 2;
                    double.TryParse(sourceData[index].tag_value, out result);
                    if (remainder.Equals(0))
                    {
                        double.TryParse(sourceData[index - 1].tag_value, out double prevResult);
                        result = (prevResult + result) / 2;
                    }
                }

                IMetaData md = input.DataSource.First();

                return new CalculateOutput(input.SessinId, DateTime.Now,
                       new MetaData[] {
                           new MetaData(){
                              tag_time=input.InputeDateTime,
                              tag_value=result.ToString(),
                              tag_id=md.tag_id,
                              tag_name=md.tag_name,
                              code=md.code,
                              window_id=md.window_id,
                              ext_value=md.ext_value
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


