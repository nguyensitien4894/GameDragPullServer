using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MsWebGame.Portal.Models
{
    public class SmsChargeConfigModel
    {
        public SmsChargeConfigModel()
        {
            cards = new List<SmsCardModel>();
        }

        public string Telecom { get; set; }
        public List<SmsCardModel> cards { get; set; }
    }
}