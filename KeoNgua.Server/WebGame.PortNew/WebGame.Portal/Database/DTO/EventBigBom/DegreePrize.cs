using System;

namespace MsWebGame.Portal.Database.DTO.EventBigBom
{
	public class DegreePrize
    {
        public int DegreeID { get; set; }
        public long B1P { get; set; }
        public long RefundAmount { get; set; }
        public int RedeemStatus { get; set; }
        public string Quantity { get; set; }
        //public string RedeemStatusStr
        //{
        //    get
        //    {
        //        if (RedeemStatus ==0) return "Chưa nhận";
        //        else if (RedeemStatus == 1) return "Đã nhận";
        //        else if (RedeemStatus == 2) return "Hết hạn mức";
        //        else return string.Empty;
        //    }
        //}
    }
}