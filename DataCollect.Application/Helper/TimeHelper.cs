﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataCollect.Application.Helper
{
    public class TimeHelper
    {
        public static DateTime ConvertToDateTime(string dateTimeString)
        {
            if (dateTimeString.Length < 26)
            {
                return default(DateTime);
            }
            try
            {
                var year = Convert.ToInt32(dateTimeString.Substring(4, 2)) + 2000;
                var month = Convert.ToInt32(dateTimeString.Substring(7, 2));
                var day = Convert.ToInt32(dateTimeString.Substring(10, 2));
                var hour = Convert.ToInt32(dateTimeString.Substring(13, 2));
                var minute = Convert.ToInt32(dateTimeString.Substring(16, 2));
                var second = Convert.ToInt32(dateTimeString.Substring(19, 2));
                var millisecond = Convert.ToInt32(dateTimeString.Substring(22, 3));

                var dateTime = new DateTime(year, month, day, hour, minute, second, millisecond);
                return dateTime;
            }
            catch (Exception)
            {
                return default(DateTime);
            }
        }

        public static string ConvertToDateTimeString(string type, DateTime dateTime)
        {
            var year = dateTime.Year.ToString().Substring(2, 2);
            var month = dateTime.Month.ToString("00");
            var day = dateTime.Day.ToString("00");
            var hour = dateTime.Hour.ToString("00");
            var minute = dateTime.Minute.ToString("00");
            var second = dateTime.Second.ToString("00");
            var millisecond = dateTime.Millisecond.ToString();
            var dateTimeFormat = year + "." + month + "." + day + "-" + hour + ":" + minute + ":" + second + "," + millisecond;
            var dateLength = dateTimeFormat.Length.ToString("0000");
            var dateTimeString = type + dateLength + dateTimeFormat;
            return dateTimeString;
        }
        public static long DateTimeToLongS(DateTime dateTime)
        {
            var startTime = TimeZoneInfo.ConvertTimeToUtc(new DateTime(1970, 1, 1, 8, 0, 0, 0)); // 当地时区
            long timeStamp = (long)(dateTime.ToUniversalTime() - startTime).TotalMilliseconds; // 相差秒数
            return timeStamp;
        }
        public static long DateTimeToLongS10(DateTime dateTime)
        {
            var startTime = TimeZoneInfo.ConvertTimeToUtc(new DateTime(1970, 1, 1, 8, 0, 0, 0)); // 当地时区
            long timeStamp = (long)(dateTime.ToUniversalTime() - startTime).TotalSeconds; // 相差秒数
            return timeStamp;
        }
    }
}
