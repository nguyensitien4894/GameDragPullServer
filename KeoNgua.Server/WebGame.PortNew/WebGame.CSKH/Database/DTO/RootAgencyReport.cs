using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MsWebGame.CSKH.Database.DTO
{
    public class RootAgencyReport
    {
        public  long AccountID { get; set; }
        public string AccountName { get; set; }
        public string DisplayName { get; set; }
        public string FullName { get; set; }
        public DateTime TransDay { get; set; }
        public  long TotalOutQuantity { get; set; }
        public long TotalOutValue { get; set; }
        public long TotalInQuantity { get; set; }
        public  long TotalInValue { get; set; }
    }
}