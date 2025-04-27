using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MsWebGame.Portal.Database.DTO
{
    public class Bank
    {
        public int ID { get; set; }
        public string BankNumber { get; set; }
        public string BankName { get; set; }
        public bool Status { get; set; }
        public string OperatorName { get; set; }
        public string ShortOperatorCode { get; set; }
        public int ServiceID { get; set; }
        public int BankOperatorsSecondaryID { get; set; }
    }
}