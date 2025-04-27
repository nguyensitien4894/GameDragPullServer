using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MsWebGame.Thecao.Models.SmsCharges
{

    public class SmsChargeModel
    {
        public string msisdn { get; set; }
        public string requestId { get; set; }
        public int amount { get; set; }
        public string info { get; set; }
        public string signature { get; set; }
    }
}