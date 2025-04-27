using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MsWebGame.Portal.Helpers.Chargings.MobileSMS
{
    public class MobileRequest
    {
        public string username { get; set; }
        public string password { get; set; }
        public string signature { get; set; }
        public string transId { get; set; }
        public string telco { get; set; }
        public string serial { get; set; }
        public string pin { get; set; }
        public string amount { get; set; }
    }
}