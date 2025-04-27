using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Newtonsoft.Json;
using System.Net;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Configuration;
using TraditionGame.Utilities.Log;
using MsTraditionGame.Utilities.Log;

namespace MsWebGame.CSKH.Models
{
    public class BankPartnersModel
    {
        public long Id { get; set; }

        public string PartnerName { get; set; }

        public string Momo { get; set; }

        public string Bank { get; set; }

        public int? Status { get; set; }

        public int ServiceID { get; set; }
    }

    public class QienGate
    {
        public static string postRequest(string checksum, string requestBody)
        {
            string url = $"https://ezconnectdgp.com/out";
            HttpWebRequest webRequest = null;
            webRequest = System.Net.WebRequest.Create(url) as HttpWebRequest;
            webRequest.Method = "POST";
            webRequest.ServicePoint.Expect100Continue = false;
            webRequest.UserAgent = "";
            webRequest.Timeout = 30000;
            webRequest.Headers["APIKEY"] = ConfigurationManager.AppSettings["QUIEN_API_KEY"];
            webRequest.Headers["Checksum"] = checksum;
            webRequest.ContentType = "application/json"; ;

            StreamWriter requestWriter = new StreamWriter(webRequest.GetRequestStream());
            try
            {
                requestWriter.Write(requestBody);
            }
            catch
            {
                throw;
            }
            finally
            {
                requestWriter.Close();
                requestWriter = null;
            }

            StreamReader responseReader = null;
            string responseData = "";

            try
            {
                responseReader = new StreamReader(webRequest.GetResponse().GetResponseStream());
                responseData = responseReader.ReadToEnd();
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                webRequest.GetResponse().GetResponseStream().Close();
                responseReader.Close();
                responseReader = null;
            }

            return responseData;
        }
        public static string Checksum(string keyString, string requestBody)
        {
            var data = Encoding.UTF8.GetBytes(requestBody);
            var key = Encoding.UTF8.GetBytes(keyString);
            // Create HMAC-MD5 Algorithm;
            var hmac = new HMACMD5(key);
            // Compute hash.
            var hashBytes = hmac.ComputeHash(data);
            // Convert to HEX string.
            var checksum = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            return checksum;
        }
        public static CashoutResponse CashoutBank(
            string ref_id, long? amount, long accountID, string bankType, string stk, string name)
        {
            StringBuilder log = new StringBuilder();
            try
            {
                //params
                var json = new JObject();
                json.Add("ref_id", ref_id);
                json.Add("amount", amount);
                json.Add("type", "bank");
                json.Add("callback", ConfigurationManager.AppSettings["QUIEN_CASHOUT_BANK_CALLBACK_URL"]);
                json.Add("bank_type", bankType);
                json.Add("stk", stk);
                json.Add("name", name);
                var requestBody = json.ToString(Formatting.None);


                log.AppendLine("requestBody: " + requestBody);
                string checksum = Checksum(ConfigurationManager.AppSettings["QUIEN_PRIVATE_KEY"], requestBody);
                log.AppendLine("checksum: " + checksum);
                string resString = postRequest(checksum, requestBody);
                log.AppendLine("Response: " + resString);
                var rs = JsonConvert.DeserializeObject<CashoutResponse>(resString);
                return rs;
            }
            catch (Exception ex)
            {
                log.Append("Error CashoutBank: " + ex.ToString());
                return new CashoutResponse()
                {
                    err_code = -1,
                    err_msg = "Không gửi được cho đối tác"
                };
            }
            finally {
                NLogManager.MomoLog(log.ToString());
            }          
        }

        public static CashoutResponse CashoutMomo(string ref_id, long? amount, string phoneNumber, string name)
        {
            StringBuilder log = new StringBuilder();
            log.Append("************************Gửi thông tin cashout momo qua đối tác Qien*********************");
            try
            {
                //params
                var json = new JObject();
                json.Add("ref_id", ref_id);
                json.Add("amount", amount);
                json.Add("type", "momo");
                json.Add("callback", ConfigurationManager.AppSettings["QUIEN_CASHOUT_MOMO_CALLBACK_URL"]);
                json.Add("receiver", phoneNumber);
                json.Add("name", name);
                var requestBody = json.ToString(Formatting.None);


                log.AppendLine("requestBody: " + requestBody);
                string checksum = Checksum(ConfigurationManager.AppSettings["QUIEN_PRIVATE_KEY"], requestBody);
                log.AppendLine("checksum: " + checksum);
                string resString = postRequest(checksum, requestBody);
                log.AppendLine("Response: " + resString);
                var rs = JsonConvert.DeserializeObject<CashoutResponse>(resString);
                return rs;
            }
            catch (Exception ex)
            {
                log.Append("Error CashoutBank: " + ex.ToString());
                return new CashoutResponse()
                {
                    err_code = -1,
                    err_msg = "Không gửi được cho đối tác"
                };
            }
            finally
            {
                NLogManager.MomoLog(log.ToString());
            }
        }
    }


    public class CashoutResponse
    {
        public string type { get; set; }
        public string message { get; set; }
        public string url { get; set; }
        public string qr { get; set; }
        public string receiver { get; set; }
        public int err_code { get; set; }
        public string err_msg { get; set; }

        public string ticket { get; set; }
        public string ref_id { get; set; }
        public string bank_type { get; set; }
        public string callback { get; set; }
        public string receiver_name { get; set; }
        public string expire_at { get; set; }

    }
    public class QienCallbackParams
    {
        public int err_code { get; set; }
        public string err_msg { get; set; }
        public string type { get; set; }
        public string trans_id { get; set; }
        public string ref_id { get; set; }
        public string ticket { get; set; }
        public string bank_number { get; set; }
        public string bank_type { get; set; }
        public string sender { get; set; }
        public string receiver { get; set; }
        public long amount { get; set; }
        public string message { get; set; }
        public string timestamp { get; set; }

        public long amount_real { get; set; }

        public long amount_add { get; set; }

        public bool done { get; set; }

        public int IntergerChargeType() //"bank" or "momo" or "viettelpay" or "zalo" or "card"
        {
            switch (type) //Lib.Contans.CHARGETYPE
            {
                case "bank":
                    return 1;
                case "momo":
                    return 2;
                case "viettelpay":
                    return 3;
                case "zalo":
                    return 4;
                case "card":
                    return 5;
                default: return 0;
            }
        }

    }


}