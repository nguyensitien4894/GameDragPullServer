using System;
using TraditionGame.Utilities.Utils;
using MsWebGame.CSKH.Utils;

namespace MsWebGame.CSKH.Database.DTO
{
    public class Artifacts
    {
        public int ArtifactID { get; set; }
        public string ArtifactCode { get; set; }
        public string ArtifactName { get; set; }
        public long Price { get; set; }
        public string PriceStr { get; set; }
        public string PriceFormat { get { return Price.LongToMoneyFormat(); } }
        public bool Status { get; set; }
        public string StatusFormat { get { return Status.BoolToConfigGameFormat(); } }
        public string Description { get; set; }
        public long CreateUser { get; set; }
        public DateTime CreateDate { get; set; }
    }
}