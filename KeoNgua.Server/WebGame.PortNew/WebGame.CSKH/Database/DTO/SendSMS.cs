using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MsWebGame.CSKH.Database.DTO
{
    public class SendSMS
    {
        public long ID { get; set; }
        public string SmsContent { get; set; }
        public bool IsSendSms { get; set; }
        public string Phone { get; set; }
    }
}