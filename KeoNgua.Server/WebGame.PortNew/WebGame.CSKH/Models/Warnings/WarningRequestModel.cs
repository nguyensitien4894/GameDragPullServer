using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MsWebGame.CSKH.Models.Warnings
{
    public class WarningRequestModel
    {
        public WarningRequestModel()
        {
            LstType = new List<SelectListItem>();
            LstLimitAmount = new List<SelectListItem>();
            LstQuata = new List<SelectListItem>();
            LstVP = new List<SelectListItem>();

        }



        public int WarningType { get; set; }

        public int VPLimit { get; set; }
        public int QuotaDay { get; set; }
        public long LimitAmount { get; set; }
        public List<SelectListItem > LstType { get; set; }
        public List<SelectListItem> LstLimitAmount { get; set; }
        public List<SelectListItem> LstQuata { get; set; }
        public List<SelectListItem> LstVP { get; set; }

    }
}