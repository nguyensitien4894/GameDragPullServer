using DataAnnotationsExtensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MsWebGame.CSKH.Models.Telecoms
{
    public class TelecomOperatorModel
    {
        public TelecomOperatorModel()
        {
            listStatus = new List<SelectListItem>();
        }
        public int ID { get; set; }
        [Required(ErrorMessageResourceType = typeof(Utils.Message), ErrorMessageResourceName = "OperatorCodeRequired")]
        [DisplayName("Mã nhà mạng")]
        public string OperatorCode { get; set; }
        
        [Required(ErrorMessageResourceType = typeof(Utils.Message), ErrorMessageResourceName = "OperatorNameRequired")]
        [DisplayName("Tên nhà mạng")]
        public string OperatorName { get; set; }
        [Required(ErrorMessageResourceType = typeof(Utils.Message), ErrorMessageResourceName = "OperatorRateRequired")]
        [Min(0.01, ErrorMessageResourceType = typeof(Utils.Message), ErrorMessageResourceName = "OperatorRateMin")]
        [DisplayFormat(DataFormatString = "{0:#,##0.0#}", ApplyFormatInEditMode = true)]
        [DisplayName("Tỉ lệ nạp thẻ")]
        
        
        public double? Rate { get; set; }
        [Required(ErrorMessageResourceType = typeof(Utils.Message), ErrorMessageResourceName = "OperatorExChangeRateRequired")]
        [DisplayName("Tỉ lệ đổi thưởng")]
        [Min(0.01, ErrorMessageResourceType = typeof(Utils.Message), ErrorMessageResourceName = "ExchangeRateMin")]
        public double? ExchangeRate { get; set; }
        [Required(ErrorMessageResourceType = typeof(Utils.Message), ErrorMessageResourceName = "OperatorStatusRequired")]

        [DisplayName("Trạng thái nạp thẻ ")]
        public bool Status { get; set; }
        [DisplayName("Trạng thái đổi thẻ  ")]
        public bool ExchangeStatus { get; set; }

        public long CreateUser { get; set; }

        public DateTime CreateDate { get; set; }

        public long UpdateUser { get; set; }

        public DateTime UpdateDate { get; set; }
        [DisplayName("Được kích hoạt bởi bên thứ 3")]
        public bool? ActiveByNPP { get; set; }

        public List<SelectListItem>listStatus { get; set; }
        public int serviceId { get; set; }
    }
}