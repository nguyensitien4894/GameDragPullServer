using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MsWebGame.CSKH.Models.SystemMails
{
    public class SystemMailModel
    {
        public SystemMailModel()
        {
            listStatus = new List<SelectListItem>();
        }
        public long ID { get; set; }

        [Required(ErrorMessageResourceType = typeof(Utils.Message), ErrorMessageResourceName = "TitleRequired")]
        [DisplayName("Tiêu đề")]
        public string Title { get; set; }
        [Required(ErrorMessageResourceType = typeof(Utils.Message), ErrorMessageResourceName = "ContentRequired")]
        [DisplayName("Nội dung")]
        public string Content { get; set; }
        [DisplayName("Ngày tạo")]
        public DateTime? CreateTime { get; set; }
        [Required(ErrorMessageResourceType = typeof(Utils.Message), ErrorMessageResourceName = "StatusRequired")]
        [DisplayName("Trạng thái")]
        public bool? Status { get; set; }
        public List<SelectListItem> listStatus { get; set; }

        [DisplayName("Trạng thái")]
        public string StatusStr { get {
                if (Status.HasValue && Status.Value)
                    return "Hoạt động";
                else
                    return "Tạm dừng";
            } }
    }
}