using System;
using System.Collections.Generic;
using System.Text;

namespace CSharpFlink.Core.Calculate
{
    public interface ICalculate
    {
        string ResultId { get; set; }

        ICalculateOutput Calc(ICalculateInpute input);
    }
}
