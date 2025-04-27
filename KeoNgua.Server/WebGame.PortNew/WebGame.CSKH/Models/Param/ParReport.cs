using System;

namespace MsWebGame.CSKH.Models.Param
{
    public class ParReport
    {
        public long AgencyIDTotal { get; set; }
        public string NickName { get; set; }
        public string PartnerName { get; set; }
        public int PartnerType { get; set; }
        public int TransType { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int ServiceID { get; set; }
    }
}