using CSharpFlink.Core.Calculate;
using System;
using System.Collections.Generic;
using System.Text;

namespace CSharpFlink.Core.Task
{
    public interface ITask : IDisposable
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
