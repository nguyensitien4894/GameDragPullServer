using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TraditionGame.Utilities.SmartBanks.Model
{

    public class BankModel {
        public string BankName { get; set; }


        public string BankCode { get; set; }

    }
    public class SmartBankChargeModel
    {
        public SmartBankChargeModel()
        {
            Banks = new List<BankModel>();
        }


        public int ResponseCode { get; set; }
        public  List<BankModel> Banks { get; set; }






    }
}
