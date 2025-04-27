using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MsWebGame.CSKH.Database.DTO
{
    public class NruDeviceStatistics
    {
        public long ID { get; set; }

        public DateTime? CheckDate { get; set; }

        public long? NruWeb { get; set; }

        public long? NruIos { get; set; }

        public long? NruAndroid { get; set; }

        public long? NruWinPhone { get; set; }

        public int? ServiceID { get; set; }

        public DateTime? CreateDate { get; set; }
        public long? PuPlay { get; set; }
        public long? PuCardRecharge { get; set; }
        public long? PuBuyAgency { get; set; }
        public long ? PuGiftCode { get; set; }
    }
}