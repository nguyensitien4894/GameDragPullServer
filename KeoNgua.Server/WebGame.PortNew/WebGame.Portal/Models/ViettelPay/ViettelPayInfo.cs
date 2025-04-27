using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MsWebGame.Portal.Models.ViettelPay
{
    
    public class Info
    {
        public string phone { get; set; }
        public string name { get; set; }
    }

    public class ViettelPayInfo
    {
        public int status { get; set; }
        public string message { get; set; }
        public Info info { get; set; }
    }
}