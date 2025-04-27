using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TraditionGame.Utilities.Utils;

namespace MsWebGame.CSKH.Database.DTO
{
    public class UserProfit
    {
        public long ID { get; set; }
        public long AccountID { get; set; }
        public string Username { get; set; }
        public long Profit { get; set; }
        public DateTime CreatedDate { get; set; }
        public int ServiceID { get; set; }
        public string ProfitFormat
        {
            get
            {
                return Profit.LongToMoneyFormat();
            }
        }
    }
}