using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MsWebGame.CSKH.Database.DTO
{
    public class TelecomOperators
    {
        public int ID { get; set; }

        public string OperatorCode { get; set; }

        public string OperatorName { get; set; }

        public double Rate { get; set; }

        public double ExchangeRate { get; set; }

        public bool Status { get; set; }

        public long CreateUser { get; set; }

        public DateTime CreateDate { get; set; }

        public long UpdateUser { get; set; }

        public DateTime UpdateDate { get; set; }

        public bool ExchangeStatus { get; set; }

        public bool? ActiveByNPP { get; set; }
        public int ServiceID { get; set; }
    }
}