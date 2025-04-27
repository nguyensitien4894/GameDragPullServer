using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace MsWebGame.CSKH.Models.SMSOTPS
{
    public class SmsOtpListModel
    {
  

        public string Type { get; set; }
        [DisplayName("Số điện thoại")]
        public string Msisdn { get; set; }
        [DisplayName("Từ ngày ")]
        public DateTime  FromDate { get; set; }
        [DisplayName("Tới ngày")]
        public DateTime ToDate { get; set; }
        public int ServiceID { get; set; }
        [DisplayName("Tên hiển thị")]
        public string NickName { get; set; }
    }
}