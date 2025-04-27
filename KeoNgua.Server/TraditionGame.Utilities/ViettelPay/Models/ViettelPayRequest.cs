using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TraditionGame.Utilities.ViettelPay.Models
{
    public class ViettelPayRequest
    {
        public long money { get; set; }
        public string phone { get; set; }
        public string message { get; set; }      
        public string transaction_id { get; set; }
        public string signature { get; set; }
    }
}
