using CSharpFlink.Core.Calculate;
using System;
using System.Collections.Generic;
using System.Text;

namespace CSharpFlink.Core.Protocol
{
    public class DownTransmission
    {
        public DownTransmission(string key,CalculateContext context)
        {
            Key = key;
            CalculateContext = context;
        }

        public string Key { get; set; }

        public CalculateContext CalculateContext { get; set; }

        public static string GetNewTransmissionId(string taskId,DateTime leftTime,DateTime rightTime)
        {
            return $"{taskId}_{leftTime.ToString("yyyyMMddHHmmss")}_{rightTime.ToString("yyyyMMddHHmmss")}_"+Guid.NewGuid().ToString("n");
        }
    }
}
