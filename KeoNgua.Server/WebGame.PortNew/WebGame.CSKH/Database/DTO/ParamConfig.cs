using System;
using System.ComponentModel;

namespace MsWebGame.CSKH.Database.DTO
{
    public class ParamConfig
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