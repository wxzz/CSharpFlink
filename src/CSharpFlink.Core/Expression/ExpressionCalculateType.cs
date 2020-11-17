using System;
using System.Collections.Generic;
using System.Text;

namespace CSharpFlink.Core.Expression
{
    public enum ExpressionCalculateType : byte
    {
        /// <summary>
        /// 定时计算
        /// </summary>
        TimerCalculate=0x00,

        /// <summary>
        /// 值改变计算
        /// </summary>
        ValueChangedCalculate=0x01,
    }
}
