using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MsWebGame.Portal.Database.DTO
{
    public class Users
    {

        public long UserID { get; set; }

        public string UserName { get; set; }

        public string UserDisplayName { get; set; }

        public string Avatar { get; set; }

        public string PhoneOTP { get; set; }

        public string PhoneContact { get; set; }

        public string BankAccount { get; set; }

        public string Email { get; set; }
       
    

       

        public int? Status { get; set; }

       

        public int? LockWithDraw { get; set; }

        public int? LockTopup { get; set; }

        public string Password { get; set; }

        public long? Wallet { get; set; }

        public long? WalletHang { get; set; }

        public int UserType { get; set; }

        public int AgencyLevel { get; set; }
        public long AgencyID { get; set; }
        public long WalletStar { get; set; }
        public int ChangePassword
        {
            get; set;
        }
    }

}