using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TraditionGame.Utilities.SmartBanks.Model
{
   public  class SmartBankChargeRequestModel
    {
        public string BankCode { get; set; }
        public string CallBackUrl { get; set; }
        public long Amount { get; set; }
      

    }
}
