using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace MsWebGame.CSKH.Models.SMSOTPS
{
    public class SmsOtpModel
    {
        [DisplayName("Mã yêu cầu")]
        public long RequestId { get; set; }

        public string RequestTime { get; set; }

        public string Type { get; set; }
        [DisplayName("Số điện thoại")]
        public string Msisdn { get; set; }
        [DisplayName("Trạng thái Otp")]
        public int? Active { get; set; }
        [DisplayName("Người tạo")]
        public string DisplayName { get; set; }

        public int LoginAppSafeStatus { get; set; }
        

        public DateTime? ExpiredAt { get; set; }

        public long? UserID { get; set; }
        [DisplayName("Trạng thái gửi SMS")]
        public int? Status { get; set; }
        [DisplayName("Ngày tạo")]
        public DateTime? CreateDate { get; set; }

        public DateTime? UpdateDate { get; set; }

        public long? CreateBy { get; set; }

        public long? UpdateBy { get; set; }
        [DisplayName("Phí Otp")]
        public long? Amount { get; set; }
        public int? ServiceID { get; set; }
        public string OtyType
        {
            get
            {

                var smsOtp = new List<string> { "1", "2" };
                if (smsOtp.Contains(Type)){
                    return "SMS";

                }
                var AppSafes = new List<string> {  "10","20" };
                if (AppSafes.Contains(Type)){
                    if (LoginAppSafeStatus == 1)
                    {
                        return "SMS";
                    }
                    return "AppSafe";

                }
                return string.Empty;
            }

        }
    }

}