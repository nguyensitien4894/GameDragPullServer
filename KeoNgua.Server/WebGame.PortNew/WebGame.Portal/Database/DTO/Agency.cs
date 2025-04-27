using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MsWebGame.Portal.Database.DTO
{
    public class Agency
    {
        public long AccountId { get; set; }

        //public string AccountCode { get; set; }

        //public string AccountName { get; set; }

        public string DisplayName { get; set; }

        public short? AccountLevel { get; set; }

        //public long? ParentID { get; set; }

        //public long? UserMarketID { get; set; }

        //public int? Status { get; set; }

        //public int? AreaID { get; set; }

        //public int? BankID { get; set; }

        //public string SecretKey { get; set; }

        public string PhoneNo { get; set; }

        public string FBLink { get; set; }

        public string BankAccountNumber { get; set; }

        public string BankAccountName { get; set; }
        public string PhoneDisplay { get; set; }

        public string BankBranch { get; set; }
        public string FullName { get; set; }
        public string AreaName { get; set; }
        //public string ParrentAccountName { get; internal set; }
        public string Email { get; set; }
        public string ZaloLink { get; set; }
        public string  TelegramLink { get; set; }
        public int Index { get; set; }
    }
}