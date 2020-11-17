using CSharpFlink.Core.Calculate;
using CSharpFlink.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSharpFlink.Core.Window.Operator
{
    public class Avg : Calculate.Calculate
    {
        public override ICalculateOutput Calc(ICalculateInpute input)
        {
            if (input.DataSource.Any())
            {
                double result = input.DataSource.Average(t => double.Parse(t.tag_value));

                IMetaData md = input.DataSource.First();

                return new CalculateOutput(input.SessinId,DateTime.Now,
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
