using System;
using TraditionGame.Utilities.Utils;
using MsWebGame.CSKH.Utils;

namespace MsWebGame.CSKH.Database.DTO
{
    public class Privilege
    {
        public DateTime RankedMonth { get; set; }
        public int QuantityStone { get; set; }
        public int QuantityBronze { get; set; }
        public int QuantitySilver { get; set; }
        public int QuantityGold { get; set; }
        public int QuantityDiamond { get; set; }
        public long TotalVPStone { get; set; }
        public long TotalVPBronze { get; set; }
        public long TotalVPSilver { get; set; }
        public long TotalVPGold { get; set; }
        public long TotalVPDiamond { get; set; }
        public long TotalPrizeStone { get; set; }
        public long TotalPrizeBronze { get; set; }
        public long TotalPrizeSilver { get; set; }
        public long TotalPrizeGold { get; set; }
        public long TotalPrizeDiamond { get; set; }
    }

    public class PrivilegeArtifacts
    {
        public long PriArtID { get; set; }
        public int PrivilegeID { get; set; }
        public string PrivilegeName { get; set; }
        public int ArtifactID { get; set; }
        public string ArtifactName { get; set; }
        public int Quantity { get; set; }
        public string QuantityStr { get; set; }
        public string QuantityFormat { get { return Quantity.IntToMoneyFormat(); } }
        public int RemainQuantity { get; set; }
        public string RemainQuantityStr { get; set; }
        public string RemainQuantityFormat { get { return RemainQuantity.IntToMoneyFormat(); } }
        public long TotalPrize { get; set; }
        public string TotalPrizeStr { get; set; }
        public string TotalPrizeFormat { get { return TotalPrize.LongToMoneyFormat(); } }
        public bool Status { get; set; }
        public string StatusFormat { get { return Status.BoolToConfigGameFormat(); } }
        public string Description { get; set; }
        public long CreateUser { get; set; }
        public DateTime CreateDate { get; set; }
    }

    public class PrivilegeGameInfo
    {
        public int GameID { get; set; }
        public string GameCode { get; set; }
        public string GameName { get; set; }
        public double GameWeight { get; set; }
        public double ProfitMargin { get; set; }
        public double ConversionCoefficient { get; set; }
        public string ConversionCoefficientStr { get; set; }
        public string ConversionCoefficientFormat { get { return ConversionCoefficient.DoubleToMoneyFormat(); } }
        public bool Status { get; set; }
        public string StatusFormat { get { return Status.BoolToConfigGameFormat(); } }
    }

    public class PrivilegeLevel
    {
        public int ID { get; set; }
        public string PrivilegeCode { get; set; }
        public string PrivilegeName { get; set; }
        public long VP { get; set; }
        public string VPStr { get; set; }
        public string VPFormat { get { return VP.LongToMoneyFormat(); } }
        public bool Status { get; set; }
        public string StatusFormat { get { return Status.BoolToConfigGameFormat(); } }
    }

    public class PrivilegeLookup
    {
        public string UserName { get; set; }
        public string DisplayName { get; set; }
        public int MonthRank { get; set; }
        public int YearRank { get; set; }
        public DateTime IncreaseRankDate { get; set; }
        public long VipPoint { get; set; }
        public int RankID { get; set; }
        public string RankIDFormat { get { return RankID.IntToRankFormat(); } }
        public long AwardPrize { get; set; }
        public long? NotAwardPrize { get; set; }
        public DateTime? RefundReceiveDate { get; set; }
    }

    public class PrivilegePrize
    {
        public int RankID { get; set; }
        public string RankIDFormat { get { return RankID.IntToRankFormat(); } }
        public double RefundRate { get; set; }
        public double PointExchangeRate { get; set; }
        public double GiftRate { get; set; }
        public double MoneyExchangeRate { get; set; }
        public string MoneyExchangeRateStr { get; set; }
        public string MoneyExchangeRateFormat { get { return MoneyExchangeRate.DoubleToMoneyFormat(); } }
        public bool Status { get; set; }
        public string StatusFormat { get { return Status.BoolToConfigGameFormat(); } }
    }

    public class VpProgress
    {
        public string period { get; set; }
        public long total { get; set; }
        public string totalFormat { get { return total.LongToMoneyFormat(); } }
    }

    public class ReportVP
    {
        public DateTime ngay { get; set; }
        public long pu { get; set; }
        public string puFormat { get { return pu.LongToMoneyFormat(); }  }
    }
}