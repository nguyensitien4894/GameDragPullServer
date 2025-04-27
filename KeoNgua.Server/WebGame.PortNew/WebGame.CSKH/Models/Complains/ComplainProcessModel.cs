using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MsWebGame.CSKH.Models.Complains
{
    public class ComplainProcessModel
    {
        public int No { get; set; }
        public string CreateResult { get; set; }
        public string ReceiveResult { get; set; }
        public long AdminID { get; set; }
        public string AdminUserName { get; set; }
        public string Create { get; set; }
        public long TransID { get; set; }
        public long ComplainID { get; set; }
    }
}