using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MsWebGame.CSKH.Models.LuckySpin
{
    public class PresentSpinsModel
    {
        public int ID { get; set; }

        [DisplayName("Số lượt quay")]
        //[Required(ErrorMessageResourceType = typeof(Utils.Message), ErrorMessageResourceName = "LuckySpin_QuantityRequired")]
        [UIHint("Int")]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượt quay nhỏ nhất bằng {1}")]
        public int Quantity { get; set; } = 1;

        [DisplayName("Ngày bắt đầu")]
        [Required(ErrorMessageResourceType = typeof(Utils.Message), ErrorMessageResourceName = "LuckySpin_StartDateRequired")]
        public DateTime StartDate { get; set; }

        [DisplayName("Ngày kết thúc")]
        [Required(ErrorMessageResourceType = typeof(Utils.Message), ErrorMessageResourceName = "LuckySpin_EndDateRequired")]
        public DateTime EndDate { get; set; }

        [DisplayName("Ngày tạo")]
        public DateTime CreatedDate { get; set; }
    }
}