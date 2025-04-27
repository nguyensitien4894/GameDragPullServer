using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using TraditionGame.Utilities.MyUSDT.API;
using TraditionGame.Utilities.MyUSDT.Models.Charges;

namespace TraditionGame.Utilities.MyUSDT.Charges
{
    public class BankChargeApiApi : BaseApiRequest
    {

        private static string METHOD = "buy";

        protected static string CALLBACK_ORDER = ConfigurationManager.AppSettings["CALLBACK_BUY_ORDER"].ToString();

        /// <summary>
        /// lấy thông tin rate 
        /// </summary>
        /// <returns></returns>
        public static UsdtRate GetRateConfig()
        {
            try
            {
                string BankUrl = "https://api.myusdtwallet.com/access-token/";

                int statusCode;
                string msg = string.Empty;
                var model = GetJson<UsdtRate>(BankUrl, Access_Token, out statusCode,out msg);
                model.HttpStatusCode = statusCode;
                return model;
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
            }
            return null;
        }

        /// <summary>
        /// Get Danh sach bank Nap
        /// </summary>
        /// <returns></returns>
        public static List<BankCharge> GetBankCharge()
        {
            try
            {
                string BankUrl = BaseUrl + "/v1/banks/";
                int statusCode;
                string msg;
                var model = GetJson<List<BankCharge>>(BankUrl, null, out statusCode,out msg);

                return model;
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
            }
            return null;

        }



        /// <summary>
        /// Hệ thống tạo lệnh mua USDT rồi trả về thông tin chi   tiết chuyển khoản  ạo lệnh MUA USDT
        /// </summary>
        /// <returns></returns>
        public static OrderResponseModel SendBuyOrder(long TargetAmount)
        {
            try
            {
                var request = new OrderRequestModel();
                request.CurrencyPair = CURRENT_PAIR;
                request.Method = METHOD;
                request.TargetAmount = TargetAmount;
                request.Email = EMAIL_BANK;
                request.CallBack = CALLBACK_ORDER;
                request.Note = new OrderRequestNote()
                {
                    ReceiveUsdtAddress = ReceiveUsdtAddress
                };

                var _url = BaseUrl + "/v1/orders/";
                int statusCode = 0;
                string msg = string.Empty;
                var model = PostJson<OrderResponseModel>(_url, JsonConvert.SerializeObject(request), null, out statusCode,out msg);
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

        /// <summary>
        /// Lấy thông tin order details
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static OrderDetailModel GetOrderDeails(string code)
        {
            try
            {


                var _url = BaseUrl + String.Format("/v1/orders/{0}", code);
                int statusCode = 0;
                string msg;
                var model = GetJson<OrderDetailModel>(_url, null, out statusCode,out msg);
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