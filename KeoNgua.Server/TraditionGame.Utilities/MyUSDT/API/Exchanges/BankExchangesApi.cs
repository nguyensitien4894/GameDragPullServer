using MsWebGame.Portal.Helpers.MyUSDT.Models.Exchanges;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using TraditionGame.Utilities;
using TraditionGame.Utilities.MyUSDT.API;
using TraditionGame.Utilities.MyUSDT.Models.Exchanges;

namespace TraditionGame.Utilities.Exchanges
{
    public class BankExchangesApi : BaseApiRequest
    {
        protected static string CALLBACK_ORDER = ConfigurationManager.AppSettings["CALLBACK_SELL_ORDER"].ToString();
        /// <summary>
        /// Lấy danh sách bank khi rút
        /// </summary>
        /// <returns></returns>
        public static WithDrawBankListModel GetBanks()
        {
            try
            {
                string BankUrl = BaseUrl + "/v1/bank-config/";
                int statusCode = 0;
                string msg = string.Empty;
                var model = GetJson<WithDrawBankListModel>(BankUrl,null,out statusCode,out msg);

                return model;
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
            }
            return null;

        }

        private static string METHOD = "sell";

       
        /// <summary>
        ///Gọi lệnh tạo order USDT (bán )
        /// </summary>
        /// <param name="TargetAmount"></param>
        /// <param name="BankAccount">số tài khoản người nhận VND</param>
        /// <param name="BankAccountName">Tên chủ tài khoản nhận VND</param>
        /// <param name="BankName">Tên ngân hàng</param>
        /// <returns></returns>
        public static SellOrderResponseModel SendSellOrder(long TargetAmount,string BankAccount,string BankAccountName,string BankName)
        {
            try
            {
                var request = new SellOrderRequestModel();
                request.CurrencyPair = CURRENT_PAIR;
                request.Method = METHOD;
               
                request.TargetAmount = TargetAmount;
                request.Email = EMAIL_BANK;
                request.CallBack = CALLBACK_ORDER;
                request.Note = new SellOrderRequestNoteModel()
                {
                    BankAccount = BankAccount,
                    BankAccountName= BankAccountName,
                    BankName= BankName
                };

                var _url = BaseUrl + "/v1/orders/";
                int satusCode = 0;
                string msg = string.Empty;
                var model = PostJson<SellOrderResponseModel>(_url, JsonConvert.SerializeObject(request),null,out satusCode,out msg);
                model.HttpStatusCode = satusCode;
                model.HttpStatusMsg = msg;
                return model;
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
            }
            return null;

        }

        /// <summary>
        /// hàm này là hàm xét duyệt của admin (chờ phản hồi call back
        /// </summary>
        /// <param name="Amount"></param>

        /// <returns></returns>
        public static MerchantTransactionsResponseModel MerchantTransactions(double Amount,string Address)
        {
            try
            {
                var request = new MerchantTransactionsRequestModel();
                request.Address = Address;
                request.Amount = Amount;

                
                var _url = "https://api.myusdtwallet.com/merchant-transactions/";
                int statusCode = 0;
                string access_token = Access_Token;
                string msg = string.Empty;
                var model = PostJson<MerchantTransactionsResponseModel>(_url, JsonConvert.SerializeObject(request), access_token,out statusCode,out msg);
                model.HttpStatusCode = statusCode;
                model.HttpMsg = msg;
                return model;
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
            }
            return null;

        }

    }
}