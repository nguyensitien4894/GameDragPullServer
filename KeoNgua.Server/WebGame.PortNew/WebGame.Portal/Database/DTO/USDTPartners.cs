using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MsWebGame.Portal.Database.DTO
{
    public class USDTPartners
    {
        public long Id { get; set; }

        public string PartnerName { get; set; }

        public string Bank { get; set; }

        public int? Status { get; set; }

        public int? ServiceID { get; set; }
    }
}