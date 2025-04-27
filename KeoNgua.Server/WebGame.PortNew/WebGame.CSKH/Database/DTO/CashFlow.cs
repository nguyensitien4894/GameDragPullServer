using TraditionGame.Utilities.Utils;

namespace MsWebGame.CSKH.Database.DTO
{
    public class CashFlow
    {
        public long SellAgencyTotal { get; set; }
        public string SellAgencyTotalFormat { get { return SellAgencyTotal.LongToMoneyFormat(); } }

        public long BuyAgencyTotal { get; set; }
        public string BuyAgencyTotalFormat { get { return BuyAgencyTotal.LongToMoneyFormat(); } }

        public long SellUserTotal { get; set; }
        public string SellUserTotalFormat { get { return SellUserTotal.LongToMoneyFormat(); } }

        public long BuyUserTotal { get; set; }
        public string BuyUserTotalFormat { get { return BuyUserTotal.LongToMoneyFormat(); } }

        public long DeltaAgency { get { return BuyAgencyTotal - SellAgencyTotal; } }
        public string DeltaAgencyFormat { get { return DeltaAgency.LongToMoneyFormat(); } }

        public long DeltaUser { get { return BuyUserTotal - SellUserTotal; } }
        public string DeltaUserFormat { get { return DeltaUser.LongToMoneyFormat(); } }
    }
}