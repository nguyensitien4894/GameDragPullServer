using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MsWebGame.CSKH.Models.BankSecondary
{
    public class BankSecondaryModel
    {
        public BankSecondaryModel()
        {
        }

        public int ID { get; set; }
        public string BankNumber { get; set; }
        public string BankName { get; set; }
        public bool? Status { get; set; }
        public DateTime CreateDate { get; set; }
        [DisplayName("Tên ngân hàng ")]
        public string OperatorName { get; set; }
        [DisplayName("Ngân hàng ")]
        public int BankOperatorsSecondaryID { get; set; }
        public int ServiceID { get; set; }
    }
}