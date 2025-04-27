using System;

namespace TraditionGame.Utilities.Utils
{
    public static class DateTimeExtension
    {
        public static DateTime AbsoluteStart(this DateTime dateTime)
        {
            return dateTime.Date;
        }
        public static String GetTimestamp(this DateTime value)
        {
            return value.ToString("yyyy-MM-dd HH:mm:ss.fff");
        }
        /// <summary>
        /// Gets the 11:59:59 instance of a DateTime
        /// </summary>
        public static DateTime AbsoluteEnd(this DateTime dateTime)
        {
            return AbsoluteStart(dateTime).AddDays(1).AddTicks(-1);
        }
        /// <summary>
        /// kiểm tra xem date có trong khoảng thời gian nào đó
        /// </summary>
        /// <param name="dateToCheck"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public static bool InRange(this DateTime dateToCheck, DateTime startDate, DateTime endDate)
        {
            return dateToCheck >= startDate && dateToCheck <= endDate;
        }
    }
}

