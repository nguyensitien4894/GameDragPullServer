using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MsWebGame.CSKH.Models.Transactions
{
    public class TransactionFreeModel
    {
        public string NickName { get; set; }
        public DateTime? fromDate { get; set; }
        public DateTime? toDate { get; set; }
        public string PartnerName { get; set; }
        public int ServiceID { get; set; }
    }
}