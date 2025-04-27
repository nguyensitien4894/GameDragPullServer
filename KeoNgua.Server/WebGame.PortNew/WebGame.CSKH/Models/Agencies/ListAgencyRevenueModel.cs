using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MsWebGame.CSKH.Database.DTO;

namespace MsWebGame.CSKH.Models.Agencies
{
    public class ListAgencyRevenueModel
    {
        public ListAgencyRevenueModel()
        {
            listReport = new List<AgencyRevenue>();
            listAgencyOne = new List<SelectListItem>();
            listLevel = new List<SelectListItem>();
            listLevel.Add(new SelectListItem() { Text = "Cấp 1", Value = "1" });
            listLevel.Add(new SelectListItem() { Text = "Cấp 2", Value = "2" });
        }


        [DisplayName("Từ ngày")]
        [UIHint("DateNullable")]
        public DateTime? StartDate { get; set; }
        [DisplayName("Tới ngày")]
        [UIHint("DateNullable")]
        public DateTime? EndDate { get; set; }
        public List<AgencyRevenue> listReport { get; set; }
        [DisplayName("Đại lý cha")]
        public long? ParrentID { get; set; }
        public List<SelectListItem> listAgencyOne { get; set; }
        [DisplayName("Level")]
        public int? Level { get; set; }
        public List<SelectListItem> listLevel
        {
            get; set;

        }
    }
}