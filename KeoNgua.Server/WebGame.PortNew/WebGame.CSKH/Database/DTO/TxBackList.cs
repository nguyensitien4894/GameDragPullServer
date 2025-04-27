using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TraditionGame.Utilities.Utils;

namespace MsWebGame.CSKH.Database.DTO
{
    public class TxBackList
    {
        public int ID { get; set; }
        public long AccountID { get; set; }
        public string DisplayName { get; set; }
        public int ServiceID { get; set; }
        public long MinBet { get; set; }
        public bool Active { get; set; }

        public DateTime CreatedDate { get; set; }

        public string MinBetFormat
        {
            get
            {
                return MinBet.LongToMoneyFormat();
            }
        }
        public string ServiceName
        {
            get
            {
                if (ServiceID == 1) return "B1";
                if (ServiceID == 3) return "B3";
                return string.Empty;
            }
        }



    }
}