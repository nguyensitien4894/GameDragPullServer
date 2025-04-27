using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace MsWebGame.CSKH.Models.ParamConfigs
{
    public class ParamConfigModel
    {
        public int ID { get; set; }
        [DisplayName("Loại cấu hình")]
        public string ParamType { get; set; }
        [DisplayName("Mã")]
        public string Code { get; set; }
        [DisplayName("Giá trị")]
        public string Value { get; set; }
        [DisplayName("Mô tả ")]
        public string Description { get; set; }

        public int? Status { get; set; }

        public DateTime? CreatedTime { get; set; }
    }
}