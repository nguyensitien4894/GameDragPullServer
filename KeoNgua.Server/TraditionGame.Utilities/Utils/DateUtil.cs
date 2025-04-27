using System;

namespace TraditionGame.Utilities.Utils
{
    public class DateUtil
    {
        public static string TimeSpanToString(TimeSpan time)
        {
            if (time == null) return String.Empty;
            return string.Format("{0:D2}:{1:D2}:{2:D2}", time.Hours, time.Minutes, time.Seconds); ;
        }
        public static string DateTimeToString()
        {
            return DateTime.Now.ToString("ddMMyyyyHHmmss");
        }
        
        /// <summary>
        /// lấy ra ngày bắt đầu và ngày kết thúc của quý  dựa vào ngày hiện tại
        /// </summary>
        /// <param name="datetime"></param>
        /// <param name="dtFirstDay"></param>
        /// <param name="dtLastDay"></param>
        public static void GetQuatarStartEndDate(DateTime datetime, out int currQuarter, out DateTime dtFirstDay, out DateTime dtLastDay)
        {

            currQuarter = (datetime.Month - 1) / 3 + 1;
            dtFirstDay = new DateTime(datetime.Year, 3 * currQuarter - 2, 1);
            dtLastDay = dtFirstDay.AddMonths(3).AddDays(-1);
            //dtLastDay = new DateTime(datetime.Year, 3 * currQuarter + 1, 1).AddDays(-1);
        }
    }
}
