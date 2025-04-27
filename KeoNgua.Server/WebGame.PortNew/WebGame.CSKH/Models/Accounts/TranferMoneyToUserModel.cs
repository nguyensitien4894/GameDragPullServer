using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MsWebGame.CSKH.Models.Accounts
{
    public class TranferMoneyToUserModel
    {
        public long AdminID { get; set; }
        [DisplayName("Tên admin chuyển bit ")]
        public string AdminAccountName { get; set; }
        [Required(ErrorMessageResourceType = typeof(Utils.Message), ErrorMessageResourceName = "UserRequired")]
        [DisplayName("Tên người dùng nhận bit ")]
        public string UserName { get; set; }
        [Range(100000, int.MaxValue, ErrorMessage = "Số bit giao dịch lớn hơn {1}")]
        [DisplayName("Số bit giao dịch ")]
        [UIHint("Long")]
        public long Amount { get; set; }
        [DisplayName("Ghi chú ")]
        [Required(ErrorMessageResourceType = typeof(Utils.Message), ErrorMessageResourceName = "NoteRequired")]
        public string Note { get; set; }

        [DisplayName("Số Bit hiện có ")]
        [UIHint("Long")]
        public long AdminAmount { get; set; }
    }
}