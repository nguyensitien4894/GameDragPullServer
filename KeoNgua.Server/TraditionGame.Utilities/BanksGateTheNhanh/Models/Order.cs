using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TraditionGame.Utilities.BanksGateTheNhanh
{
    public class Order
    {
        public string Status { get; set; }
        public string BankName { get; set; }
        public string BankAccountNumber { get; set; }
        public string BankAccountName { get; set; }
        public int Amount { get; set; }
        public int RefCode { get; set; }
        public string OrderNo { get; set; }
        public int Timeout { get; set; }

    }
}
