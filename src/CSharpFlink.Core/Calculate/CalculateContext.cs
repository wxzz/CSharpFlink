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
        public CalculateContext(string name,string desc, DateTime leftTime, DateTime rightTime, CalculateType calculateType,ICalculateInpute inpute, List<ICalculateOutput> outputs, List<ICalculate> calculateOperators)
        {
            Name = name;
            Desc = desc;
            LeftTime = leftTime;
            RightTime = rightTime;
            CalculateType = calculateType;
            CalculateInpute = inpute;
            CalculateOutputs = outputs;
            CalculateOperators = calculateOperators;

            IExecutionEnvironment env = ExecutionEnvironment.GetExecutionEnvironment(null);
            if (env == null)
            {
                Sinks = new List<SinkFunction>();
            }
            else
            {
                Sinks = new List<SinkFunction>(((ExecutionEnvironment)env).Sinks);
            }
        }

        public ICalculateInpute CalculateInpute { get; set; }

        public List<ICalculateOutput> CalculateOutputs { get; set; }

        public List<ICalculate> CalculateOperators { get; set; }

        public string Desc { get; set; }

        public string Name { get; set; }

        public DateTime LeftTime { get; set; }

        public DateTime RightTime { get; set; }

        public CalculateType CalculateType { get; set; }

        public List<SinkFunction> Sinks { get; set; }
    }
}
