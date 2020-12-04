using CSharpFlink.Core.Calculate;
using CSharpFlink.Core.Expression;
using CSharpFlink.Core.Model;
using CSharpFlink.Core.Window;
using CSharpFlink.Core.Window.Operator;
using System;
using System.Collections.Generic;
using System.Text;

namespace CSharpFlink.Core.RPC
{
    public class RpcContext
    {
        public string TaskId { get; set; }

        /// <summary>
        /// 计算任务名
        /// </summary>
        public string TaskName { get; set; }

        /// <summary>
        /// 计算类型
        /// </summary>
        public CalculateType CalculateType { get; set; }       
       
        /// <summary>
        /// 计算周期
        /// </summary>
        public int WindowInterval { get; set; }

        /// <summary>
        /// 窗口延迟个数
        /// </summary>
        public int DelayWindowCount { get; set; }

        /// <summary>
        /// 聚合计算类型
        /// </summary>
        public AggregateCalculateType AggregateCalculateType { get; set; }

        /// <summary>
        /// 表达式脚本
        /// </summary>
        public string ExpressionScript { get; set; }

        ///// <summary>
        ///// 表达式类型
        ///// </summary>
        public ExpressionCalculateType ExpressionCalculateType { get; set; }

        /// <summary>
        /// 保存计算结果的ID
        /// </summary>
        public string ResultId { get; set; }
    }  
}
