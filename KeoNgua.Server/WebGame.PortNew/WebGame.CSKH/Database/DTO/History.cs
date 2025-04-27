using System;
using TraditionGame.Utilities.Utils;
using MsWebGame.CSKH.Utils;

namespace MsWebGame.CSKH.Database.DTO
{
    public class HistoryJackpot
    {
        public long AccountID { get; set; }
        public string UserName { get; set; }
        public int GameID { get; set; }
        public string GameIDFormat { get { return GameID.IntToGameFormat(); } }
        public long PrizeValue { get; set; }
        public string PrizeValueFormat { get { return PrizeValue.LongToMoneyFormat(); } }
        public DateTime CreatedTime { get; set; }
    }

    public class HistoryThankful
    {
        public string AccountID { get; set; }
        public string NickName { get; set; }
        public long SumLoose { get; set; }
        public long ThankfulAward { get; set; }
        //public int GameID { get; set; }
        //public string GameIDFormat { get { return GameID.IntToGameFormat(); } }
        //public long PrizeValue { get; set; }
        //public string PrizeValueFormat { get { return PrizeValue.LongToMoneyFormat(); } }
        //public DateTime FromDate { get; set; }
        //public DateTime ToDate { get; set; }
    }

    public class HistoryThankfulAccountList
    {
        public long AccountID { get; set; }
        public string NickName { get; set; }

        public long GameMode { get; set; }
    }

    public class HistoryPlay
    {
        public long STT { get; set; }
        public long OrgBalance { get; set; }
        public long Balance { get; set; }
        public string OrgBalanceFormat { get { return OrgBalance.LongToMoneyFormat(); } }
        public string BalanceFormat { get { return Balance.LongToMoneyFormat(); } }
        public long BetValue { get; set; }
        public string BetValueFormat { get { return BetValue.LongToMoneyFormat(); } }
        public long PrizeValue { get; set; }
        public string PrizeValueFormat { get { return PrizeValue.LongToMoneyFormat(); } }
        public long RefundValue { get; set; }
        public string RefundValueFormat { get { return RefundValue.LongToMoneyFormat(); } }
        public long SpinID { get; set; }
        public int GameType { get; set; }
        public string GameTypeFormat { get { return GameType.IntToGameFormat(); } }
        public string Description { get; set; }
        public DateTime PlayTime { get; set; }
        public long AccountID { get; set; }
    }

    public class HistoryTransfer
    {
        public long CreateUserID { get; set; }
        public string CreateUserName { get; set; }
        public string ReceiverName { get; set; }
        public string CreateDisplayName { get; set; }
        public string ReceiverDisplayName { get; set; }
        public long OrgAmount { get; set; }
        public long OrgBalance { get; set; }
        public long Amount { get; set; }
        public DateTime TransDate { get; set; }
        public string TranStatus { get; set; }
        public string Description { get; set; }
        public string Note { get; set; }
        public int ReceiverType { get; set; }




    }

    public class HistoryWalletLog
    {
        public long UserID { get; set; }
        public string Username { get; set; }
        public string DisplayName { get; set; }
        public long OrgBalance { get; set; }
        public string OrgBalanceFormat
        {
            get { return OrgBalance.LongToMoneyFormat(); }
        }
        public long Amount { get; set; }
        public string AmountFormat
        {
            get { return Amount.LongToMoneyFormat(); }
        }
        public long RemainBalance { get; set; }
        public string RemainBalanceFormat
        {
            get { return RemainBalance.LongToMoneyFormat(); }
        }
        public string PartnerName { get; set; }
        public int PartnerType { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Description { get; set; }
    }
}