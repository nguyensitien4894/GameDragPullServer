using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MsWebGame.Thecao.Models.SmartBanks
{
    public class SmartBankCallBackModel
    {


        public string RequestCode { get; set; }
        public long Amount { get; set; }
        public string CallBackUrl { get; set; }

        public string Refkey { get; set; }
        public int? Status { get; set; }
        public string StatusStr { get; set; }
    }
}