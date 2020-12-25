using CSharpFlink.Core.Calculate;
using CSharpFlink.Core.Model;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSharpFlink.Core.Expression.Operator
{
    public class ExpressionCalculate : Calculate.Calculate
    {
        public ExpressionCalculate(string resultId) : base(resultId)
        {

        }

        public override ICalculateOutput Calc(ICalculateInpute input)
        {
            if (input.DataSource.Any())
            {
                string script = input.Script;
                string[] patternDataList = ExpressionTask.GetPatternDataList(script);
                foreach(string pattern in patternDataList)
                {
                    IMetaData md = input.DataSource.FirstOrDefault(t => t.TagId == pattern);

                    if(md!=null)
                    {
                        script = script.Replace($"[{pattern}]", md.TagValue.ToString());
                    }
                }
                ScriptOptions scriptOptions = ScriptOptions.Default.WithImports("System.Math");    //支持Math包含的方法运算
                object result = CSharpScript.EvaluateAsync(script,scriptOptions).Result;

                return new CalculateOutput(input.SessinId, DateTime.Now,
                       new MetaData[] {
                           new MetaData(){
                             TagId=ResultId,
                             TagTime=input.InputeDateTime,
                             TagValue=result.ToString()
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
