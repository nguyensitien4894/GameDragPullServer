using MsWebGame.Portal.Lib;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using TraditionGame.Utilities;
using System.Json;
using System.Security.Cryptography;
using System.IO;
using Newtonsoft.Json.Linq;
using MsWebGame.Portal.Database.DAO;
using TraditionGame.Utilities.Log;
using static QRCoder.PayloadGenerator.SwissQrCode.Iban;

namespace MsWebGame.Portal.Models.USDTBanks
{
    public class QienGate
    {
         public static string PartnerCode = "Qien";
        public static TransferInfo TopupBank(int amount, long accountID, string accountName, string bankType)
        {
            StringBuilder log = new StringBuilder();
            TransferInfo transferInfo = new TransferInfo();

            //params
            string ref_id = TraditionGame.Utilities.Security.Security.MD5Encrypt(Guid.NewGuid().ToString());
            var requestBody = BuildRequestBody("bank", ref_id, amount, Lib.Constant.QUIEN_BANK_CALLBACK_URL, bankType);//need checking
            log.AppendLine("requestBody: " + requestBody);
            string checksum = Checksum(Lib.Constant.QUIEN_PRIVATE_KEY, requestBody);
            log.AppendLine("checksum: " + checksum);

            string resString = postRequest(checksum, requestBody);
            log.AppendLine("Request Topup Bank: " + resString);                                                             //need checking

            int insertresutl = 0;
            var rs = JsonConvert.DeserializeObject<TopupResponse>(resString);
            if (rs != null && rs.err_code == 0)
            {
                string requestData = "bank|" + bankType + "|" + amount + "|" + rs.receiver +                                //need checking
                    "|" + rs.receiver_name + "|" + rs.message + "|" + rs.expire_at ;
                insertresutl = TransactionDAO.AddChargeCode(
                    rs.ticket, 
                    (int)Constant.CHARGETYPE.BANK,                                                                          //need checking
                    requestData,
                    accountID, 
                    amount,
                    ref_id,
                    rs.expire_at,
                    PartnerCode
                );
                if (insertresutl == 1)
                {
                    transferInfo.status = 1;
                    transferInfo.data = new QienData(){
                        bankCode = bankType,
                        receiver = rs.receiver,
                        receiverName = rs.receiver_name,
                        message = rs.message,
                        qr_url = rs.qr,
                        iframe_url= rs.url,
                        expire_at = rs.expire_at
                    };
                    transferInfo.description = "success";
                }
            }
            else
            {
                transferInfo.error_code = rs.err_code;
                transferInfo.description = "Hệ thống đang bảo trì! vui lòng quay lại sau";

            }
            if (insertresutl != 1)
            {
                transferInfo.status = -2;
                transferInfo.description = "Hệ thống đang bảo trì! vui lòng quay lại sau";
                //Telegram.Send(
                //    ConfigurationManager.AppSettings["TOKEN_NOTIFY_TELE"],
                //    ConfigurationManager.AppSettings["URGENCY_ERROR_TELEGRAM_GROUP"],
                //    "Kiểm tra hệ thống nạp bank:" + "amount" + amount +
                //    "|accountID:" + accountID +
                //    "|accountName" + accountName +
                //    "|bankCode" + bankCode
                // );
            }
            NLogManager.MomoLog(log.ToString());
            return transferInfo;
        }

        public static TransferInfo TopupMomo(int amount, long accountID)
        {
            StringBuilder log = new StringBuilder();
            TransferInfo transferInfo = new TransferInfo();

            //params
            string ref_id = TraditionGame.Utilities.Security.Security.MD5Encrypt(Guid.NewGuid().ToString());
            var requestBody = BuildRequestBody("momo", ref_id, amount, Lib.Constant.QUIEN_MOMO_CALLBACK_URL);   //need checking
            string checksum = Checksum(Lib.Constant.QUIEN_PRIVATE_KEY, requestBody);
            log.AppendLine("requestBody: " + requestBody);
            log.AppendLine("checksum: " + checksum);
            
            string resString = postRequest(checksum, requestBody);
            log.AppendLine("Request Topup momo: " + resString);                                                 //need checking

            var rs = JsonConvert.DeserializeObject<TopupResponse>(resString);
            int insertresutl = 0;
            if (rs != null && rs.err_code == 0)
            {
                string requestData = "momo|" + amount + "|" + rs.receiver +                                     //need checking
                    "|" + rs.receiver_name + "|" + rs.message + "|" + rs.expire_at;
                insertresutl = TransactionDAO.AddChargeCode(
                    rs.ticket,
                    (int)Constant.CHARGETYPE.MOMO,                                                              //need checking
                    requestData,
                    accountID,
                    amount,
                    ref_id,
                    rs.expire_at,
                    PartnerCode
                );
                if (insertresutl == 1)
                {
                    transferInfo.status = 1;
                    transferInfo.data = new QienData()
                    {
                        receiver = rs.receiver,
                        receiverName = rs.receiver_name,
                        message = rs.message,
                        qr_url = rs.qr,
                        iframe_url = rs.url,
                        expire_at = rs.expire_at
                    };
                    transferInfo.description = "success";
                }
                else
                {
                    transferInfo.status = -2;
                    transferInfo.description = "Hệ thống đang bảo trì! vui lòng quay lại sau";
                }
            }
            else
            {
                transferInfo.error_code = rs.err_code;
                transferInfo.description = "Hệ thống đang bảo trì! vui lòng quay lại sau";

            }
            NLogManager.MomoLog(log.ToString());
            return transferInfo;
        }

        public static TransferInfo TopupViettelPay(int amount, long accountID)
        {
            StringBuilder log = new StringBuilder();
            TransferInfo transferInfo = new TransferInfo();

            //params
            string ref_id = TraditionGame.Utilities.Security.Security.MD5Encrypt(Guid.NewGuid().ToString());
            var requestBody = BuildRequestBody("viettelpay", ref_id, amount, Lib.Constant.QUIEN_VIETTEL_PAY_CALLBACK_URL);  //need checking
            string checksum = Checksum(Lib.Constant.QUIEN_PRIVATE_KEY, requestBody);

            log.AppendLine("requestBody: " + requestBody);
            log.AppendLine("checksum: " + checksum);

            string resString = postRequest(checksum, requestBody);
            log.AppendLine("RequestTopup viettelpay: " + resString);                                                        //need checking

            int insertresutl = 0;
            var rs = JsonConvert.DeserializeObject<TopupResponse>(resString);
            if (rs != null && rs.err_code == 0)
            {
                string requestData = "viettelpay|" + amount + "|" + rs.receiver +                                           //need checking
                    "|" + rs.receiver_name + "|" + rs.message + "|" + rs.expire_at;
                insertresutl = TransactionDAO.AddChargeCode(
                    rs.ticket,
                    (int)Constant.CHARGETYPE.VIETTEL_PAY,                                                                   //need checking
                    requestData,
                    accountID,
                    amount,
                    ref_id,
                    rs.expire_at,
                    PartnerCode
                );
                if (insertresutl == 1)
                {
                    transferInfo.status = 1;
                    transferInfo.data = new QienData()
                    {
                        receiver = rs.receiver,
                        receiverName = rs.receiver_name,
                        message = rs.message,
                        qr_url = rs.qr,
                        iframe_url = rs.url,
                        expire_at = rs.expire_at
                    };
                    transferInfo.description = "success";
                }
                else
                {
                    transferInfo.status = -2;
                    transferInfo.description = "Hệ thống đang bảo trì! vui lòng quay lại sau";
                }
            }
            else
            {
                transferInfo.error_code = rs.err_code;
                transferInfo.description = "Hệ thống đang bảo trì! vui lòng quay lại sau";

            }
            NLogManager.MomoLog(log.ToString());
            return transferInfo;
        }

        public static TransferInfo TopupZalo(int amount, long accountID)
        {
            StringBuilder log = new StringBuilder();
            TransferInfo transferInfo = new TransferInfo();

            //params
            string ref_id = TraditionGame.Utilities.Security.Security.MD5Encrypt(Guid.NewGuid().ToString());
            var requestBody = BuildRequestBody("zalo", ref_id, amount, Lib.Constant.QUIEN_ZALO_CALLBACK_URL);   //need checking
            string checksum = Checksum(Lib.Constant.QUIEN_PRIVATE_KEY, requestBody);
            log.AppendLine("requestBody: " + requestBody);
            log.AppendLine("checksum: " + checksum);

            string resString = postRequest(checksum, requestBody);
            log.AppendLine("RequestTopup zalo: " + resString);                                                  //need checking

            int insertresutl = 0;
            var rs = JsonConvert.DeserializeObject<TopupResponse>(resString);
            if (rs != null && rs.err_code == 0)
            {
                string requestData = "zalo|" + amount + "|" + rs.receiver +                                     //need checking
                    "|" + rs.receiver_name + "|" + rs.message + "|" + rs.expire_at;
                insertresutl = TransactionDAO.AddChargeCode(
                    rs.ticket,
                    (int)Constant.CHARGETYPE.ZALO,                                                              //need checking
                    requestData,
                    accountID,
                    amount,
                    ref_id,
                    rs.expire_at,
                    PartnerCode
                );
                if (insertresutl == 1)
                {
                    transferInfo.status = 1;
                    transferInfo.data = new QienData()
                    {
                        receiver = rs.receiver,
                        receiverName = rs.receiver_name,
                        message = rs.message,
                        qr_url = rs.qr,
                        iframe_url = rs.url,
                        expire_at = rs.expire_at
                    };
                    transferInfo.description = "success";
                }
                else
                {
                    transferInfo.status = -2;
                    transferInfo.description = "Hệ thống đang bảo trì! vui lòng quay lại sau";
                }
            }
            else
            {
                transferInfo.error_code = rs.err_code;
                transferInfo.description = "Hệ thống đang bảo trì! vui lòng quay lại sau";

            }
            NLogManager.MomoLog(log.ToString());
            return transferInfo;
        }

        public static TransferInfo TopupCard(string operatorCode, string cardSerial, string cardCode, int amount, string ref_id,long accountID)
        {
            NLogManager.MomoLog("TOPUP Qien Gate INFO: \r\ncardSerial = " + cardSerial +
                "\r\ncardCode = " + cardCode +
                "\r\ncardType = " + operatorCode +
                "\r\nmenhGia = " + amount +
                "\r\nrequestId = " + ref_id);

            //default type: "VTT", "VNP", "VMS" => viettel, vinaphone, mobifone
            if (operatorCode == "VTT")
            {
                operatorCode = "viettel";
            }
            else if (operatorCode == "VNP")
            {
                operatorCode = "vinaphone";
            }
            else if (operatorCode == "VMS")
            {
                operatorCode = "mobifone";
            }


            StringBuilder log = new StringBuilder();
            TransferInfo transferInfo = new TransferInfo();

            //params
            var json = new JObject();
            json.Add("type", "card");
            json.Add("ref_id", ref_id);
            json.Add("amount", amount);
            json.Add("card_telco", operatorCode);
            json.Add("card_serial", cardSerial);
            json.Add("card_code", cardCode);
            json.Add("callback", Lib.Constant.QUIEN_CARD_CALLBACK_URL);

            var requestBody = json.ToString(Formatting.None);
            string checksum = Checksum(Lib.Constant.QUIEN_PRIVATE_KEY, requestBody);
            log.AppendLine("requestBody: " + requestBody);
            log.AppendLine("checksum: " + checksum);

            string resString = postRequest(checksum, requestBody);
            log.AppendLine("response: " + resString);

            int insertresutl = 0;
            var rs = JsonConvert.DeserializeObject<TopupResponse>(resString);
            if (rs != null && rs.err_code == 0)
            {
                string requestData = "card|" + amount + "|" + operatorCode +                                     //need checking
                    "|" + cardSerial + "|" + cardCode + "|" + rs.expire_at;
                insertresutl = TransactionDAO.AddChargeCode(
                    rs.ticket,
                    (int)Constant.CHARGETYPE.CARD,                                                              //need checking
                    requestData,
                    accountID,
                    amount,
                    ref_id,
                    rs.expire_at,
                    PartnerCode
                );
                if (insertresutl == 1)
                {
                    transferInfo.status = 1;
                    transferInfo.data = new QienData()
                    {
                        receiver = rs.receiver,
                        receiverName = rs.receiver_name,
                        message = rs.message,
                        qr_url = rs.qr,
                        iframe_url = rs.url,
                        expire_at = rs.expire_at,
                        partner_key = rs.ticket

                    };
                    transferInfo.description = "success";
                }
                else
                {
                    transferInfo.status = -2;
                    transferInfo.description = "Hệ thống đang bảo trì! vui lòng quay lại sau";
                }
            }
            else
            {
                transferInfo.error_code = rs.err_code;
                transferInfo.description = "Hệ thống đang bảo trì! vui lòng quay lại sau";

            }
            NLogManager.MomoLog(log.ToString());
            return transferInfo;

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

        public static string BuildRequestBody(string type, string ref_id, int amount, string callbackUrl, string bankType = "")
        {
            var json = new JObject();
            json.Add("type", type);
            json.Add("ref_id", ref_id);
            if (bankType != "")
            {
                json.Add("bank_type", bankType);
            }
            json.Add("amount", amount);
            json.Add("callback", callbackUrl);
            json.Add("qr_inline", true);
            var requestBody = json.ToString(Formatting.None);
            //"{\"type\":\"bank\",\"ref_id\":\"b4a3c57373b80eec3916aa6ec9d2355f\",\"amount\":10000,\"callback\":\"url\"}";

            return requestBody;
        }
        public static string postRequest(string checksum, string requestBody)
        {
            string url = $"https://ezconnectdgp.com/deposit";
            HttpWebRequest webRequest = null;
            webRequest = System.Net.WebRequest.Create(url) as HttpWebRequest;
            webRequest.Method = WebHelper.Method.POST.ToString();
            webRequest.ServicePoint.Expect100Continue = false;
            webRequest.UserAgent = "";
            webRequest.Timeout = 30000;
            webRequest.Headers["APIKEY"] = Lib.Constant.QUIEN_API_KEY;
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

            string rs = Lib.WebHelper.WebResponseGet(webRequest);
            return rs; ;
        }

        public static TransferInfo CashoutBank(
            string ref_id, int amount, long accountID, string bankType, string stk, string name)
        {
            StringBuilder log = new StringBuilder();
            TransferInfo transferInfo = new TransferInfo();

            //params
            var json = new JObject();
            json.Add("ref_id", ref_id);
            json.Add("amount", amount);
            json.Add("type", "bank");
            json.Add("callback", "");
            json.Add("bank_type", bankType);
            json.Add("stk", stk);
            json.Add("name", name);
            var requestBody = json.ToString(Formatting.None);


            log.AppendLine("requestBody: " + requestBody);
            string checksum = Checksum(Lib.Constant.QUIEN_PRIVATE_KEY, requestBody);
            log.AppendLine("checksum: " + checksum);

            string resString = postRequest(checksum, requestBody);
            log.AppendLine("Request Cashout Bank: " + resString);                                                             //need checking

            var rs = JsonConvert.DeserializeObject<TopupResponse>(resString);
            if (rs != null && rs.err_code == 0)
            {
                //Cập nhật trạng thái đã gửi qua đối tác, và chờ đối tác xử lý
                
            }
            else
            {
                //Cập nhật trại thái đã gửi qua đối tác, bị lổi

                transferInfo.error_code = rs.err_code;
                transferInfo.description = "";

            }
            NLogManager.MomoLog(log.ToString());
            return transferInfo;
        }

    }


    public class BankRequestParams
    {
        public string BankCode { get; set; }
        public int Amount { get; set; }
        public string Captcha { get; set; }
        public string PrivateKey { get; set; }
    }

    public class TopupResponse
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
    public class TransferInfo
    {

        public int status { get; set; }
        public QienData data { get; set; }
        public string description { get; set; }

        public int error_code { get; set; }
        
    }

    public class QienData
    {
        public string bankCode { get; set; }
        public string bankName { get; set; }
        public string receiver { get; set; }
        public string receiverName { get; set; }
        public string message { get; set; }
        public string qr_url { get; set; }
        public string iframe_url { get; set; }
        public string expire_at { get; set; }
        public string partner_key { get; set; }

    }


    /// <summary>
    /// respons format from quien gateway
    /// </summary>
    /// <returns></returns>
    public class BankListResponse
    {
        public string err_code { get; set; }
        public string err_msg { get; set; }
        public List<QienBank> banks { get; set; }
    }
    /// <summary>
    /// respons banks format from quien gateway
    /// </summary>
    /// <returns></returns>
    public class QienBank
    {
        public string bank_type { get; set; }
        public string bank_name { get; set; }
        public string bank_logo { get; set; }
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