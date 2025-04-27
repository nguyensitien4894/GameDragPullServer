using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MsWebGame.CSKH.Database.DTO
{
    public class UserComplainType
    {
        public long ID { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public long? UpdateUser { get; set; }

        public DateTime? UpdateDate { get; set; }
    }
}