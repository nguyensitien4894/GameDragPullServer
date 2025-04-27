using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MsWebGame.CSKH.Database.DTO
{
    public class UserMomoRequest
    {

        public long RequestID { get; set; }

        public int RequestType { get; set; }

        public string RequestCode { get; set; }

        public long UserID { get; set; }

        public long Amount { get; set; }

        public double? Rate { get; set; }

        public long? ReceivedMoney { get; set; }

        public long? RefundReceivedMoney { get; set; }

        public int Status { get; set; }

        public string PartnerStatus { get; set; }

        public string PartnerErrorCode { get; set; }

        public string PartnerMessage { get; set; }

        public string FeedbackErrorCode { get; set; }

        public string FeedbackMessage { get; set; }

        public string Description { get; set; }

        public string RefKey { get; set; }

        public string RefSendKey { get; set; }

        public double? Fee { get; set; }

        public int? PartnerID { get; set; }

        public string GameCode { get; set; }

        public string GameAccount { get; set; }

        public string NccCode { get; set; }

        public string Provider { get; set; }

        public string MomoAccount { get; set; }

        public string MomoNumber { get; set; }

        public int? ServiceID { get; set; }


        public DateTime RequestDate { get; set; }

        public string ColorStr
        {
            get
            {
                if (Status == 0)
                {
                    return "label label-warning";
                }
                else if (Status == -1)
                {
                    return "label label-grey";
                }
                else if (Status == -2)
                {
                    return "label label-grey";
                }
                else if (Status == 1)
                {
                    return "label label-success";
                }

                else
                {
                    return "label label-grey";
                }
            }
        }
        public long UpdateUser { get; set; }
        public string PartnerName
        {
            get
            {
                if (PartnerID.HasValue)
                {
                    int value = PartnerID.Value;
                    if (value == 1) return "HAPPY";
                    if (value == 2) return "SHOPTHENHANH";
                   
                }
                return string.Empty;
            }
        }
        public DateTime UpdateDate { get; set; }
        public string ServieName
        {
            get
            {
                if (ServiceID.HasValue)
                {
                    int value = ServiceID.Value;
                    if (value == 1) return "B1";
                    if (value == 2) return "B2";
                    if (value == 3) return "B3";
                }
                return string.Empty;
            }
        }
        public string DisplayName { get; set; }
        public string StatusStr
        {
            get
            {
                if (PartnerID.HasValue)
                {
                    int value = Status;
                    if (value == 0) return "Chờ xử lý";
                    if (value == 1) return "Thành công";
                    if (value == -1) return "Chờ xử lý";
                    if (value == -2) return "Admin hủy duyệt";
                }
                return string.Empty;
            }
        }

        public string ReceiverName { get; set; }   
    }
}