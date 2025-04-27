using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MsWebGame.Portal.Database.DTO
{
    public class Cards
    {
        public int ID { get; set; }

        public string CardCode { get; set; }

        public string CardName { get; set; }

        public int CardValue { get; set; }

        public double CardRate { get; set; }

        public bool Status { get; set; }

        public long CreateUser { get; set; }

        public DateTime CreateDate { get; set; }

        public long UpdateUser { get; set; }

        public DateTime UpdateDate { get; set; }

        public long? CardTypeId { get; set; }
        public long? PartnerId { get; set; }
        public string OperatorCode { get; set; }
        //tỉ giá đổi thẻ
        public double ExchangeRate { get; set; }
        //tỉ giá nạp thẻ
        public double ChargeRate { get; set; }

        public bool ExchangeStatus { get; set; }
    }
}