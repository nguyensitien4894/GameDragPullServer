using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TraditionGame.Utilities.Utils;

namespace MsWebGame.Portal.Database.DTO
{
    public class GameHistory
    {
        public long SpinID { get; set; }
        public long Balance { get; set; }
        public long BetValue { get; set; }
        public long PrizeValue { get; set; }
        public long RefundValue { get; set; }
        public  string AmountFormat
        {
            get
            {
                var tmpAmount= ((PrizeValue + RefundValue) - BetValue);
                return tmpAmount.LongToMoneyFormat();
            }
        }
        public int GameType { get; set; }
        public string Description { get; set; }
        public DateTime PlayTime { get; set; }
        public DateTime CreatedTime { get; set; }
        public string BalanceFormat
        {
            get
            {
                return Balance.LongToMoneyFormat();
            }
        }
        public string BetValueFormat
        {
            get
            {
                return BetValue.LongToMoneyFormat();
            }
        }
        public string PrizeValueFormat
        {
            get
            {
                return PrizeValue.LongToMoneyFormat();
            }
        }
        public string RefundValueFormat
        {
            get
            {
                return RefundValue.LongToMoneyFormat();
            }
        }
    }
}