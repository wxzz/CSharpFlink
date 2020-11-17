using CSharpFlink.Core.Calculate;
using CSharpFlink.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSharpFlink.Core.Window.Operator
{
    /// <summary>
    /// 标准方差计算∑(Xi-X)^2/(n-1)
    /// </summary>   
    public class Variance : Calculate.Calculate
    {
        public override ICalculateOutput Calc(ICalculateInpute input)
        {
            if (input.DataSource.Any())
            {
                double avgValue = input.DataSource.Average(t =>
                {
                    double.TryParse(t.tag_value,out double value);
                    return value;
                    });
                double squareDiffSum = input.DataSource.Sum(t =>
                {
                    double.TryParse(t.tag_value, out double value);
                    return Math.Pow((value - avgValue), 2);
                });
               
                double result = squareDiffSum / (input.DataSource.Count() - 1);

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
