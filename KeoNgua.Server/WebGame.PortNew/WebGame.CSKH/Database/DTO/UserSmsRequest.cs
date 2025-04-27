using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MsWebGame.CSKH.Database.DTO
{
    public class UserSmsRequest
    {
        public string DisplayName { get; set; }
        public long RequestID { get; set; }

        public long UserID { get; set; }

        public string Phone { get; set; }

        public long Amount { get; set; }

        public double? Rate { get; set; }

        public long? ReceivedMoney { get; set; }

        public int Status { get; set; }

        public string PartnerErrorCode { get; set; }

        public string PartnerMessage { get; set; }

        public string Signature { get; set; }

        public string Description { get; set; }

        public string RefKey { get; set; }

        public int? PartnerID { get; set; }

        public int? ServiceID { get; set; }

        public DateTime RequestDate { get; set; }

        public long UpdateUser { get; set; }

        public DateTime UpdateDate { get; set; }
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
                    if (value == -2) return "Thất bại";
                }
                return string.Empty;
            }
        }
        public string PartnerName
        {
            get
            {
                if (PartnerID.HasValue)
                {
                    int value = PartnerID.Value;
                    if (value == 1) return "SHOPTHENHANH";


                }
                return string.Empty;
            }
        }

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
    }
}