using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MsWebGame.CSKH.Models.Mails
{
    public class MailListModel
    {
        [DisplayName("Người nhận")]
        public string UserName { get; set; }
        public int?Status { get; set; }
        [UIHint("DateTimeNullable")]
        [DisplayName("Từ ngày")]
        public DateTime? FromDate { get; set; }
        [UIHint("DateTimeNullable")]
        [DisplayName("Tới ngày")]
        public DateTime? ToDate { get; set; }
        public int ServiceID { get; set; }
    }
}