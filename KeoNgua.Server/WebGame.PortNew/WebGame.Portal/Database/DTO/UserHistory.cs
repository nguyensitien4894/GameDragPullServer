using System;
using TraditionGame.Utilities.Utils;

namespace MsWebGame.Portal.Database.DTO
{
    public class UserHistory
    {
        public long PosBalance { get; set; }

        public long Amount { get; set; }

        public string Description { get; set; }

        public int ExchangeType { get; set; }
        public DateTime CreatedTime { get; set; }
        public string PosBalanceFormat
        {
            get
            {
                return PosBalance.LongToMoneyFormat();
            }
        }
        public string AmountFormat
        {
            get
            {
                return Amount.LongToMoneyFormat();
            }
        }



    }
}