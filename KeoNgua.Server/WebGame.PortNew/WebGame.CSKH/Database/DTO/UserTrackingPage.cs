using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MsWebGame.CSKH.Database.DTO
{
    public class UserTrackingPage
    {
        public string UrlPage { get; set; }
        public int LoginType { get; set; }
        public int CountRegister { get; set; }
        public string UrlPath { get; set; }
        public string UtmMedium { get; set; }
        public  string UtmSource { get; set; }
        public string UtmCampaign { get; set; }
        public string UtmContent { get; set; }
     
        public string LoginTypeStr
        {
            get
            {
                if (LoginType == 1) return "Đăng ký thường";
                else if (LoginType == 2) return "Đăng ký facebook";
                else return "Không xác định";
            }
        }
    }
}