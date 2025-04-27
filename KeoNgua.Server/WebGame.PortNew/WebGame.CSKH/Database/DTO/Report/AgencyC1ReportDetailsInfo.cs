using System;
using System.Collections.Generic;

namespace MsWebGame.CSKH.Database.DTO
{
    public class AgencyC1ReportDetailsInfo
    {
        public List<ReportAgencyGeneralTrans> LstResult { get; set; }
        public string PartnerName { get; set; }
        public int TransType { get; set; }
        public DateTime FromDate { get; set; }
        public int ServiceID { get; set; }
    }
}