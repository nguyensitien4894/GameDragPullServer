using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MsWebGame.CSKH.Database.DTO
{
    public class SystemMail
    {
        
        public long ID { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public DateTime? CreateTime { get; set; }

        public bool? Status { get; set; }

       
    }
}