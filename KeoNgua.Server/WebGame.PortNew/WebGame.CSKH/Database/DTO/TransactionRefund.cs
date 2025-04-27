using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TraditionGame.Utilities.Utils;

namespace MsWebGame.CSKH.Database.DTO
{
    public class TransactionRefund
    {
        public long TransID { get; set; }

        public string TransCode { get; set; }

        public int TransType { get; set; }

        public long OrgAmount { get; set; }

        public double fee { get; set; }

        public long Amount { get; set; }

        public int Status { get; set; }

        public DateTime TransDate { get; set; }

        public int ReasonID { get; set; }

        public string Note { get; set; }

        public long CreateUserID { get; set; }

        public int? CreateUserType { get; set; }

        public long? ReceiverID { get; set; }

        public int? ReceiverType { get; set; }

        public long? ApproverID { get; set; }

        public DateTime? ApproverDate { get; set; }

        public long? CancellerID { get; set; }

        public DateTime? CancelledDate { get; set; }

        public long? FromTransID { get; set; }

        public string Description { get; set; }
        public string CreateDisplayName { get; set; }
        public long RemainBalanceCreateUser { get; set; }
        public string ReceiverDisplayName { get; set; }
        public long RemainBalanceReceiveUser { get; set; }
        public int ServiceID { get; set; }
        public long RetrievalAmount { get; set; }
        public string RetrievalAmountFormat
        {
            get
            {
                return RetrievalAmount.LongToMoneyFormat();
            }
        }
        public string RemainBalanceCreateUserFormat
        {
            get
            {
                return RemainBalanceCreateUser.LongToMoneyFormat();
            }
        }
        public string AmountFormat
        {
            get
            {
                return Amount.LongToMoneyFormat();
            }
        }
        public string RemainBalanceReceiveUserFormat
        {
            get
            {
                return RemainBalanceReceiveUser.LongToMoneyFormat();
            }
        }

    }
}