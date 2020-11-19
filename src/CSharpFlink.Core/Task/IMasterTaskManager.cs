using CSharpFlink.Core.Calculate;
using CSharpFlink.Core.Expression;
using CSharpFlink.Core.Model;
using CSharpFlink.Core.Protocol;
using CSharpFlink.Core.Window;
using System;

namespace CSharpFlink.Core.Task
{
    public interface IMasterTaskManager : IDisposable
    {
        void AddMetaData(string windowId, IMetaData md);
        void AddWindowTask(string windowId, string windowName,int windowInterval, int delayWindowCount,ICalculate calc);
        void RemoveWindowTask(string windowId);

        void AddExpressionTask(string expId, string expName, ExpressionCalculateType expCalculateType, int timerInterval, string script,ICalculate calc);

        void RemoveExpressionTask(string expId);

        bool ContainsWindow(string windowId);

        IWindowTask GetWindow(string windowId);

        void AddWorker(string id, string name, PublishCalculateCompleted calculateCallback);

        void RemoveWorker(string id);

        void RemoveCache(byte[] upMsg);
    }
}