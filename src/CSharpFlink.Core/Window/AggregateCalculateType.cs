using System;
using System.Collections.Generic;
using System.Text;

namespace CSharpFlink.Core.Window
{
    public enum AggregateCalculateType : byte
    {
        C_5S_Sum=0x00,
        C_5S_Avg,
        C_5S_Max,
        C_5S_Min,

        C_1M_Sum,
        C_1M_Avg,
        C_1M_Max,
        C_1M_Min,

        C_5M_Sum,
        C_5M_Avg,
        C_5M_Max,
        C_5M_Min,

        C_1H_Sum,
        C_1H_Avg,
        C_1H_Max,
        C_1H_Min,
    }
}
