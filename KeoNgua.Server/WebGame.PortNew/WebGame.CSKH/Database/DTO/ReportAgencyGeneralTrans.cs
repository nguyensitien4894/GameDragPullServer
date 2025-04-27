using System;
using TraditionGame.Utilities.Utils;

namespace MsWebGame.CSKH.Database.DTO
{
    public class ReportAgencyGeneralTrans
    {

        public long AccountID { get; set; }
        public string AccountName { get; set; }
        public string DisplayName { get; set; }
        public string FullName { get; set; }
        public string PhoneNo { get; set; }
        public int TransferType { get; set; }
        public long TransID { get; set; }
        public string TransCode { get; set; }
        public int  TransType { get; set; }
        public int ReasonID { get; set; }
        public  long OrgAmount { get; set; }
        public double Fee { get; set; }
        public  long Amount { get; set; }
        public string AmountFormat { get { return MoneyExtension.LongToMoneyFormat(Amount); } }
        public int Status { get; set; }
        public DateTime TransDate { get; set; }
        public string Note { get; set; }
        public string Description { get; set; }

    }
}