using CSharpFlink.Core.Calculate;
using CSharpFlink.Core.Common;
using CSharpFlink.Core.Expression;
using CSharpFlink.Core.Expression.Operator;
using CSharpFlink.Core.Log;
using CSharpFlink.Core.Model;
using CSharpFlink.Core.Task;
using CSharpFlink.Core.Window;
using CSharpFlink.Core.Window.Operator;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CSharpFlink.Core.RPC
{
    public class RpcTaskExcute : IRpcTaskExcute
    {
        private IMasterTaskManager TaskManager { get; set; }

        public RpcTaskExcute(IMasterTaskManager taskManager)
        {
            TaskManager = taskManager;
        }

        public bool InsertOrUpdate(RpcContext[] rpcContexts)
        {
            try
            {
                foreach (RpcContext item in rpcContexts)
                {
                    if (item.CalculateType == CalculateType.Aggregate)
                    {
                        Calculate.Calculate calEntity = WindowTaskUtil.GetAggregateCalculate(item.ResultId,item.AggregateCalculateType);
                        TaskManager.AddOrUpdateWindowTask(item.TaskId, item.TaskName,item.IsOpenWindow, item.WindowInterval, item.DelayWindowCount, calEntity);
                    }
                    else if(item.CalculateType == CalculateType.Expression)
                    {
                        Calculate.Calculate calEntity = new ExpressionCalculate(item.ResultId);
                        TaskManager.AddOrUpdateExpressionTask(item.TaskId, item.TaskName, item.ExpressionCalculateType, item.WindowInterval, item.ExpressionScript, calEntity);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Logger.Log.Error(true, "执行InsertOrUpdate出错", ex);
                return false;
            }
        }

        public bool Remove(RpcContext[] rpcContexts)
        {
            try
            {
                foreach (RpcContext item in rpcContexts)
                {
                    if (item.CalculateType == CalculateType.Aggregate)
                    {                      
                        TaskManager.RemoveWindowTask(item.TaskId);
                    }
                    else if (item.CalculateType == CalculateType.Expression)
                    {                      
                        TaskManager.RemoveExpressionTask(item.TaskId);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Logger.Log.Error(true, "执行Remove出错", ex);
                return false;
            }
        }

        public bool AddMetaData(MetaData[] mds)
        {
            try
            {
                foreach (MetaData metaData in mds)
                {
                    TaskManager.AddMetaData(metaData.WindowId, metaData);
                }
                return true;
            }
            catch (Exception ex)
            {
                Logger.Log.Error(true, "执行AddMetaData出错", ex);
                return false;
            }
        }

        public bool ContainsTaskId(string taskId, CalculateType calculateType)
        {
            bool containsFlag = false;
            try
            {
                if (calculateType == CalculateType.Aggregate)
                {
                    containsFlag= TaskManager.ContainsWindow(taskId);
                }
                else if (calculateType == CalculateType.Expression)
                { 
                    containsFlag= TaskManager.ContainsExpression(taskId);
                }
                return containsFlag;
            }
            catch (Exception ex)
            {
                Logger.Log.Error(true, "执行ContainsTaskId出错", ex);
                return containsFlag;
            }
        }

        public string[] GetAllTaskId(CalculateType calculateType)
        {
            string[] taskIds = null;
            try
            {
                if (calculateType == CalculateType.Aggregate)
                {
                    taskIds= TaskManager.GetAllWindowId();
                }
                else if (calculateType == CalculateType.Expression)
                {
                    taskIds = TaskManager.GetAllExpressionId();
                }
                return taskIds;
            }
            catch (Exception ex)
            {
                Logger.Log.Error(true, "执行GetAllTaskId出错", ex);
                return taskIds;
            }
        }
    }
}
