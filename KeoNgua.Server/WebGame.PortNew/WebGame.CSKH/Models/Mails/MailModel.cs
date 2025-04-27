using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace MsWebGame.CSKH.Models.Mails
{
    public class MailModel
    {
        public long ID { get; set; }
        [DisplayName("Người gửi")]
        public long Sender { get; set; }
        public long Reciever { get; set; }

        [Required(ErrorMessageResourceType = typeof(Utils.Message), ErrorMessageResourceName = "TitleRequired")]
        [DisplayName("Tiêu đề")]
        public string Title { get; set; }

        [Required(ErrorMessageResourceType = typeof(Utils.Message), ErrorMessageResourceName = "ContentRequired")]
        [DisplayName("Nội dung")]
        [AllowHtml]
        public string Content { get; set; }

        [DisplayName("Trạng thái")]
        public int? Status { get; set; }

        [DisplayName("Ngày tạo")]
        public DateTime? CreatedTime { get; set; }

        [DisplayName("Tài khoản nhận")]
        public string RecevierUserName { get; set; }

        [Required(ErrorMessageResourceType = typeof(Utils.Message), ErrorMessageResourceName = "UserRequired")]
        [DisplayName("Nickname nhận")]
        public string RecevierNickname { get; set; }

        [DisplayName("Chọn cổng")]
        public int ServiceID { get; set; }

        public List<SelectListItem> listStatus { get; set; }
    }
}