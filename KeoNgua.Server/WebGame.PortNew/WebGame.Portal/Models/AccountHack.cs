using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MsWebGame.Portal.Models
{
    public class AccountHack
    {
        public long ID { get; set; }
        public string AccountBankName { get; set; }
        public string AccountBankNumber { get; set; }
        public string BankName { get; set; }
        public bool? Status { get; set; }
        public long AccountID { get; set; }
        public string UserName { get; set; }
        public string Reason { get; set; }
        public long CreateBy { get; set; }
        public long UpdateBy { get; set; }
    }
}