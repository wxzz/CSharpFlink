using CSharpFlink.Core.Calculate;
using CSharpFlink.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSharpFlink.Core.Window.Operator
{
    /// <summary>
    /// 众数计算
    /// </summary>
    public class Mode : Calculate.Calculate
    {
        public Mode(string resultId) : base(resultId)
        {

        }

        public override ICalculateOutput Calc(ICalculateInpute input)
        {
            if (input.DataSource.Any())
            {
                var source = input.DataSource.Select(t => t.TagValue).ToList();
                var modeSor = from item in source                             
                              group item by item into gro                           
                              orderby gro.Count() descending                            
                              select new { num = gro.Key, nums = gro.Count() };
                double.TryParse(modeSor.FirstOrDefault().num, out double result);
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
