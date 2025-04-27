using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MsWebGame.CSKH.Database.DTO
{
    public class BankPartners
    {
        public long Id { get; set; }

        public string PartnerName { get; set; }

        public string Momo { get; set; }

        public string Bank { get; set; }

        public int? Status { get; set; }

        public int? ServiceID { get; set; }

    }
}