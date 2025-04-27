using System;

namespace MsWebGame.CSKH.Models.Agencies
{
    public class TransactionModel
    {
        public string NickName { get; set; }
        public int UserType { get; set; }
        public string PartnerName { get; set; }
        public int Type { get; set; }
        public int Status { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int ServiceID { get; set; }
    }
}