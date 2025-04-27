using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MsWebGame.Portal.Models.Momo
{
    public class Momo
    {
        public long Amount { get; set; }
        public string Captcha { get; set; }
        public string PrivateKey { get; set; }
        //public string Receive_BankCode { get; set; }
        //public string BankName { get; set; }
        //public string Receive_BankNumber { get; set; }
        //public string Receive_BankHolderName { get; set; }
        public string NoiDung { get; set; }
        public string Receive_MomoPhoneNumber { get; set; }
        public string Receive_MomoHolderName { get; set; }
        public int OperatorID { get; set; }
    }
}