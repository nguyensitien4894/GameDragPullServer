using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MsWebGame.CSKH.Utils;

namespace MsWebGame.CSKH.Models.Accounts
{
    public class TranferMoneyModel
    {
        public TranferMoneyModel()
        {
            lstAgencys = new List<SelectListItem>();
        }
        public long AdminID { get; set; }
        [DisplayName("Tên admin chuyển Bit ")]
        public string AdminAccountName { get; set; }
        [DisplayName("Số Bit hiện có ")]
        [UIHint("Long")]
        public long AdminAmount { get; set; }
        [Required(ErrorMessageResourceType = typeof(Utils.Message), ErrorMessageResourceName = "AgencyParentRequired")]
        [DisplayName("Tên đại lý nhận Bit ")]
        public long AgencyId { get; set; }
        
        public List<SelectListItem> lstAgencys { get; set; }
        [Range(100000, int.MaxValue, ErrorMessage = "Số Bit giao dịch lớn hơn {1}")]
        [DisplayName("Số Bit giao dịch ")]
        [Required(ErrorMessageResourceType = typeof(Utils.Message), ErrorMessageResourceName = "AmountRequired")]
        [UIHint("Long")]
        public long Amount { get; set; }
        [DisplayName("Ghi chú ")]
       
        [Required(ErrorMessageResourceType = typeof(Utils.Message), ErrorMessageResourceName = "NoteRequired")]
        public string Note { get; set; }
    }
}