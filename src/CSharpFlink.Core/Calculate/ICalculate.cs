using System;
using System.Collections.Generic;
using System.Text;

namespace CSharpFlink.Core.Calculate
{
    public interface ICalculate
    {
        ICalculateOutput Calc(ICalculateInpute input);
    }
}
