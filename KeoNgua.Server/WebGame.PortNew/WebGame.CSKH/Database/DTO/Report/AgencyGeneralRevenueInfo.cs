using System;
using System.Collections.Generic;

namespace MsWebGame.CSKH.Database.DTO
{
    public class AgencyGeneralRevenueInfo
    {
        public List<RootAgencyReport> LstReport { get; set; }
        public long AgencyIDTotal { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int ServiceID { get; set; }
        public long Balance { get; set; }
    }
}