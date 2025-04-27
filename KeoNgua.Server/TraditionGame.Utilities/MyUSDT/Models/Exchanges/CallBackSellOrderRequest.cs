using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TraditionGame.Utilities.MyUSDT.Models.Exchanges
{
    public  class CallBackSellorderRequestModel
    {
        [JsonProperty("currency_pair")]
        public long CurrencyPair { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("method")]
        public string Method { get; set; }

        [JsonProperty("amount")]
        public double Amount { get; set; }

        [JsonProperty("rate")]
        public double Rate { get; set; }

        [JsonProperty("call_back")]
        public Uri CallBack { get; set; }

        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("timeout")]
        public long Timeout { get; set; }

        [JsonProperty("note")]
        public CallBackSellorderRequestModelNote Note { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("date_added")]
        public DateTimeOffset DateAdded { get; set; }

        [JsonProperty("remaining")]
        public long Remaining { get; set; }

        [JsonProperty("amount_vcc")]
        public long AmountVcc { get; set; }

        [JsonProperty("amount_usdt")]
        public double AmountUsdt { get; set; }

        [JsonProperty("input_transaction")]
        public InputTransaction InputTransaction { get; set; }

        [JsonProperty("output_transaction")]
        public OutputTransaction OutputTransaction { get; set; }
    }

    public  class InputTransaction
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("date_added")]
        public DateTimeOffset DateAdded { get; set; }

        [JsonProperty("date_modified")]
        public DateTimeOffset DateModified { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("hash")]
        public string Hash { get; set; }

        [JsonProperty("note")]
        public InputTransactionNote Note { get; set; }

        [JsonProperty("currency")]
        public long Currency { get; set; }

        [JsonProperty("order")]
        public long Order { get; set; }
    }

    public  class InputTransactionNote
    {
        [JsonProperty("omni_info")]
        public OmniInfo OmniInfo { get; set; }
    }

    public  class OmniInfo
    {
        [JsonProperty("amount")]
        public string Amount { get; set; }

        [JsonProperty("confirmations")]
        public long Confirmations { get; set; }
    }

    public  class CallBackSellorderRequestModelNote
    {
        [JsonProperty("bank_name")]
        public string BankName { get; set; }

        [JsonProperty("bank_account")]
        public string BankAccount { get; set; }

        [JsonProperty("bank_account_name")]
        public string BankAccountName { get; set; }

        [JsonProperty("receive_usdt_address")]
        public string ReceiveUsdtAddress { get; set; }
    }

    public  class OutputTransaction
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("date_added")]
        public DateTimeOffset DateAdded { get; set; }

        [JsonProperty("date_modified")]
        public DateTimeOffset DateModified { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("hash")]
        public string Hash { get; set; }

        [JsonProperty("note")]
        public OutputTransactionNote Note { get; set; }

        [JsonProperty("currency")]
        public long Currency { get; set; }

        [JsonProperty("order")]
        public long Order { get; set; }
    }

    public  class OutputTransactionNote
    {
        [JsonProperty("bank_fee")]
        public long BankFee { get; set; }

        [JsonProperty("sms_info")]
        public SmsInfo SmsInfo { get; set; }

        [JsonProperty("bank_name")]
        public string BankName { get; set; }

        [JsonProperty("bank_account")]
        public string BankAccount { get; set; }

        [JsonProperty("bank_account_name")]
        public string BankAccountName { get; set; }
    }

    public  class SmsInfo
    {
        [JsonProperty("ref")]
        public string Ref { get; set; }

        [JsonProperty("amount")]
        public long Amount { get; set; }

        [JsonProperty("context")]
        public string Context { get; set; }

        [JsonProperty("sms_type")]
        public long SmsType { get; set; }

        [JsonProperty("bank_name")]
        public string BankName { get; set; }
    }
}