using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MsWebGame.CSKH.Database.DTO
{
    public class Card
    {

        public int ID { get; set; }

        public string CardCode { get; set; }

        public string CardName { get; set; }

        public int CardValue { get; set; }

        public double? CardRate { get; set; }

        public double? CardSwapRate { get; set; }

        public bool Status { get; set; }

        public long CreateUser { get; set; }

        public double ChargeRate { get; set; }

        public long UpdateUser { get; set; }

        public DateTime UpdateDate { get; set; }

        public int? TelecomOperatorsID { get; set; }
        public string OperatorName { get; set; }
        public long ? PartnerId { get; set; }
        public string PartnerName { get; set; }
        public bool ExchangeStatus { get; set; }
        public int ServiceID { get; set; }
        public string ServiceName { get; set; }
    }
}