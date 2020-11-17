using System;
using System.Collections.Generic;
using System.Text;

namespace TestCommon
{
    public enum AggregateCalculateType : byte
    {
        C_Sum = 0x00,
        C_Avg = 0x01,
        C_Max = 0x02,
        C_Min = 0x03,
    }
}
