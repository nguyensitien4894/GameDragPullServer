using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MsWebGame.Portal.Database.DTO
{
    public class UserAgency
    {
        public long AccountID { get; set; }
        public string AccountName { get; set; }
        public int AccountType { get; set; }
        public int AccountLevel { get; set; }
        public int  AccountStatus { get; set; }
        public string NickName { get; set; }
        public long? TelegramID { get; set; }
        public bool IsBot { get; set; }
        public long OTPSafeID { get; set; }
        public string PhoneSafeNo { get; set; }
        public string SignalID { get; set; }


    }
}