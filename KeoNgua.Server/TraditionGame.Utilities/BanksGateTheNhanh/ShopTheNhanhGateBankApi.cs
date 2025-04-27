using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Script.Serialization;


namespace TraditionGame.Utilities.BanksGateTheNhanh
{
    public class ShopTheNhanhGateBankApi
    {
        private static string partnerKey = ConfigurationManager.AppSettings["SHOPMUATHE_KEY"].ToString();
        private static string partnerCode = ConfigurationManager.AppSettings["SHOPMUATHE_PARTNERCODE"].ToString();
        private static string CallbackUrl = ConfigurationManager.AppSettings["MOMO_SHOPMUATHE_CallbackUrl"].ToString();
       

        private static string serviceCode = "bankdirect";
        private static string url = "https://bankgate.pianopal.info/VPGJsonService.ashx";
        public static BankAccount GetBanks(string Type)
        {

            try
            {
                string commandCode = "getbanks";

                JavaScriptSerializer serializer = new JavaScriptSerializer();
                string requestContent = serializer.Serialize(new GetBankRequest()
                {
                    Type = Type
                });
                var signature = Encrypts.MD5(partnerCode + serviceCode + commandCode + requestContent + partnerKey);
                var requestData = new UserRequest()
                {
                    PartnerCode = partnerCode,
                    CommandCode = commandCode,
                    RequestContent = requestContent,
                    ServiceCode = serviceCode,
                    Signature = signature
                };

                var serviceResponse = PostJson(url, serializer.Serialize(requestData));
                if (serviceResponse.ResponseCode == "1")
                {
                    List<BankAccount> account = serializer.Deserialize<List<BankAccount>>(serviceResponse.ResponseContent);
                    return account.Any() ? account.FirstOrDefault() : null;
                }

                return null;
            }catch(Exception ex)
            {
                NLogManager.PublishException(ex);
                return null;
            }
           



        }


        public static Order MakeOrder(string  Type,long accountID, string BankName, string BankAccountName, string BankAccountNumber, int Amount, string RefCode)
        {
            try
            {
                string commandCode = "order";

                JavaScriptSerializer serializer = new JavaScriptSerializer();
                string requestContent = serializer.Serialize(new OrderRequest()
                {
                    Type = Type,
                    AccountName = accountID.ToString(),
                    BankName = BankName,
                    BankAccountName = BankAccountName,
                    BankAccountNumber = BankAccountNumber,
                    AppCode = string.Empty,
                    RefCode = RefCode,
                    Amount = Amount,
                    CallbackUrl = CallbackUrl,
                });
                var signature = Encrypts.MD5(partnerCode + serviceCode + commandCode + requestContent + partnerKey);



                var requestData = new UserRequest()
                {
                    PartnerCode = partnerCode,
                    CommandCode = commandCode,
                    RequestContent = requestContent,
                    ServiceCode = serviceCode,
                    Signature = signature
                };



                var serviceResponse = PostJson(url, serializer.Serialize(requestData));
                if (serviceResponse.ResponseCode == "1")
                {
                    Order account = serializer.Deserialize<Order>(serviceResponse.ResponseContent);
                    return account;
                }
                return null;
            }catch(Exception ex)
            {
                NLogManager.PublishException(ex);
                return null;
            }


        }

        private static APIResponse PostJson(string uri, string postData)
        {
            var card = new APIResponse();
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(uri);
                request.ContentType = "application/json";
                request.Method = "POST";//GET
                                        //request.Accept = "JSON";
                using (Stream requestStream = request.GetRequestStream())
                {
                    byte[] postDatabytes = Encoding.UTF8.GetBytes(postData);
                    requestStream.Write(postDatabytes, 0, postDatabytes.Length);
                }


                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                var result = new StreamReader(response.GetResponseStream()).ReadToEnd();
                response.Close();
                card = JsonConvert.DeserializeObject<APIResponse>(result);

                return card;
            }
            catch (Exception ex)
            {
                card.ResponseCode = "-99";
                card.ResponseContent = ex.Message;
            }
            return card;
        }


    }
}
