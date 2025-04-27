using System;
using TraditionGame.Utilities.Utils;

namespace MsWebGame.CSKH.Database.DTO
{
    public class CardRechargeTopList
    {
        public long UserID { get; set; }
        public string  Username { get; set; }
        public string DisplayName { get; set; }
        public DateTime ? CreatedTime { get; set; }
        public long AmountCharge { get; set; }
        public long AmountUserCharge { get; set; }
        public int ChargeCnt { get; set; }
       
        public string AmountUserChargeFormat { get
            {
                return AmountUserCharge.LongToMoneyFormat();
            }
        }
        public string AmountChargeFormat
        {
            get
            {
                return AmountCharge.LongToMoneyFormat();
            }
        }


        public string ChargeCntFormat
        {
            get
            {
                return ChargeCnt.IntToMoneyFormat();
            }
            
        }
    }
}