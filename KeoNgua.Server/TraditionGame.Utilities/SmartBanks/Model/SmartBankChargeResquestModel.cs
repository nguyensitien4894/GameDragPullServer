using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TraditionGame.Utilities.SmartBanks.Model
{
    public class SmartBankChargeResquestModel
    {

        public int ResponseCode { get; set; }

        public string RequestCode { get; set; }
        public string Bankname { get; set; }
        public string BankAccount { get; set; }
        public string BankNumber { get; set; }
        public string Message { get; set; }
        public  int HttpStatusCode { get; set; }
        public string HttpMsg { get; set; }
        public int Amount { get; set; }
    }
}
