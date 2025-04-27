using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MsWebGame.Thecao.Database.DTO
{
    public class PrivilegeType
    {
        public int ID { get; set; }

        public string PrivilegeCode { get; set; }

        public string PrivilegeName { get; set; }

        public long VP { get; set; }

        public bool Status { get; set; }
        public double ExChangeBit { get; set; }
    }
}