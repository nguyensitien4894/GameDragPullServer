using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MsWebGame.Portal.Database.DTO
{
    public class UserVip
    {
      
        public long RankID { get; set; }
        public long VipPoint { get; set; }
        public long RefundAmount { get; set; }
        public int RedeemStatus { get; set; }
        public string  RankName
        {
            get
            {
                var acceptList = new List<long> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
                if (acceptList.Contains(RankID))
                {
                    return String.Format("Vip {0}", RankID);
                }
                else
                    return "Vip 1";
            }
           

        }
    }
       
}