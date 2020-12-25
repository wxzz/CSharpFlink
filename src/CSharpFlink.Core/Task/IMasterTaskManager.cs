using CSharpFlink.Core.Calculate;
using CSharpFlink.Core.Expression;
using CSharpFlink.Core.Model;
using CSharpFlink.Core.Protocol;
using CSharpFlink.Core.Window;
using System;
using System.Collections.Generic;

namespace CSharpFlink.Core.Task
{
    public interface IMasterTaskManager : IDisposable
    {
        void AddMetaData(string windowId, IMetaData md);
        void AddOrUpdateWindowTask(string windowId, string windowName,bool isOpenWindow,int windowInterval, int delayWindowCount, List<ICalculate> calcs);
        void RemoveWindowTask(string windowId);

        void AddOrUpdateExpressionTask(string expId, string expName, ExpressionCalculateType expCalculateType, int timerInterval, string script, List<ICalculate> calcs);

        void RemoveExpressionTask(string expId);

        bool ContainsWindow(string windowId);

        IWindowTask GetWindow(string windowId);
        string[] GetAllWindowId();

        bool ContainsExpression(string expId);

        IExpressionTask GetExpression(string expId);

        string[] GetAllExpressionId();

        void AddWorker(string id, string name, PublishCalculateCompleted calculateCallback);

        void RemoveWorker(string id);

        void RemoveCache(byte[] upMsg);
    }
}