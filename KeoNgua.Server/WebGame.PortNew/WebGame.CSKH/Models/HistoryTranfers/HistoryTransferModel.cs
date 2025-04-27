using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using TraditionGame.Utilities.Utils;
using MsWebGame.CSKH.Utils;

namespace MsWebGame.CSKH.Models.HistoryTranfers
{
    public class HistoryTransferModel
    {
        [DisplayName("Nội dung ")]
        public string Description { get; set; }
        public string Note { get; set; }
        public long CreateUserID { get; set; }
        [DisplayName("Nickname chuyển ")]
        public string CreateUserName { get; set; }
        [DisplayName("Nickname nhận ")]
        public string ReceiverName { get; set; }
        [DisplayName("Nickname chuyển ")]
        public string CreateDisplayName { get; set; }
        [DisplayName("Nickname nhận ")]
        public string ReceiverDisplayName { get; set; }
        [DisplayName("Tổng chi phí")]
        public long OrgAmount { get; set; }
        [DisplayName("Tổng chi phí")]
        public string OrgAmountFormat
        {
            get { return OrgAmount.LongToMoneyFormatNew(); }
        }

        [DisplayName("Số dư")]
        public long OrgBalance { get; set; }
        [UIHint("Long")]
        public long Amount { get; set; }
        [DisplayName("Số tiền chuyển ")]
        public string AmoutFormat
        {
            get { return Amount.LongToMoneyFormatNew(); }
        }

        [DisplayName("Ngày thực hiện giao dịch")]
        public DateTime TransDate { get; set; }
        public string TranStatus { get; set; }
        [DisplayName("Trạng thái")]
        public string TranStatusFormat
        {
            get { return TranStatus.StrToTransStatusFormat(); }
        }
        public int ReceiverType { get; set; }
        [DisplayName("Người dùng")]
        public string ReceiverTypeStr
        {
            get { return ReceiverType.StrToAccountStatusFormat(); }
        }
    }
}