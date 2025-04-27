using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MsWebGame.Portal.Database.DTO
{
    public class BankOperatorsSecondary
    {
        public int ID { get; set; }
        public string ShortOperatorCode { get; set; }
        public string OperatorCode { get; set; }
        public string OperatorName { get; set; }
        public double Rate { get; set; }
        public bool Status { get; set; }
        public long CreateUser { get; set; }
        public DateTime CreateDate { get; set; }
        public int ServiceID { get; set; }
    }
}