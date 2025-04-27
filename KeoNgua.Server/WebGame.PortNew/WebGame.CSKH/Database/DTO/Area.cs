using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MsWebGame.CSKH.Database.DTO
{
    public class Area
    {
        public long AreaID { get; set; }

        public string AreaCode { get; set; }

        public string AreaName { get; set; }

        public string ZipCode { get; set; }

        public long? ParentID { get; set; }

        public bool? Status { get; set; }

        public string Description { get; set; }

    }

} 