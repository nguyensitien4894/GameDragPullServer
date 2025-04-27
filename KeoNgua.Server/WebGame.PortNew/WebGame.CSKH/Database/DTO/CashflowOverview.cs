using System;

namespace MsWebGame.CSKH.Database.DTO
{
    public class CashflowOverview
    {
        public DateTime CheckDate { get; set; }
        public string CheckDateFormat { get { return CheckDate.ToString("dd/MM/yyyy"); } }
        public long? TotalCardRechargeValue { get; set; }

        public long? TotalCardSwapValue { get; set; }

        public long? TotalAgencySellValue { get; set; }

        public long? TotalAgencyBuyValue { get; set; }

        public long? TotalGameProfit { get; set; }

        public long? TotalAgencyFee { get; set; }

        public long? TotalUserFee { get; set; }

        public long? TotalGiftcodeValue { get; set; }
        public long? TotalVP { get; set; }
        public long? TotalVqmmValue { get; set; }

        public long? TotalVipReward { get; set; }

        public long? TotalAgencyReward { get; set; }

        public long? TotalEventValue { get; set; }
        public long STT { get; set; }
    }
}