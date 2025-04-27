using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MsWebGame.CSKH.Database.DTO;

namespace MsWebGame.CSKH.Models.Agencies
{
    public class ListImportC2Model
    {
        public ListImportC2Model()
        {
            listAgencyOne = new List<SelectListItem>();
            listSuccess = new List<TmpAgencyC2>();
            ListError = new List<TmpAgencyC2>();
        }
        [DisplayName("Đại lý cha")]
        public long? ParrentID { get; set; }
        public List<SelectListItem> listAgencyOne { get; set; }

        public List<TmpAgencyC2> listSuccess { get; set; }
        public List<TmpAgencyC2> ListError { get; set; }

    }
}