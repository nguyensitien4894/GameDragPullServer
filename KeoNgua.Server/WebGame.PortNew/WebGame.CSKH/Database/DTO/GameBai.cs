using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TraditionGame.Utilities.Utils;

namespace MsWebGame.CSKH.Database.DTO
{
    public class GameBai
    {
        public string Username { get; set; }
        public long BetValue { get; set; }
        public long AwardValue { get; set; }

        public DateTime CreatedTime { get; set; }
        public long Balance { get; set; }

        public long OrgBalance { get; set; }
        public int ServiceID { get; set; }

        public long GameSessionID { get; set; }
        public string ClientIP { get; set; }
        public string ServiceName { get {

                if (ServiceID == 1) return "B1";
                else if (ServiceID == 3) return "B3";
                return string.Empty;
            }}



        public String BetValueFormat { get
            {
                return BetValue.LongToMoneyFormat();
            }
        }
        public String BalanceFormat
        {
            get
            {
                return Balance.LongToMoneyFormat();
            }
        }
        public String OrgBalanceFormat
        {
            get
            {
                return OrgBalance.LongToMoneyFormat();
            }
        }
        public String AwardValueFormat
        {
            get
            {
                return AwardValue.LongToMoneyFormat();
            }
        }

    }
}