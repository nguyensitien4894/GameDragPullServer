using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MsWebGame.Portal.Helpers.Chargings.MobileSMS
{
    public class MobileRequestResponse
    {
        public int errorid { get; set; }
        public string errordes { get; set; }
        public string transactionId { get; set; }
    }
}