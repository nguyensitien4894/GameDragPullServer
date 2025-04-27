using System;


namespace MsWebGame.Portal.Database.DTO
{
    public class GameGiftCode
    {
        public long ID { get; set; }
        public long CampaignID { get; set; }
        public string CampaignCode { get; set; }
        public string CampaignName { get; set; }
        public string GiftCode { get; set; }

        public int GiftCodeType { get; set; }
        //public string GiftCodeTypeFormat
        //{
        //    get { return GiftCodeType.IntToGiftCodeTypeFomat(); }
        //}

        //public long MoneyReward { get; set; }
        //public string MoneyRewardFormat
        //{
        //    get { return MoneyReward.LongToMoneyFormat(); }
        //}

        public long TotalUsed { get; set; }
        //public string TotalUsedFormat
        //{
        //    get { return TotalUsed.LongToMoneyFormat(); }
        //}

        public long LimitQuota { get; set; }
        //public string LimitQuotaFormat
        //{
        //    get { return LimitQuota.LongToMoneyFormat(); }
        //}

        public long UserID { get; set; }
        public string Username { get; set; }

        public bool Status { get; set; }
        //public string StatusFormat
        //{
        //    get { return Status.BoolToConfigGameFormat(); }
        //}

        public DateTime ExpiredDate { get; set; }
        public DateTime CreateDate { get; set; }
    }
}