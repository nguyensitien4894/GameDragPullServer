using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MsWebGame.CSKH.Utils;

namespace MsWebGame.CSKH.Models.GiftCodes
{
    public class GiftCodeAdMode
    {
        public GiftCodeAdMode()
        {
            listGiftTypes = new List<SelectListItem>();
        }
        [DisplayName("Chiến dịch")]
     
        [Required(ErrorMessageResourceType = typeof(Utils.Message), ErrorMessageResourceName = "CampaignRequired")]
        public string CampaignName { get; set; }
        [Required(ErrorMessageResourceType = typeof(Utils.Message), ErrorMessageResourceName = "GiftcodeTypeRequired")]
        [DisplayName("Loại chiến dịch")]
        public int GiftcodeType { get; set; }
        [Required(ErrorMessageResourceType = typeof(Utils.Message), ErrorMessageResourceName = "MoneyRewardMoneyReward")]
        [DisplayName("Mệnh giá")]
        public string  MoneyReward { get; set; }
        
         [Required(ErrorMessageResourceType = typeof(Utils.Message), ErrorMessageResourceName = "QuantityRequired")]
        [Range(1,Int32.MaxValue, ErrorMessageResourceType = typeof(Utils.Message), ErrorMessageResourceName = "QuantityMin")]
        [DisplayName("Số lượng")]
        public int Quantity { get; set; }
        [Required(ErrorMessageResourceType = typeof(Utils.Message), ErrorMessageResourceName = "DescriptionRequired")]
        [DisplayName("Mô tả")]
        public string Description { get; set; }

        [DisplayName("Ngày hết hạn")]
        public DateTime  ExpireDate { get; set; }
        public int serviceId { get; set; }

        public List<SelectListItem > listGiftTypes { get; set; }
  

    }
}