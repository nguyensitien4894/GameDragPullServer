using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MsWebGame.Portal.Database.DTO
{
    public class SmsCard
    {
        public int ID { get; set; }

        public int DisplayValue { get; set; }

        public int Value { get; set; }

        public bool Status { get; set; }

        public long CreateUser { get; set; }

        public DateTime CreateDate { get; set; }

        public long UpdateUser { get; set; }

        public DateTime UpdateDate { get; set; }

        public int PartnerID { get; set; }

        public int? ServiceID { get; set; }

        public double? Rate { get; set; }
    }
}