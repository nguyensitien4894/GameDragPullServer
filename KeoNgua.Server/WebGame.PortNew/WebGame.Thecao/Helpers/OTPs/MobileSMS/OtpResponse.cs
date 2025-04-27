using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MsWebGame.Thecao.Helpers.OTPs.MobileSMS
{
    public class OtpResponse
    {
        public string code { get; set; }
        public string des { get; set; }
    }
    public class OtpSuccess
    {
        public static string SUCESS = "100";
    }
}