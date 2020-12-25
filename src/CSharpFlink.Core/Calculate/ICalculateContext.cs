using CSharpFlink.Core.Calculate;
using CSharpFlink.Core.Sink;
using ProtoBuf;
using System;
using System.Collections.Generic;

namespace CSharpFlink.Core.Calculate
{
    public interface ICalculateContext
    {
        string Desc { get; set; }

        string Name { get; set; }

        public DateTime LeftTime { get; set; }

        public DateTime RightTime { get; set; }

        public CalculateType CalculateType { get; set; }

        List<ICalculate> CalculateOperators { get; set; }

        ICalculateInpute CalculateInpute { get; set; }

        List<ICalculateOutput> CalculateOutputs { get; set; }

        List<SinkFunction> Sinks { get; set; }
    }
}