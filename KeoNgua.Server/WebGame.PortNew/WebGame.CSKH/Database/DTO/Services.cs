using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MsWebGame.CSKH.Database.DTO
{
    public class Services
    {
        public int ServiceID { get; set; }

        public string ServiceName { get; set; }

        public bool Status { get; set; }

        public string Description { get; set; }
    }
}