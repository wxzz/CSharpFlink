using CSharpFlink.Core.Calculate;
using CSharpFlink.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSharpFlink.Core.Window.Operator
{
    public class Min : Calculate.Calculate
    {
        public Min(string resultId) : base(resultId)
        {

        }

        public override ICalculateOutput Calc(ICalculateInpute input)
        {
            if (input.DataSource.Any())
            {
                double result = input.DataSource.Min(t => double.Parse(t.TagValue));
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
