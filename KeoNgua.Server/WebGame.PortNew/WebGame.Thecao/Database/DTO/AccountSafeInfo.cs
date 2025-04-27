using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MsWebGame.Thecao.Database.DTO
{
    public class AccountSafeInfo
    {
        public int responseStatus { get; set; }
        public long SafeBalance { get; set; }
        public  long Balance { get; set; }
        public long SessionID { get; set; }
    }
}