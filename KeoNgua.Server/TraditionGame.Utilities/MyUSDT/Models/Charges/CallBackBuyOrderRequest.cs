using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TraditionGame.Utilities.MyUSDT.Models.Charges
{
    public  class CallBackBuyOrderRequest
    {
        /// <summary>
        ///  id cặp tiền tệ (ở đây 1 là id cặp USDT-VCC)
        /// </summary>
        [JsonProperty("currency_pair")]
        public long CurrencyPair { get; set; }
        /// <summary>
        /// Status: trạng thái đơn hàng (pending, processing, completed, canceled,failed)
        /// </summary>
        [JsonProperty("status")]
        public string Status { get; set; }
        /// <summary>
        /// buy
        /// </summary>
        [JsonProperty("method")]
        public string Method { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("amount")]
        public string Amount { get; set; }

        [JsonProperty("rate")]
        public double Rate { get; set; }
        /// <summary>
        /// link call back
        /// </summary>
        [JsonProperty("call_back")]
        public Uri CallBack { get; set; }
        /// <summary>
        /// mã code order
        /// </summary>
        [JsonProperty("code")]
        public string Code { get; set; }
        /// <summary>
        /// thời gian timeout 
        /// </summary>
        [JsonProperty("timeout")]
        public long Timeout { get; set; }

        [JsonProperty("note")]
        public CallBankOrderRequestNote Note { get; set; }
        /// <summary>
        /// email nhận thông báo
        /// </summary>
        [JsonProperty("email")]
        public string Email { get; set; }
        /// <summary>
        /// thời gian tạo đơn hàng
        /// </summary>
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
        [JsonProperty("sms_info")]
        public SmsInfo SmsInfo { get; set; }
    }

    public  class SmsInfo
    {
        [JsonProperty("ref")]
        public string Ref { get; set; }

        [JsonProperty("amount")]
        public long Amount { get; set; }

        [JsonProperty("balance")]
        public long Balance { get; set; }

        [JsonProperty("context")]
        public string Context { get; set; }

        [JsonProperty("sms_type")]
        public long SmsType { get; set; }
    }

    public  class CallBankOrderRequestNote
    {
        [JsonProperty("bank_name")]
        public string BankName { get; set; }

        [JsonProperty("master_bank_name")]
        public string MasterBankName { get; set; }

        [JsonProperty("master_bank_account")]
        public string MasterBankAccount { get; set; }

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
        [JsonProperty("send_amount")]
        public string SendAmount { get; set; }

        [JsonProperty("receive_usdt_address")]
        public string ReceiveUsdtAddress { get; set; }
    }
}