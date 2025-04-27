using System;
using System.Collections.Generic;

namespace MsWebGame.CSKH.Database.DTO
{
    public class AgencyL1CashFlowUsersInfo
    {
        public List<ReportAgencyL1CashFlowUsers> LstResult { get; set; }
        public string NickName { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int ServiceID { get; set; }
    }
}