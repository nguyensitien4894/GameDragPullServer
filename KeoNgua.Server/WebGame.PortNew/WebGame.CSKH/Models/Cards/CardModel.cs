using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using TraditionGame.Utilities.Utils;

namespace MsWebGame.CSKH.Models.Cards
{
    public class CardModel
    {
        public CardModel()
        {
            listStatus = new List<SelectListItem>();
            listTelecom = new List<SelectListItem>();
        }
        public int ID { get; set; }
        [Required(ErrorMessageResourceType = typeof(Utils.Message), ErrorMessageResourceName = "CardCodeRequired")]
        [DisplayName("Mã thẻ cào ")]
        public string CardCode { get; set; }
        [Required(ErrorMessageResourceType = typeof(Utils.Message), ErrorMessageResourceName = "CardNameRequired")]
        [DisplayName("Tên thẻ cào ")]
        public string CardName { get; set; }
       
        [DisplayName("Giá trị thẻ")]
        public int? CardValue { get; set; }
        [Required(ErrorMessageResourceType = typeof(Utils.Message), ErrorMessageResourceName = "CardValueRequired")]
        [DisplayName("Giá trị thẻ")]
        public string CardValueStr { get; set; }

        public double? CardRate { get; set; }

        public double? CardSwapRate { get; set; }
        [Required(ErrorMessageResourceType = typeof(Utils.Message), ErrorMessageResourceName = "StatusRequired")]
        [DisplayName("Trạng thái nạp thẻ ")]
        public bool Status { get; set; }
        [DisplayName("Trạng thái đổi thẻ ")]
        public bool ExchangeStatus { get; set; }
        public long CreateUser { get; set; }

        public DateTime ChargeRate { get; set; }

        public long UpdateUser { get; set; }

        public DateTime UpdateDate { get; set; }
        [Required(ErrorMessageResourceType = typeof(Utils.Message), ErrorMessageResourceName = "TelecomRequired")]
        [DisplayName("Nhà mạng")]
        public long ? TelecomOperatorsID { get; set; }
        [DisplayName("Tên nhà mạng ")]
        public string OperatorName { get; set; }

        public List<SelectListItem> listStatus { get; set; }
        public List<SelectListItem> listTelecom { get; set; }
        public List<SelectListItem> listPartners { get; set; }
        public long? PartnerId { get; set; }
        [DisplayName("Đối tác cho mệnh giá đặc biệt")]
        public string PartnerName { get; set; }
        [DisplayName("Cổng")]
        public string ServiceName { get; set; }
        [DisplayName("Giá trị thẻ")]
        public string CardValueFormat
        {
            get
            {
                return Convert.ToInt64(CardValue).LongToMoneyFormat();
            }
        }
        public int ServiceID { get; set; }
    }
}