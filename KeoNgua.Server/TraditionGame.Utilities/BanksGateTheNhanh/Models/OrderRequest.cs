using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TraditionGame.Utilities.BanksGateTheNhanh
{
   public  class OrderRequest
    {
        public string Type { get; set; }
        public string AccountName { get; set; }
        public string BankName { get; set; }
        public string BankAccountName { get; set; }
        public string BankAccountNumber { get; set; }
        public string AppCode { get; set; }
        public string RefCode { get; set; }
        public int Amount { get; set; }
        public string CallbackUrl { get; set; }

    }
}
