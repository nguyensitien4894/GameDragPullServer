using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MsWebGame.Thecao.Helpers.Chargings.MobileSMS
{
    public class MobileSmsCallBackRequest
    {
        public string transId { get; set; }
        public int errorid { get; set; }
        public string errordesc { get; set; }
        public int  amount { get; set; }
        public string signature { get; set; }
    }
}