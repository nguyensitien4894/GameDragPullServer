using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MsWebGame.CSKH.Models.Cards
{
    public class CardListModel
    {
        public CardListModel()
        {
            listStatus = new List<SelectListItem>();
            listTelecom = new List<SelectListItem>();
            listPartners = new List<SelectListItem>();
        }
        [DisplayName("Trạng thái nạp thẻ ")]
        public bool ?Status { get; set; }
        public List<SelectListItem> listStatus { get; set; }
        public List<SelectListItem> listTelecom { get; set; }
        public List<SelectListItem> listPartners { get; set; }
        [DisplayName("Nhà mạng")]
        public long TeleId { get; set; }
        [DisplayName("Tên thẻ cào")]
        public string CardName { get; set; }
        [DisplayName("Mã thẻ")]
        public string CardCode { get; set; }
        public int PartnerId { get; set; }
        public int ServiceID { get; set; }


    }
}