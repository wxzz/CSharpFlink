using CSharpFlink.Core.Calculate;
using CSharpFlink.Core.Expression;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpFlink.Core.RPC
{
    public delegate void AddWindowHandler(string windowId, string windowName, int windowInterval, int delayWindowCount, ICalculate calc);

    public delegate void AddExpressionHandler(string expId, string expName, ExpressionCalculateType expCalculateType, int timerInterval, string script);

    public delegate void RemoveWindowHandler(string windowId);

    public delegate void RemoveExpressionHandler(string expId);
}
