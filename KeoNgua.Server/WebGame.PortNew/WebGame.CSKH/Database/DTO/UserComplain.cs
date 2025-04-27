using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MsWebGame.CSKH.Database.DTO
{
    public class UserComplain
    {
        public long ID { get; set; }

        public long? UserID { get; set; }

        public long? ComplainTypeID { get; set; }

        public string Content { get; set; }

        public bool Status { get; set; }

        public string Response { get; set; }

        public long UpdateUser { get; set; }

        public DateTime UpdateDate { get; set; }

        public int ServiceID { get; set; }

        public string ComplainTypeName { get; set; }
    }
}