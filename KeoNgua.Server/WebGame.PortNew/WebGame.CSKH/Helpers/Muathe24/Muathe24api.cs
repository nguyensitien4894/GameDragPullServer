using MsTraditionGame.Utilities.Log;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Script.Serialization;

namespace MsWebGame.CSKH.Helpers.Muathe24
{
    public class Muathe24api
    {
        //private static string _domainMuaThe = " https://banthe247.com/";
         private static string _domainMuaThe = "https://apidinh.hunghapay.com/";

        public static string Login(string username, string password)
        {
            try
            {

                var url = String.Format("{0}/v2/PayCard/DangNhap?userName={1}&password={2}", _domainMuaThe, username, password);
                var client = new RestClient(url);
                var request = new RestRequest(Method.POST);
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11;
                request.AddHeader("ContentType", "charset=UTF-8");
                request.AddHeader("Cache-Control", "no-cache");
                request.AddHeader("Accept", "*/*");
                IRestResponse response = client.Execute(request);
                var token = response.Content.ToString().Replace("\n", "").Replace("\b", "");
                JavaScriptSerializer json_serializer = new JavaScriptSerializer();
                var strResult = (string)json_serializer.DeserializeObject(token);
                return strResult;
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
            }
            return null;

        }
        public static MuatheResult BuyCard(string token, string cardType, long CardValue, int totalBuyNo)
        {
            MuatheResult returnCards = new MuatheResult();
            try
            {

                var url = String.Format("{0}/v2/PayCards/TelcoPay/GetCards?msg={1}:{2}:{3}", _domainMuaThe, cardType, CardValue, totalBuyNo);
                var client = new RestClient(url);
                var request = new RestRequest(Method.POST);
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11;
                request.AddHeader("ContentType", "charset=UTF-8");
                request.AddHeader("Token", token);
                request.AddHeader("Cache-Control", "no-cache");
                request.AddHeader("Accept", "*/*");

                IRestResponse response = client.Execute(request);
                var strhtml = response.Content.ToString().Replace("\n", "").Replace("\b", "");

                
                NLogManager.LogMessage(String.Format("MuaThe: {0} ngày {0}", strhtml, DateTime.Now));
                JavaScriptSerializer json_serializer = new JavaScriptSerializer();
                var reuslt = (Dictionary<string, object>)json_serializer.DeserializeObject(strhtml);
                if (reuslt == null || !reuslt.Any())
                {
                    returnCards.ErrorCode = -1009;
                    returnCards.Message = "Không lấy được kết quả khi mua thẻ";
                    return returnCards;
                };

                object errorCode = -1000;
                reuslt.TryGetValue("errorCode", out errorCode);
                object rtMsg = string.Empty;
                reuslt.TryGetValue("message", out rtMsg);
                var lngStatus = Convert.ToInt64(errorCode);

                returnCards.ErrorCode = lngStatus;
                returnCards.Message = rtMsg.ToString();
                if (lngStatus == 0)
                {
                    object data = string.Empty;
                    reuslt.TryGetValue("Data", out data);
                    var lstcard = JsonConvert.DeserializeObject<List<CardBuy>>(data.ToString());
                    returnCards.LstCards = lstcard;
                }

            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                returnCards.ErrorCode = -99;
                returnCards.Message = ex.Message;

            }
            return returnCards;

        }
    }
}