using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MsWebGame.CSKH.Models.Accounts
{
    public class AccountPopupModel
    {
        public long UserID { get; set; }
        public string Username { get; set; }
        public string GameAccountName { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime? PhoneUpdateDate { get; set; }
        public long Balance { get; set; }
        public long SafeBalance { get; set; }
        public long TotalBalance { get; set; }
        public long TotalRecharge { get; set; }
        public int ServiceID { get; set; }
    }
}