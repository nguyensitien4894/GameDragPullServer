using System;
using System.Collections.Generic;
using TraditionGame.Utilities.Utils;

namespace MsWebGame.CSKH.Database.DTO
{
    public class ImportCardInfo
    {
        public int ID { get; set; }
        public int CardValue { get; set; }
        public string CardNumber { get; set; }
        public string CardSerial { get; set; }
        public DateTime ExperiedDate { get; set; }
        public int TelOperatorID { get; set; }
        public int Status { get; set; }
        public DateTime ImportDate { get; set; }
        public bool Result { get; set; }
        public string Description { get; set; }
    }

    public class ResultImportCardList
    {
        public List<ImportCardInfo> LstSuccess { get; set; }
        public List<ImportCardInfo> LstError { get; set; }
        public ParsBuyCard24H ParsBuyCard { get; set; }
    }

    public class ParsBuyCard24H {
        public string trace { get; set; }
        public string telco { get; set; }
        public int amount { get; set; }
        public int quantity { get; set; }
    }

    public class BuyCard24hData
    {
        public string ProviderCode { get; set; }
        public string Serial { get; set; }
        public string PinCode { get; set; }
        public int Amount { get; set; }
    }

    public class CardBankInfo
    {
        public int CardValue { get; set; }
        public string CardValueFormat { get { return CardValue.IntToMoneyFormat(); } }

        public string CardNumber { get; set; }
        public string CardSerial { get; set; }
        public DateTime ExperiedDate { get; set; }
        public int TelOperatorID { get; set; }
        public string OperatorName { get; set; }
        public int Status { get; set; }
        public string TradeType { get; set; }
        public DateTime ImportDate { get; set; }
        public DateTime SellDate { get; set; }
        public bool IsUsed { get; set; }
    }

    public class CardBankCheck
    {
        public int TelOperatorID { get; set; }
        public string OperatorName { get; set; }
        public int CardValue { get; set; }
        public string CardValueFormat { get { return CardValue.IntToMoneyFormat(); } }

        public int TotalQuantity { get; set; }
        public string TotalQuantityFormat { get { return TotalQuantity.IntToMoneyFormat(); } }

        public long TotalValue { get; set; }
        public string TotalValueFormat { get { return TotalValue.LongToMoneyFormat(); } }

        public int TotalRemainQuantity { get; set; }
        public string TotalRemainQuantityFormat { get { return TotalRemainQuantity.IntToMoneyFormat(); } }
    }
}