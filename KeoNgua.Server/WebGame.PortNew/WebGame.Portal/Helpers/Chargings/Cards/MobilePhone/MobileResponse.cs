using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MsWebGame.Portal.Helpers.Chargings.Cards.MobilePhone
{
    public class MobileResponse
    {
        public string status { get; set; }
        public string description { get; set; }
        public string transRef { get; set; }
        public string cardCode { get; set; }
        public string cardSerial { get; set; }
        public int amount { get; set; }
        public string Signature { get; set; }
    }
}