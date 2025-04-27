using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MsWebGame.CSKH.Models.CardPartnerss
{
    public class CardPartnersModel
    {
        public long Id { get; set; }

        public string PartnerName { get; set; }

        public string VTT { get; set; }

        public string VNP { get; set; }

        public string VMS { get; set; }
        public string ZING { get; set; }
        public string VCOIN { get; set; }

        public int? Status { get; set; }
        public int ServiceID { get; set; }
    }
}