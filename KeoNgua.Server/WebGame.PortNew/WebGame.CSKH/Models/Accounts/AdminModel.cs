using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MsWebGame.CSKH.Models.Accounts
{
    public class AdminModel
    {
        public long AdminID { get; set; }
        [DisplayName("Tên đăng nhập")] [Required(ErrorMessageResourceType = typeof(Utils.Message),ErrorMessageResourceName = "UserNameRequired")]
        public string UserName { get; set; }
        [DisplayName("Số điện thoại ")]
        [Required(ErrorMessageResourceType = typeof(Utils.Message),  ErrorMessageResourceName = "PhoneRequired")]
        public string PhoneContact { get; set; }
        [DisplayName("Hòm thư ")]
        public string Email { get; set; }
        [DisplayName("Hạng ")]
        public int Level { get; set; }

        public int Status { get; set; }

        public string RoleID { get; set; }
        [DisplayName("Ngày tạo")]
        public DateTime? CreatedAt { get; set; }
       
        public long? CreatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }
        
        public long? UpdatedBy { get; set; }
        
        [DisplayName("Mật khẩu")]
        public string Password { get; set; }

        public long? Wallet { get; set; }

    }
}