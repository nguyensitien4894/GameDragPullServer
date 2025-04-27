using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MsWebGame.Thecao.Database.DTO
{
    public class UserRedemption
    {
        public long ID { get; set; }

        public long UserID { get; set; }

        public long? RefundAmount { get; set; }

        public long? PriArtID { get; set; }

        public int? Quantity { get; set; }

        public int RankID { get; set; }

        public long VP { get; set; }

        public long Point { get; set; }

        public DateTime RankedMonth { get; set; }

        public bool Status { get; set; }

        public string Description { get; set; }

        public DateTime? RefundReceiveDate { get; set; }

        public DateTime? GiftReceiveDate { get; set; }

        public long? CreateUser { get; set; }

        public DateTime? CreateDate { get; set; }

        public long ? RequestVP { get; set; }

        public DateTime ? RedemptionDate { get; set; }
    }
}