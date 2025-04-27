using System;

namespace MsWebGame.CSKH.Database.DTO
{
    public class ReportAgencyL1CashFlowUsers
    {
        public DateTime TransDay { get; set; }
        public long SellAgencyTotal { get; set; }
        public long BuyAgencyTotal { get; set; }
        public long SellUserTotal { get; set; }
        public long BuyUserTotal { get; set; }
        public long BuyAgencyQuantity { get; set; }
        public long SellUserQuantity { get; set; }
        public long BuyUserQuantity { get; set; }
    }
}