using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace MsWebGame.CSKH.Models.AccountHack
{
    public class AccountHackModel
    {
        public long ID { get; set; }
        [DisplayName("Chủ Tài Khoản")]
        public string AccountBankName { get; set; }
        [DisplayName("Số Tài khoản")]
        public string AccountBankNumber { get; set; }
        [DisplayName("Tên Ngân Hàng")]
        public string BankName { get; set; }
        public bool? Status { get; set; }
        public long AccountID { get; set; }
        [DisplayName("Tên Hiện Thị")]
        public string UserName { get; set; }
        public string Reason { get; set; }
    }
}