using CSharpFlink.Core.Calculate;
using CSharpFlink.Core.Model;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSharpFlink.Core.Expression.Operator
{
    public class ExpressionCalculate : Calculate.Calculate
    {
        public override ICalculateOutput Calc(ICalculateInpute input)
        {
            if (input.DataSource.Any())
            {
                string script = input.Script;
                string[] patternDataList = ExpressionTask.GetPatternDataList(script);
                foreach(string pattern in patternDataList)
                {
                    IMetaData md = input.DataSource.FirstOrDefault(t => t.tag_id == pattern);

                    if(md!=null)
                    {
                        script = script.Replace($"[{pattern}]", md.tag_value.ToString());
                    }
                }

                object result = CSharpScript.EvaluateAsync(script).Result;

                return new CalculateOutput(input.SessinId, DateTime.Now,
                       new MetaData[] {
                           new MetaData(){
                             tag_time=input.InputeDateTime,
                             tag_value=result.ToString()
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
