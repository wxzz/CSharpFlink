using System;
using System.Collections.Generic;
using System.Text;

namespace CSharpFlink.Core.Common
{
    public class DateTimeUtil
    {
        private static DateTime _timeStampStartTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        /// <summary>
        /// 10位时间戳（单位：秒）转换为DateTime
        /// </summary>
        /// <param name="timeStamp">10位时间戳（单位：秒）</param>
        /// <returns>DateTime</returns>
        public static DateTime TimeStampToDateTime(long timeStamp)
        {
            return _timeStampStartTime.AddSeconds(timeStamp).ToLocalTime();
        }

        /// <summary>
        /// DateTime转换为10位时间戳（单位：秒）
        /// </summary>
        /// <param name="dateTime"> DateTime</param>
        /// <returns>10位时间戳（单位：秒）</returns>
        public static long DateTimeToTimeStamp(DateTime dateTime)
        {
            return (long)(dateTime.ToUniversalTime() - _timeStampStartTime).TotalSeconds;
        }
    }
}
