using System;
using System.ComponentModel;
using TraditionGame.Utilities.Utils;

namespace MsWebGame.CSKH.Models.Cards
{
    public class UserCardRechardModel
    {
        public long RequestID { get; set; }

        public long UserID { get; set; }

        public int TelOperatorID { get; set; }
        [DisplayName("Mã thẻ ")]
        public string CardNumber { get; set; }
        [DisplayName("Serial thẻ ")]
        public string SerialNumber { get; set; }
        [DisplayName("Giá trị thẻ ")]
        public int CardValue { get; set; }
        [DisplayName("Nhà mạng ")]
        public string OperatorName { get; set; }
        [DisplayName("Số tiền thực nhận ")]
        public long? ReceivedMoney { get; set; }
        [DisplayName("Loại thẻ  ")]
        public string CardName { get; set; }
        [DisplayName("Nickname nạp thẻ ")]
        public string GameAccountName { get; set; }
        [DisplayName("Mã yêu cầu ")]
        public string PartnerErrorCode { get; set; }
        [DisplayName("Mã phản hồi ")]
        public string FeedbackErrorCode { get; set; }
        public int Status { get; set; }
        [DisplayName("Mô tả ")]
        public string Description { get; set; }
        [DisplayName("Số tiền thực nhận ")]
        public string ReceivedMoneyFormmat
        {
            get { return ReceivedMoney.LongToMoneyFormat(); }
        }
        [DisplayName("Giá trị thẻ ")]
        public string CardValueFormat
        {
            get { return CardValue.IntToMoneyFormat(); }
        }

        [DisplayName("Ngày tạo ")]
        public DateTime CreateDate { get; set; }
        [DisplayName("Tin nhắn yêu cầu ")]
        public string PartnerMessage { get; set; }
        [DisplayName("Tin nhắn phản hồi ")]
        public string FeedbackMessage { get; set; }

        [DisplayName("Trạng thái ")]
        public string StatusStr
        {
            get
            {
                if (Status == 0)
                {
                    return "Chờ xử lý";
                }
                else if (Status == -1)
                {
                    return "Nạp thất bại";
                }
                else if (Status == -1)
                {
                    return "Chờ xử lý";
                }
                else if (Status == 2)
                {
                    return "Nạp thành công";
                }
                else
                {
                    return "Nạp thất bại";
                }
            }
        }
    }
}