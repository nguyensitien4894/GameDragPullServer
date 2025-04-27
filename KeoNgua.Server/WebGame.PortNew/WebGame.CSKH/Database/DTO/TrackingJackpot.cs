using System;
using TraditionGame.Utilities.Utils;

namespace MsWebGame.CSKH.Database.DTO
{
    public class TrackingJackpot
    {
        public long AccountID { get; set; }

        public string UserName { get; set; }

        public int JackpotNum { get; set; }
        public string JackpotNumFormat
        {
            get { return JackpotNum.IntToMoneyFormat(); }
        }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public long Period { get; set; }
        public string PeriodFormat
        {
            get { return Period.LongToMoneyFormat(); }
        }

        public long TimeFrequency { get; set; }
        public string TimeFrequencyFormat
        {
            get { return TimeFrequency.LongToMoneyFormat(); }
        }
    }
}