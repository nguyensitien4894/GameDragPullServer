using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MsWebGame.Thecao.Database.DTO
{
    public class UserVip
    {
      
        public long RankID { get; set; }
        public long VipPoint { get; set; }
        public long RefundAmount { get; set; }
        public int RedeemStatus { get; set; }
    }
       
}