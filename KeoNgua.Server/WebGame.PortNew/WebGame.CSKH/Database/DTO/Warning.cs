using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MsWebGame.CSKH.Database.DTO
{
    public class Warning
    {
        public long UserID { get; set; }
        public string DisplayName { get; set; }
        public int ServiceID { get; set; }
        public decimal TotalAmount { get; set; }
        
    }
}