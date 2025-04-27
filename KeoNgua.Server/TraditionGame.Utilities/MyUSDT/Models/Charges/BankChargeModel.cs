using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TraditionGame.Utilities.MyUSDT.Models.Charges
{
    public  class BankCharge
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("enable")]
        public bool Enable { get; set; }

        [JsonProperty("bank_name")]
        public string BankName { get; set; }

        [JsonProperty("account_number")]
        public string AccountNumber { get; set; }

        [JsonProperty("account_name")]
        public string AccountName { get; set; }

        [JsonProperty("note")]
        public BankNote Note { get; set; }
    }
    public  class BankNote
    {
    }
   
}