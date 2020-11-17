using CSharpFlink.Core.Calculate;
using CSharpFlink.Core.Execution;
using CSharpFlink.Core.Sink;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace CSharpFlink.Core.Calculate
{
    public class CalculateContext : ICalculateContext
    {
        public CalculateContext(string name,string desc, DateTime leftTime, DateTime rightTime, CalculateType calculateType,ICalculateInpute inpute, ICalculateOutput output, ICalculate calculateOperator)
        {
            Name = name;
            Desc = desc;
            LeftTime = leftTime;
            RightTime = rightTime;
            CalculateType = calculateType;
            CalculateInpute = inpute;
            CalculateOutput = output;
            CalculateOperator = calculateOperator;
            Sinks = new List<SinkFunction>(ExecutionEnvironment.Sinks);
        }

        public ICalculateInpute CalculateInpute { get; set; }

        public ICalculateOutput CalculateOutput { get; set; }

        public ICalculate CalculateOperator { get; set; }

        public string Desc { get; set; }

        public string Name { get; set; }

        public DateTime LeftTime { get; set; }

        public DateTime RightTime { get; set; }

        public CalculateType CalculateType { get; set; }

        public List<SinkFunction> Sinks { get; set; }
    }
}
