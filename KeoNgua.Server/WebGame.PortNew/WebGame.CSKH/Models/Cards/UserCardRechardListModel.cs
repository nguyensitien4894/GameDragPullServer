using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MsWebGame.CSKH.Models.Cards
{
    public class UserCardRechardListModel
    {
        public UserCardRechardListModel()
        {
            listTelecom = new List<SelectListItem>();
            listCard = new List<SelectListItem>();
            listStatus = new List<SelectListItem>();
            listPartner = new List<SelectListItem>();
            listSMG = new List<SelectListItem>();
            listValue = new List<SelectListItem>();


        }

        public string PartnerErrorCode { get; set; }
        [DisplayName("Nhà mạng ")]
        public long? TelOperatorID { get; set; }
        [DisplayName("Giá trị thẻ ")]
        public int? CardValue { get; set; }
        [DisplayName("Nickname nạp ")]
        public string GameAccountName { get; set; }
        [DisplayName("Mã thẻ ")]
        public string CardNumber { get; set; }
        [DisplayName("Serial thẻ ")]
        public string SerialNumber { get; set; }
        [UIHint("DateTimeNullable")]
        [DisplayName("Tới ngày ")]
        public DateTime? ToRechargeDate { get; set; }
        [UIHint("DateTimeNullable")]
        [DisplayName("Từ ngày ")]
        public DateTime? FromRechargeDate { get; set; }
        public int? Status { get; set; }
        public int ?PartnerID { get; set; }
        public int ?smg { get; set; }
        public int ? Check_Refund { get; set; }
        public List<SelectListItem> listTelecom { get; set; }
        public int ? cardValue { get; set; }
        public List<SelectListItem> listCard { get; set; }
        public List<SelectListItem> listStatus { get; set; }
        public List<SelectListItem> listPartner { get; set; }
        public List<SelectListItem> listSMG { get; set; }
        public List<SelectListItem> listValue { get; set; }
    }
}