using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using MsWebGame.CSKH.Helpers.Muathe24;
using Newtonsoft.Json;
using RestSharp;
using MsTraditionGame.Utilities.Log;
using Newtonsoft.Json.Linq;

namespace MsWebGame.CSKH.Helpers.MuaTheHDP
{
    public class MuaTheHDPApi
    {
        public static BuyCardHDPResult BuyCard(string cardType, long CardValue, int totalBuyNo)
        {
            BuyCardHDPResult returnCards = new BuyCardHDPResult();
            try
            {


                string json = GetCardRequest(cardType, CardValue, totalBuyNo);
                NLogManager.LogMessage(String.Format("MuaThe: {0} ngày {0}", json, DateTime.Now));
                BuyCardHDPResult result = JsonConvert.DeserializeObject<BuyCardHDPResult>(json);
                if (result == null)
                {
                    returnCards.status = -1009;
                    returnCards.msg = "Không lấy được kết quả khi mua thẻ";
                    return returnCards;
                }
                if (result.status != 1)
                {
                    returnCards.status = result.status;
                    returnCards.msg = result.msg;
                    return returnCards;
                }
                returnCards.status = result.status;
                returnCards.msg = result.msg;
                returnCards.cards = result.cards;


            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                returnCards.status = -99;
                returnCards.msg = ex.Message;
            }
            return returnCards;

        }

        public static string GetCardRequest(string cardType, long CardValue, int totalBuyNo)
        {
            string url = ConfigurationManager.AppSettings["HDP_BUYCARD_URL"].ToString();
            string merchant_id = ConfigurationManager.AppSettings["HDP_MERCHANT_ID"].ToString();
            string postjson = new JObject(new JProperty("merchant_id", merchant_id), new JProperty("cardtype", cardType), new JProperty("cardvalue", CardValue.ToString()), new JProperty("quantity", totalBuyNo.ToString())).ToString();
            byte[] postBytes = Encoding.UTF8.GetBytes(postjson);
            var webrequest = System.Net.HttpWebRequest.Create(url);
            webrequest.Method = "POST";
            webrequest.ContentType = "application/json";
            webrequest.ContentLength = postBytes.Length;

            using (var stream = webrequest.GetRequestStream())
            {
                stream.Write(postBytes, 0, postBytes.Length);
                stream.Flush();
            }
            var sendresponse = webrequest.GetResponse();
            string sendresponsetext = "";
            using (var streamReader = new StreamReader(sendresponse.GetResponseStream()))
            {
                sendresponsetext = streamReader.ReadToEnd().Trim();
            }
            return sendresponsetext;
        }
    }

    public class CardBuyHDP
    {
        public string cardtype { get; set; }
        public string cardseri { get; set; }
        public string cardcode { get; set; }
        public int cardvalue { get; set; }
        public string cardvexpdate { get; set; }
    }

    public class BuyCardHDPResult
    {
        public int status { get; set; }
        public string msg { get; set; }
        public List<CardBuyHDP> cards { get; set; }
        public string transaction_id { get; set; }
    }
}