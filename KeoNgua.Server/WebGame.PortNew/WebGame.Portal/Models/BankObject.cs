using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MsWebGame.Portal.Models
{
    public class BankObject
    {
        public BankObject()
        {
            BankItems = new List<BankInfo>();
        }
        public string OperatorName { get; set; }
        public int ServiceID { get; set; }
        public int BankOperatorsSecondaryID { get; set; }
        public List<BankInfo> BankItems { get; set; }
    }

    public class BankInfo
    {
        public string BankName { get; set; }
        public string BankNumber { get; set; }
    }
}