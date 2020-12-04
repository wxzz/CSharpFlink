using System;
using System.Collections.Generic;
using System.Text;

namespace CSharpFlink.Core.Window
{
    public enum AggregateCalculateType : byte
    {
        C_Sum = 0x00,
        C_Avg = 0x01,
        C_Max = 0x02,
        C_Min = 0x03,
        C_Median = 0x04,
        C_Mode = 0x05,
        C_Variance = 0x06
    }
}
