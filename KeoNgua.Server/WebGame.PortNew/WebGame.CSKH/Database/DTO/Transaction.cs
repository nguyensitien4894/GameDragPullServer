using System;
using TraditionGame.Utilities.Utils;
using MsWebGame.CSKH.Utils;

namespace MsWebGame.CSKH.Database.DTO
{
    public class Transaction
    {
        public DateTime CreatedDate { get; set; }
        public int ServiceID { get; set; }
        public string ServiceIDFormat { get { return ServiceID.ConvertServiceID(); } }
        public long OverrunValue { get; set; }
        public string OverrunValueFormat { get { return OverrunValue.LongToMoneyFormat(); } }
        public long Balance { get; set; }
        public string BalanceFormat { get { return Balance.LongToMoneyFormat(); } }
        public string SessionID { get; set; }
        public string Description { get; set; }
    }
}