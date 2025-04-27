using System;
using System.ComponentModel;
using TraditionGame.Utilities.Utils;
using MsWebGame.CSKH.Utils;

namespace MsWebGame.CSKH.Database.DTO
{
    public class GameBankExpertise
    {
        public long Id { get; set; }
        public long TrackingID { get; set; }
        public int GameID { get; set; }
        public string GameIDFormat { get { return GameID.IntToGameFormat(); } }
        public int RoomID { get; set; }
        public string RoomIDFormat { get { return RoomID.IntToRoomFormat(); } }
        public long PoolAccumulate { get; set; }
        public string PoolAccumulateFormat { get { return PoolAccumulate.LongToMoneyFormat(); } }
        public long PrizeAccumulate { get; set; }
        public string PrizeAccumulateFormat { get { return PrizeAccumulate.LongToMoneyFormat(); } }
        public long CurrentPrizeFund { get; set; }
        public string CurrentPrizeFundFormat { get { return CurrentPrizeFund.LongToMoneyFormat(); } }
        public long CurrentJackpotFund { get; set; }
        public string CurrentJackpotFundFormat { get { return CurrentJackpotFund.LongToMoneyFormat(); } }
        public long Flag { get; set; }
        public DateTime CreatedDate { get; set; }
        public int DateCycle { get; set; }
    }

    public class GameBankExpertiseProceed
    {
        public int GameID { get; set; }
        public string GameIDFormat { get { return GameID.IntToGameFormat(); } }
        public int RoomID { get; set; }
        public string RoomIDFormat { get { return RoomID.IntToRoomFormat(); } }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    public class GameBankInfo
    {
        [DisplayName("Ngày")]
        public int DateCycle { get; set; }

        [DisplayName("Bắt Đầu")]
        public long SpinBegin { get; set; }

        [DisplayName("Kết thúc")]
        public long SpinEnd { get; set; }

        [DisplayName("Từ ngày")]
        public DateTime StartDate { get; set; }

        [DisplayName("Đến ngày")]
        public DateTime EndDate { get; set; }

        [DisplayName("Ngày tạo")]
        public DateTime CreatedDate { get; set; }

        public int GameID { get; set; }
        [DisplayName("Game")]
        public string GameIDFormat { get { return GameID.IntToGameFormat(); } }

        public int RoomID { get; set; }
        [DisplayName("Phòng")]
        public string RoomIDFormat { get { return RoomID.IntToRoomFormat(); } }

        public long NumSpins { get; set; }
        [DisplayName("Số spin")]
        public string NumSpinsFormat { get { return NumSpins.LongToMoneyFormat(); } }

        public long TotalBet { get; set; }
        [DisplayName("Tổng cược")]
        public string TotalBetFormat { get { return TotalBet.LongToMoneyFormat(); } }

        [DisplayName("Tổng thưởng")]
        public long TotalPrize { get; set; }
        [DisplayName("Tổng thưởng")]
        public string TotalPrizeFormat { get { return TotalPrize.LongToMoneyFormat(); } }

        [DisplayName("Tổng payline")]
        public long TotalPayline { get; set; }
        [DisplayName("Tổng payline")]
        public string TotalPaylineFormat { get { return TotalPayline.LongToMoneyFormat(); } }

        [DisplayName("Tổng bonus")]
        public long TotalBonus { get; set; }
        [DisplayName("Tổng bonus")]
        public string TotalBonusFormat { get { return TotalBonus.LongToMoneyFormat(); } }

        [DisplayName("Tổng tích lũy")]
        public long TotalPyramid { get; set; }
        [DisplayName("Tổng tích lũy")]
        public string TotalPyramidFormat { get { return TotalPyramid.LongToMoneyFormat(); } }

        [DisplayName("Tổng X2")]
        public long TotalDouble { get; set; }
        [DisplayName("Tổng X2")]
        public string TotalDoubleFormat { get { return TotalDouble.LongToMoneyFormat(); } }

        [DisplayName("Quỹ thưởng")]
        public long PrizeFund { get; set; }
        [DisplayName("Quỹ thưởng")]
        public string PrizeFundFormat { get { return PrizeFund.LongToMoneyFormat(); } }

        [DisplayName("Quỹ hũ")]
        public long JackpotFund { get; set; }
        [DisplayName("Quỹ hũ")]
        public string JackpotFundFormat { get { return JackpotFund.LongToMoneyFormat(); } }

        [DisplayName("Quỹ tích lũy")]
        public long PyramidFund { get; set; }
        [DisplayName("Quỹ tích lũy")]
        public string PyramidFundFormat { get { return PyramidFund.LongToMoneyFormat(); } }

        [DisplayName("Quỹ X2")]
        public long DoubleFund { get; set; }
        [DisplayName("Quỹ X2")]
        public string DoubleFundFormat { get { return DoubleFund.LongToMoneyFormat(); } }
    }
}