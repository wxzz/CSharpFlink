using System;
using System.Collections.Generic;
using System.Text;

namespace CSharpFlink.Core.Calculate
{
    public abstract class Calculate : ICalculate
    {
        public Calculate(string resultId)
        {
            ResultId = resultId;
        }


        public string ResultId { get; set; }

        public abstract ICalculateOutput Calc(ICalculateInpute input);
    }
}
