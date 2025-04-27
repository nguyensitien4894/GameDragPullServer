using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TraditionGame.Utilities.Utils;

namespace MsWebGame.CSKH.Database.DTO
{
    public class TransactionHistory
    {
        public long UserID { get; set; }
        public string Username { get; set; }
        public string DisplayName { get; set; }

        public long OrgBalance { get; set; }
        public long Amount { get; set; }
        public long AmountFee { get; set; }
        public long RemainBalance { get; set; }
        public long RemainBalanceSafe { get; set; }
        public long PartnerID { get; set; }
        public int PartnerType { get; set; }
        public string PartnerName { get; set; }
        public string PartnerNameFormat
        {
            get
            {
                if (PartnerType == 2)
                {
                    return "<p style=\"color:red \">PartnerName</p>";
                }
                else
                {
                    return "<p style=\"color:red \">PartnerName</p>";
                }
            }
        }
        public DateTime TransDate { get; set; }
        public string TransCode { get; set; }
        public string Description { get; set; }
        public string Note { get; set; }


        public string OrgBalanceFormat
        {
            get
            {
                return OrgBalance.LongToMoneyFormat();
            }
        }
        public string AmountFormat
        {
            get
            {
                return Amount.LongToMoneyFormat();
            }
        }
        public string AmountFeeFormat
        {
            get
            {
                return AmountFee.LongToMoneyFormat();
            }
        }
        public string RemainBalanceFormat
        {
            get
            {
                return RemainBalance.LongToMoneyFormat();
            }
        }

    }
}