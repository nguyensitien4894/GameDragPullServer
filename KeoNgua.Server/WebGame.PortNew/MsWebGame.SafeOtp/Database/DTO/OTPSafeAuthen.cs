using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MsWebGame.SafeOtp.Database.DTO
{
    public class OTPSafeAuthen
    {
       public long ID { get; set; }

        public string PhoneOTP { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public int Status { get; set; }

        public string Token { get; set; }

        public string SignalID { get; set; }

        public DateTime? CreatedTime { get; set; }
    }
}