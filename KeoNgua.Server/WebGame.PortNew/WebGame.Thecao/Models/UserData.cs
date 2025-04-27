using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MsWebGame.Thecao.Models
{
    public class UserData
    {
        public int AccountID { get; set; }
        public string AccountName { get; set; }
        public string ClientIP { get; set; }
        public string LoginDate { get; set; }
        public string AccessToken { get; set; }
        public int ServiceID { get; set; }
    }
}