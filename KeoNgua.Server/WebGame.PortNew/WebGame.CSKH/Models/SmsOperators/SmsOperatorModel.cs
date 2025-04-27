using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MsWebGame.CSKH.Models.SmsOperators
{
    public class SmsOperatorModel
    {
        public int ID { get; set; }

        public string OperatorCode { get; set; }

        public string OperatorName { get; set; }

        public bool Status { get; set; }

        public long CreateUser { get; set; }

        public DateTime CreateDate { get; set; }

        public long UpdateUser { get; set; }

        public DateTime UpdateDate { get; set; }

        public string Sysntax { get; set; }

        public int ServiceID { get; set; }

        public string Telecom { get; set; }

        public int PartnerID { get; set; }
    }
}