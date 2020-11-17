using System;
using System.Collections.Generic;
using System.Text;

namespace CSharpFlink.Core.Calculate
{
    public abstract class Calculate : ICalculate
    {
        public abstract ICalculateOutput Calc(ICalculateInpute input);
    }
}
