using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MsWebGame.Portal.Database.DTO
{
    public class VipCardRedemption
    {
        public long ID { get; set; }

        public long UserID { get; set; }

        public int RankID { get; set; }

        public int VP { get; set; }

        public int CardBonusNo { get; set; }

        public int CardLimit { get; set; }

        public double CardRate { get; set; }

        public int? CardRemain { get; set; }

        public DateTime? CreateDate { get; set; }

    }
}