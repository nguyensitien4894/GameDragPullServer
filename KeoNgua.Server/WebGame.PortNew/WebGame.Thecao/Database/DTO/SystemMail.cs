using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MsWebGame.Thecao.Database.DTO
{
    public class SystemMail
    {
        public long ID { get; set; }

       

        public string Title { get; set; }

        public string Content { get; set; }

        public DateTime? CreatedTime { get; set; }

    }
}