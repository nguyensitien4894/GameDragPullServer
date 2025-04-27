using System;
using TraditionGame.Utilities.Utils;
using MsWebGame.CSKH.Utils;

namespace MsWebGame.CSKH.Database.DTO
{
    public class Admin
    {
        public long AccountID { get; set; }
        public string UserName { get; set; }
        public string PhoneContact { get; set; }
        public string Email { get; set; }
        public int? Level { get; set; }
        public int Status { get; set; }
        public string RoleID { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string Password { get; set; }
        public long? Wallet { get; set; }
        public string RoleCode { get; set; }
        public int? IsFirstGoogleAuthen { get; set; }
        public string DisplayName { get; set; }
    }

    public class AdminRevenue
    {
        public long AccountID { get; set; }
        public string UserName { get; set; }
        public string DisplayName { get; set; }
        public long TotalOutUserAmount { get; set; }
        public long TotalOutAgencyAmount { get; set; }
        public long TotalOutAdminAmount { get; set; }
        public long TotalInUserAmount { get; set; }
        public long TotalInAgencyAmount { get; set; }
        public long TotalInAdminAmount { get; set; }
    }

    public class AdminTrans
    {
        public string UserName { get; set; }
        public long Wallet { get; set; }
        public string WalletFormat { get { return Wallet.LongToMoneyFormat(); } }
        public string TransCode { get; set; }
        public DateTime TransDate { get; set; }
        public long OrgAmount { get; set; }
        public string OrgAmountFormat { get { return OrgAmount.LongToMoneyFormat(); } }
        public double Fee { get; set; }
        public long Amount { get; set; }
        public string AmountFormat { get { return Amount.LongToMoneyFormat(); } }
        public string Note { get; set; }
        public string Description { get; set; }
        public string PartnerName { get; set; }
    }

    public class CallCenterInfo
    {
        public long AdminID { get; set; }
        public string UserName { get; set; }
        public string PhoneContact { get; set; }
        public string Email { get; set; }
        public int Level { get; set; }
        public int Status { get; set; }
        public string StatusFormat { get { return Status.ConvertStatus(); } }
        public string RoleID { get; set; }
    }
}