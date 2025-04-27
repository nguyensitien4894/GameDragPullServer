using System;

namespace HorseHunter.Server.DataAccess.DTO
{
    public class History
    {
        public long SpinID { get; set; }
        public int BetValue { get; set; }
        public int TotalLines { get; set; }
        public int TotalBetValue { get; set; }
        public long TotalPrizeValue { get; set; }
        public string SlotsData { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}