using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using TraditionGame.Utilities;

namespace MsWebGame.Portal.ShopMuaThe
{
    public class ShopMuaTheApi
    {

        public static APIResponse SendGate2(String RefCode, String CardType, string CardCode, String CardSerial, int AmountUser, string AccountName)
        {



            string urlService = ConfigurationManager.AppSettings["Gate2_SHOPMUATHE_URl"].ToString();
            string partnerKey = ConfigurationManager.AppSettings["Gate2_SHOPMUATHE_KEY"].ToString();
            string partnerCode = ConfigurationManager.AppSettings["Gate2_SHOPMUATHE_PARTNERCODE"].ToString();
            string CallBackUrl = ConfigurationManager.AppSettings["Gate2_SHOPMUATHE_CallBackUrl"].ToString();
            string serviceCode = "cardtelco";
            string commandCode = "usecard";

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string requestContent = serializer.Serialize(new UseCardRequest()
            {
                CardSerial = CardSerial,
                CardCode = CardCode,
                CardType = CardType,
                AccountName = AccountName,
                AppCode = "Q",
                RefCode = RefCode,
                AmountUser = AmountUser,
                CallbackUrl = CallBackUrl,
            });
            var signature = Encrypts.MD5(partnerCode + serviceCode + commandCode + requestContent + partnerKey);
            var requestData = new RequestData()
            {
                PartnerCode = partnerCode,
                CommandCode = commandCode,
                RequestContent = requestContent,
                ServiceCode = serviceCode,
                Signature = signature
            };
            var serviceResponse = PostJson(urlService, serializer.Serialize(requestData));
            serviceResponse.Signature = signature;
            // NLogManager.LogMessage("Response" + JsonConvert.SerializeObject(serviceResponse));
            return serviceResponse;
        }
        public static APIResponse SendGate3(String RefCode, String CardType, string CardCode, String CardSerial, int AmountUser, string AccountName)
        {



            string urlService = ConfigurationManager.AppSettings["Gate3_SHOPMUATHE_URl"].ToString();
            string partnerKey = ConfigurationManager.AppSettings["Gate3_SHOPMUATHE_KEY"].ToString();
            string partnerCode = ConfigurationManager.AppSettings["Gate3_SHOPMUATHE_PARTNERCODE"].ToString();
            string CallBackUrl = ConfigurationManager.AppSettings["Gate3_SHOPMUATHE_CallBackUrl"].ToString();
            string serviceCode = "cardtelco";
            string commandCode = "usecard";

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string requestContent = serializer.Serialize(new UseCardRequest()
            {
                CardSerial = CardSerial,
                CardCode = CardCode,
                CardType = CardType,
                AccountName = AccountName,
                AppCode = "Q",
                RefCode = RefCode,
                AmountUser = AmountUser,
                CallbackUrl = CallBackUrl,
            });
            var signature = Encrypts.MD5(partnerCode + serviceCode + commandCode + requestContent + partnerKey);
            var requestData = new RequestData()
            {
                PartnerCode = partnerCode,
                CommandCode = commandCode,
                RequestContent = requestContent,
                ServiceCode = serviceCode,
                Signature = signature
            };
            var serviceResponse = PostJson(urlService, serializer.Serialize(requestData));
            serviceResponse.Signature = signature;
            // NLogManager.LogMessage("Response" + JsonConvert.SerializeObject(serviceResponse));
            return serviceResponse;
        }

        public static  APIResponse Send(String RefCode,String CardType, string CardCode,String CardSerial,int AmountUser,string AccountName )
        {

            
              
            string urlService = ConfigurationManager.AppSettings["SHOPMUATHE_URl"].ToString();
            string partnerKey = ConfigurationManager.AppSettings["SHOPMUATHE_KEY"].ToString();
            string partnerCode = ConfigurationManager.AppSettings["SHOPMUATHE_PARTNERCODE"].ToString();
            string CallBackUrl = ConfigurationManager.AppSettings["SHOPMUATHE_CallBackUrl"].ToString();
            string serviceCode = "cardtelco";
            string commandCode = "usecard";

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string requestContent = serializer.Serialize(new UseCardRequest()
            {
                CardSerial = CardSerial,
                CardCode = CardCode,
                CardType = CardType,
                AccountName = AccountName,
                AppCode = "Q",
                RefCode = RefCode,
                AmountUser = AmountUser,
                CallbackUrl= CallBackUrl,
            });
            var signature = Encrypts.MD5(partnerCode + serviceCode + commandCode + requestContent + partnerKey);
            var requestData = new RequestData()
            {
                PartnerCode = partnerCode,
                CommandCode = commandCode,
                RequestContent = requestContent,
                ServiceCode = serviceCode,
                Signature = signature
            };
            var serviceResponse = PostJson(urlService, serializer.Serialize(requestData));
            serviceResponse.Signature = signature;
           // NLogManager.LogMessage("Response" + JsonConvert.SerializeObject(serviceResponse));
            return serviceResponse;
        }

        private static APIResponse PostJson(string uri, string postData)
        {
            //NLogManager.LogMessage("P2-Postdata:" + postData);
            var card = new APIResponse();
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(uri);
                request.ContentType = "application/json";
                
                 request.Timeout = 1000 * 120;
                request.Method = "POST";//GET
                                        //request.Accept = "JSON";
                using (Stream requestStream = request.GetRequestStream())
                {
                    byte[] postDatabytes = Encoding.UTF8.GetBytes(postData);
                    requestStream.Write(postDatabytes, 0, postDatabytes.Length);
                }
              

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                var  result = new StreamReader(response.GetResponseStream()).ReadToEnd();
                response.Close();
           
                card = JsonConvert.DeserializeObject<APIResponse>(result);

                return card;
            }catch(Exception ex)
            {
                card.ResponseCode = -99;
                card.ResponseContent = "Exception Code";
                NLogManager.LogMessage("STMT -99");
                NLogManager.PublishException(ex);
            }
           
            return card;
        }
    }
}
