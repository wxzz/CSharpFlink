using CSharpFlink.Core.Calculate;
using CSharpFlink.Core.Task;
using System;
using System.Collections.Generic;
using System.Text;

namespace CSharpFlink.Core.Calculate
{
    public interface ICalculateTask : IDisposable
    {
        /// <summary>
        /// 窗口ID
        /// </summary>
        string Id { get; set; }

        /// <summary>
        /// 窗口名称
        /// </summary>
        string Name { get; set; }

        ICalculate CalculateOperator { get; set; }

        IGlobalContext GlobalContext { get; }
    }
}
