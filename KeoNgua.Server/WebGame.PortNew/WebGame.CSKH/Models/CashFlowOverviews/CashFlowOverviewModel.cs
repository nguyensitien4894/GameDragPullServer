using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace MsWebGame.CSKH.Models.CashFlowOverviews
{
    public class CashFlowOverviewModel
    {
        [DisplayName("Ngày tháng")]
        public DateTime CheckDate { get; set; }
        [DisplayName("Tổng nạp thẻ")]
        public long? TotalCardRechargeValue { get; set; }
        [DisplayName("Tổng đổi thẻ")]
        public long? TotalCardSwapValue { get; set; }
        [DisplayName("Tổng  đại lý bán")]
        public long? TotalAgencySellValue { get; set; }
        [DisplayName("Tổng  đại lý mua")]
        public long? TotalAgencyBuyValue { get; set; }
        [DisplayName("Tổng  lợi nhuận game")]
        public long? TotalGameProfit { get; set; }
        [DisplayName("Tổng  phí đại lý")]
        public long? TotalAgencyFee { get; set; }
        [DisplayName("Tổng  phí người dùng")]
        public long? TotalUserFee { get; set; }
        [DisplayName("Tổng  giá trị gift code")]
        public long? TotalGiftcodeValue { get; set; }
        [DisplayName("Tổng  giá trị vqmm")]
        public long? TotalVqmmValue { get; set; }
        [DisplayName("Tổng  giá trị Vip")]
        public long? TotalVipReward { get; set; }
        [DisplayName("Tổng  đại lý trả thưởng")]
        public long? TotalAgencyReward { get; set; }
        [DisplayName("Tổng giá trị event")]
        public long? TotalEventValue { get; set; }
        [DisplayName("STT")]
        public int STT { get; set; }
    }
}