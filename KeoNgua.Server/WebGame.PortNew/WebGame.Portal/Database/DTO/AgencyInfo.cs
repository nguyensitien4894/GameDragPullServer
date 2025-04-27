using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MsWebGame.Portal.Database.DTO
{
    public class AgencyInfo
    {
        public long AccountId { get; set; }
        public short? AccountLevel { get; set; }
        public string AccountName { get; set; }
        public string DisplayName { get; set; }
        public string FullName { get; set; }
        public long? ParentID { get; set; }
        public int? Status { get; set; }
        public string PhoneOTP { get; set; }
        public string PhoneDisplay { get; set; }
        public string AreaName { get; set; }
        public string FBLink { get; set; }
        public long TelegramID { get; set; }
        public bool CheckOTP { get; set; }
        public int? OrderNum { get; set; }
        public string OldPassword { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public int Type { get; set; }
        public string Otp { get; set; }
    }
}