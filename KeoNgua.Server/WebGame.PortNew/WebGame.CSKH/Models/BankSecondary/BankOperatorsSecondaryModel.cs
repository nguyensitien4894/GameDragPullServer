using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MsWebGame.CSKH.Models.BankSecondary
{
    public class BankOperatorsSecondaryModel
    {
        public BankOperatorsSecondaryModel()
        {
            //listStatus = new List<SelectListItem>();
        }
        public int ID { get; set; }
        [Required(ErrorMessageResourceType = typeof(Utils.Message), ErrorMessageResourceName = "OperatorCodeRequired")]
        [DisplayName("Mã ngân hàng")]
        public string OperatorCode { get; set; }
        [Required(ErrorMessageResourceType = typeof(Utils.Message), ErrorMessageResourceName = "OperatorNameRequired")]
        [DisplayName("Tên ngân hàng")]
        public string OperatorName { get; set; }
        [DisplayName("Tỉ lệ nạp")]
        public double Rate { get; set; }
        [DisplayName("Trạng thái nạp ")]
        public bool Status { get; set; }
        public long CreateUser { get; set; }
        public DateTime CreateDate { get; set; }
        //public List<SelectListItem> listStatus { get; set; }
        public int serviceId { get; set; }
    }
}