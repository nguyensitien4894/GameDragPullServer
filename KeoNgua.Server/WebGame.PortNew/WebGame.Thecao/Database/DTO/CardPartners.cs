using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MsWebGame.Thecao.Database.DTO
{
    public class CardPartners
    {
        public long Id { get; set; }

        public string PartnerName { get; set; }

        public string VTT { get; set; }

        public string VNP { get; set; }

        public string VMS { get; set; }

        public int? Status { get; set; }
    }
}