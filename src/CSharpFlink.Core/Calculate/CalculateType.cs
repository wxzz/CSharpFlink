using System;
using System.Collections.Generic;
using System.Text;

namespace CSharpFlink.Core.Calculate
{
    public enum CalculateType : byte
    {
        /// <summary>
        /// 聚合计算
        /// </summary>
        Aggregate=0x00,

        /// <summary>
        /// 表达式计算
        /// </summary>
        Expression=0x01
    }
}
