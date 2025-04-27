using System.Collections.Generic;
using System.ComponentModel;
using System.Web.Mvc;
using TraditionGame.Utilities.Utils;
using MsWebGame.CSKH.Database.DTO;

namespace MsWebGame.CSKH.Models.Agencies
{
    public class ListAgencyModel
    {
        public ListAgencyModel()
        {
            listAgencyOne = new List<SelectListItem>();
            listAgency = new List<Agency>();
            listStatus = new List<SelectListItem>();
        }

        [DisplayName("Tên đại lý")]
        public string AccountName { get; set; }
        [DisplayName("Tên hiển thị")]
        public string DisplayName { get; set; }
        [DisplayName("Đại lý cấp")]
        public short? AccountLevel { get; set; }
        [DisplayName("Số điện thoại")]
        public string PhoneOTP { get; set; }
        [DisplayName("Đại lý cha")]
        public long? ParrentID { get; set; }
        public List<SelectListItem> listAgencyOne { get; set; }
        public int? Status { get; set; }
        public List<SelectListItem> listStatus { get; set; }
        public List<Agency> listAgency { get; set; }
        public long TotalWallet { get; set; }
        public long TotalGiftCode { get; set; }
        public string TotalWalletFormat
        {
            get { return TotalWallet.LongToMoneyFormat(); }
        }
        public string TotalGiftCodeFormat
        {
            get { return TotalGiftCode.LongToMoneyFormat(); }
        }
    }
}