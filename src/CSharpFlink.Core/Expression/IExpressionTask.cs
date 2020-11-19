using CSharpFlink.Core.Calculate;
using CSharpFlink.Core.Task;
using System.Collections.Generic;

namespace CSharpFlink.Core.Expression
{
    public interface IExpressionTask : ICalculateTask
    {
        ExpressionCalculateType ExpressionCalculateType { get; set; }
        List<string> PatternDataList { get; set; }
        string Script { get; set; }
        int TimerInterval { get; set; }
    }
}