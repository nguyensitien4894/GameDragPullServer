using MsWebGame.Portal.Database.DAO;
using MsWebGame.Portal.Helpers;
using MsWebGame.Portal.Models.Momo;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using Newtonsoft.Json.Linq;
using TraditionGame.Utilities;
using TraditionGame.Utilities.BanksGateTheNhanh;
using TraditionGame.Utilities.Constants;
using TraditionGame.Utilities.DNA;
using TraditionGame.Utilities.Http;
using TraditionGame.Utilities.Log;
using TraditionGame.Utilities.Messages;
using TraditionGame.Utilities.Momos.Api.Charges;
using TraditionGame.Utilities.MyUSDT.Charges;
using TraditionGame.Utilities.MyUSDT.Models.Charges;
using TraditionGame.Utilities.Session;
using TraditionGame.Utilities.Utils;
using System.Security.Cryptography;
using Microsoft.Ajax.Utilities;
using MsWebGame.Portal.Database.DTO;
using MsWebGame.Portal.Models;
using System.Web.Script.Serialization;
using System.Xml;

namespace MsWebGame.Portal.Controllers
{
    [RoutePrefix("api/MopayBank")]
    public class MopayBankController : BaseApiController
    {
        private int MOMO_MAINTAIN = -8;

        private string MomoType = "momo";

        private MopayListBankResponse GetJsonBankListInfor()
        {
            var accountId = AccountSession.AccountID;

            var displayName = AccountSession.AccountName;
            var objreturn = new MopayListBankResponse();

            objreturn.stt = 1;
            objreturn.msg = displayName.ToLower();
            objreturn.data = new List<BankDataAccount> {  
                        new BankDataAccount { code = "A", name = "Acleda" },
                        new BankDataAccount { code = "B", name = "ABA" },
                        new BankDataAccount { code = "W", name = "Wing" },
                        new BankDataAccount { code = "E", name = "Emoney" },
            };

            return objreturn;
        }

        public static string EncryptAESBase64(string input, string password)
        {
            byte[] salt = {
            73, 118, 97, 110, 32, 77, 101, 100, 118, 101,
            100, 101, 118
        };
            Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(password, salt, 1000);
            byte[] keyBytes = key.GetBytes(32);
            byte[] ivBytes = key.GetBytes(16);
            using (Aes aes = Aes.Create())
            {
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                aes.Key = keyBytes;
                aes.IV = ivBytes;
                using (ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                {
                    byte[] inputBytes = Encoding.Unicode.GetBytes(input);
                    byte[] encryptedBytes = encryptor.TransformFinalBlock(inputBytes, 0, inputBytes.Length);
                    return Convert.ToBase64String(encryptedBytes);
                }
            }
        }

        public static string DecryptAESBase64(string input, string password)
        {
            byte[] salt = {
            73, 118, 97, 110, 32, 77, 101, 100, 118, 101,
            100, 101, 118
        };
            Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(password, salt, 1000);
            byte[] keyBytes = key.GetBytes(32);
            byte[] ivBytes = key.GetBytes(16);
            using (Aes aes = Aes.Create())
            {
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                aes.Key = keyBytes;
                aes.IV = ivBytes;
                using (ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                {
                    byte[] inputBytes = Convert.FromBase64String(input);
                    byte[] decryptedBytes = decryptor.TransformFinalBlock(inputBytes, 0, inputBytes.Length);
                    return Encoding.Unicode.GetString(decryptedBytes);
                }
            }
        }

        private MopayInfoResponse GetJsonMopayInfor(long amount, string chargeType, string subType, string displayName)
        {


            //NLogManager.LogMessage("GetJsonMopayInfor : " + url);
            //try
            //{
            //    var request = (HttpWebRequest)WebRequest.Create(url);
            //    request.ContentType = "application/json";
            //    request.Method = "POST";//GET
            //    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            //    var result = new StreamReader(response.GetResponseStream()).ReadToEnd();
            //    response.Close();
            //    MopayInfoResponse objReturn = JsonConvert.DeserializeObject<MopayInfoResponse>(result);
            //    return objReturn;
            //}
            //catch (Exception ex)
            //{
            //    MopayInfoResponse objReturn = new MopayInfoResponse();
            //    objReturn.stt = -99;
            //    NLogManager.PublishException(ex);
            //    return objReturn;
            //}
            String result = string.Empty;
            long order_id_value;
            //CryptoDAO.Instance.UserCryptoRequestNextIndentity(out order_id_value);
            //string order_id = order_id_value.ToString() + "|" + displayName + "|" + DateTime.Now.ToString("yyMMddh");
            //order_id = CardHelper.Base64Encode(order_id).Replace("=", "");

            string xmlContent = "<?xml version=\"1.0\" encoding=\"utf-8\"?><soap:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\"><soap:Body><QRgen xmlns=\"http://tempuri.org/\"><username>" + EncryptAESBase64("gameslbt", "3170d89f2c3c22bc") + "</username><password>" + EncryptAESBase64("Games$@04810dd1", "3170d89f2c3c22bc") + "</password><bankcode>" + EncryptAESBase64(subType, "3170d89f2c3c22bc") + "</bankcode><fullname>GpARgOGnGNltlDd/hhTQmA==</fullname><trans_amount>" + EncryptAESBase64(amount.ToString(), "3170d89f2c3c22bc") + "</trans_amount><src_id>" + EncryptAESBase64("1111", "3170d89f2c3c22bc") + "</src_id><notify_url>" + EncryptAESBase64("https://callback01052023.luy88.com/api/BankCharge/ReceiveResultPayment1s", "3170d89f2c3c22bc") + "</notify_url></QRgen></soap:Body></soap:Envelope>";

            //var content = new StringContent(xmlContent, Encoding.UTF8, "text/xml");

            string url = "https://payment1s.com/truem/(S(lnsdu3sdry430loahdhrnhvb))/WSeMoney.asmx?op=QRgen";
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";//GET
                request.ContentType = "text/xml";
                //request.UserAgent = "Mozilla/5.0";
                request.UserAgent = "Fiddler";
                WebHeaderCollection headerReader = request.Headers;
                byte[] data = Encoding.UTF8.GetBytes(xmlContent);

                request.ContentLength = data.Length;

                Stream requestStream = request.GetRequestStream();
                requestStream.Write(data, 0, data.Length);
                requestStream.Close();
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                result = new StreamReader(response.GetResponseStream()).ReadToEnd();

                response.Close();

                // Parse the response content as XML
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(result);

                XmlNodeList nodes = xmlDoc.SelectNodes("TransAmount");

                XmlNamespaceManager nsManager = new XmlNamespaceManager(xmlDoc.NameTable);
                nsManager.AddNamespace("soap", "http://schemas.xmlsoap.org/soap/envelope/");
                nsManager.AddNamespace("tempuri", "http://tempuri.org/");

                XmlNode transAmountNode = xmlDoc.SelectSingleNode("//tempuri:TransAmount", nsManager);
                XmlNode FullNameNode = xmlDoc.SelectSingleNode("//tempuri:FullName", nsManager);
                XmlNode BankNameNode = xmlDoc.SelectSingleNode("//tempuri:BankName", nsManager);
                XmlNode BankCodeNode = xmlDoc.SelectSingleNode("//tempuri:BankCode", nsManager);
                XmlNode BankAccountNode = xmlDoc.SelectSingleNode("//tempuri:BankAccount", nsManager);
                XmlNode QRUrlNode = xmlDoc.SelectSingleNode("//tempuri:QRUrl", nsManager);

                string transAmount = transAmountNode.InnerText;
                string FullName = FullNameNode.InnerText;
                string BankName = BankNameNode.InnerText;
                string BankCode = BankCodeNode.InnerText;
                string BankAccount = BankAccountNode.InnerText;
                string QRUrl = QRUrlNode.InnerText;

                if (transAmountNode != null && FullNameNode != null && BankNameNode != null && BankCodeNode != null && BankAccountNode != null && QRUrlNode != null)
                {
                    transAmount = DecryptAESBase64(transAmountNode.InnerText, "3170d89f2c3c22bc");
                    FullName = DecryptAESBase64(FullNameNode.InnerText, "3170d89f2c3c22bc");
                    BankName = DecryptAESBase64(BankNameNode.InnerText, "3170d89f2c3c22bc");
                    BankCode = DecryptAESBase64(BankCodeNode.InnerText, "3170d89f2c3c22bc");
                    BankAccount = DecryptAESBase64(BankAccountNode.InnerText, "3170d89f2c3c22bc");
                    QRUrl = QRUrlNode.InnerText;
                }

                //nowpayment_CryptoListWalletResponse objReturn = JsonConvert.DeserializeObject<nowpayment_CryptoListWalletResponse>(result);
                //return objReturn;

                var accountId = AccountSession.AccountID;
                var objreturn = new MopayInfoResponse();

                objreturn.stt = 1;
                objreturn.type = 1;
                objreturn.msg = AccountSession.AccountName.ToLower() + DateTime.Now.ToString("yyMMddh");
                objreturn.data = new MopayData();
                objreturn.data.qr_url = QRUrl;
                objreturn.data.amount = int.Parse(amount.ToString());
                objreturn.data.code = AccountSession.AccountName.ToLower() + DateTime.Now.ToString("yyMMddh");
                objreturn.data.bank_provider = BankName;
                objreturn.data.id = 100;
                objreturn.data.phoneName = FullName;
                objreturn.data.phoneNum = BankAccount;
                objreturn.data.chargeType = "";
                objreturn.data.payment_url = "";
                objreturn.data.timeToExpired = 19919;
                objreturn.data.redirect = "";


                return objreturn;

                //MopayInfoResponse objReturn = new MopayInfoResponse();
                //objReturn.stt = -99;
                //return objReturn;
            }
            catch (Exception ex)
            {
                MopayInfoResponse objReturn = new MopayInfoResponse();
                objReturn.stt = -99;
                NLogManager.PublishException(ex);
                return objReturn;
            }
        }


        [ActionName("GetListBank")]
        [HttpGet]
        public dynamic GetListBank(string type = "0")
        {
            try
            {
                var isOption = HttpContext.Current.Request.HttpMethod == "OPTIONS";
                if (isOption)
                {
                    return HttpUtils.CreateResponseNonReason(HttpStatusCode.OK, string.Empty);
                }
                var accountId = AccountSession.AccountID;

                var displayName = AccountSession.AccountName;
                if (accountId <= 0 || String.IsNullOrEmpty(displayName))
                {
                    return new
                    {
                        ResponseCode = Constants.UNAUTHORIZED,
                        Message = ErrorMsg.UnAuthorized
                    };
                }
                var tkServiceID = AccountSession.ServiceID;
                if (tkServiceID != ServiceID)
                {
                    return new
                    {
                        ResponseCode = Constants.NOT_IN_SERVICE,
                        Message = ErrorMsg.NOTINSERVICE
                    };
                }
                var account = AccountDAO.Instance.GetProfile(accountId, ServiceID);
                if (account == null)
                {
                    return new
                    {
                        ResponseCode = Constants.UNAUTHORIZED,
                        Message = ErrorMsg.UnAuthorized
                    };
                }
                if (account.Status != 1)
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.AccountLock
                    };
                }
                if (type == "1")
                {

                    double rate = 0;
                    var bankOperator = USDTDAO.Instance.BankOperatorsSecondaryList(ServiceID);
                    if (bankOperator == null || !bankOperator.Any())
                    {
                        return new
                        {
                            ResponseCode = -1006,
                            Message = "Can't find rate configuration",
                        };
                    }
                    var firstBanks = bankOperator.FirstOrDefault();
                    if (firstBanks == null)
                    {
                        return new
                        {
                            ResponseCode = -1006,
                            Message = "Can't find rate configuration",
                        };
                    }
                    rate = firstBanks.Rate;
                    long totalRecord;
                    var list = USDTDAO.Instance.GetListBanks(0, 0, null, null, null, 1, Int16.MaxValue, tkServiceID, out totalRecord);
                    if (list == null || !list.Any())
                    {
                        return new
                        {
                            ResponseCode = -1007,
                            Message = "Can't find rate configuration",
                        };
                    }

                    List<BankDataAccount> mylist = new List<BankDataAccount>();
                    foreach (var bankOperatorsSecondary in bankOperator)
                    {
                        List<Bank> tempBank = list.Where(c => c.BankOperatorsSecondaryID == bankOperatorsSecondary.ID && c.Status).ToList();
                        if (tempBank != null && tempBank.Count > 0)
                        {
                            int dem = 0;
                            var bankobject = new BankDataAccount();
                            foreach (var bank in tempBank)
                            {
                                if (dem == 0)
                                {
                                    bankobject.code = bank.ShortOperatorCode;
                                    bankobject.name = bank.OperatorName;
                                }
                                dem++;
                            }
                            mylist.Add(bankobject);
                        }
                    }
                    return new
                    {
                        ResponseCode = 1,
                        Orders = new
                        {
                            List = mylist,
                            //{

                            //new BankDataAccount { code = "MB", name = "Quân đội MBBank" },
                            //new BankDataAccount { code = "VCB", name = "Vietcombank" },
                            //new BankDataAccount { code = "VTB", name = "VietinBank" },
                            //new BankDataAccount { code = "TPB", name = "TPBank" }
                            //};

                            Message = displayName.ToLower(),
                        }
                    };


                    //string minValue;
                    //ParaConfigDAO.Instance.GetCoreConfig("TRANSLIMIT", "BANK_USDT_LOWER_LIMIT", out minValue);
                    //var min = ConvertUtil.ToLong(minValue);
                    //string maxValue;
                    //ParaConfigDAO.Instance.GetCoreConfig("TRANSLIMIT", "BANK_USDT_UPPER_LIMIT", out maxValue);
                    //var max = ConvertUtil.ToLong(maxValue);
                    //return new
                    //{
                    //    ResponseCode = 1,
                    //    Banks = listBankObject,
                    //    //Rate = rate,
                    //    //Min = 50000,
                    //    //Max = max,
                    //    Message = displayName.ToLower(),
                    //};
                }
                else
                {
                    MopayListBankResponse momoInfo = GetJsonBankListInfor();
                    if (momoInfo != null)
                    {
                        if (momoInfo.stt == 1)
                        {

                            return new
                            {
                                ResponseCode = 1,
                                Orders = new
                                {
                                    List = momoInfo.data,
                                    Message = displayName.ToLower(),
                                }
                            };
                        }
                        return new
                        {
                            ResponseCode = MOMO_MAINTAIN,
                            Message = ErrorMsg.MOMOLOCK + "-12",
                        };

                    }
                }

                return new
                {
                    ResponseCode = MOMO_MAINTAIN,
                    Message = ErrorMsg.MOMOLOCK + "-13",
                };
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return new
                {
                    ResponseCode = -99,
                    Description = ErrorMsg.InProccessException
                };
            }
        }

        [ActionName("GetBank")]
        [HttpGet]
        public dynamic GetBank(string bankCode, string amount, string type = "0")
        {
            try
            {
                NLogManager.LogMessage("GetBank : " + bankCode);
                var isOption = HttpContext.Current.Request.HttpMethod == "OPTIONS";
                if (isOption)
                {
                    return HttpUtils.CreateResponseNonReason(HttpStatusCode.OK, string.Empty);
                }
                var accountId = AccountSession.AccountID;

                var displayName = AccountSession.AccountName;
                if (accountId <= 0 || String.IsNullOrEmpty(displayName))
                {
                    return new
                    {
                        ResponseCode = Constants.UNAUTHORIZED,
                        Message = ErrorMsg.UnAuthorized
                    };
                }
                var tkServiceID = AccountSession.ServiceID;
                if (tkServiceID != ServiceID)
                {
                    return new
                    {
                        ResponseCode = Constants.NOT_IN_SERVICE,
                        Message = ErrorMsg.NOTINSERVICE
                    };
                }
                var account = AccountDAO.Instance.GetProfile(accountId, ServiceID);
                if (account == null)
                {
                    return new
                    {
                        ResponseCode = Constants.UNAUTHORIZED,
                        Message = ErrorMsg.UnAuthorized
                    };
                }
                if (account.Status != 1)
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message = ErrorMsg.AccountLock
                    };
                }
                if (type == "1")
                {
                    string phoneNum = "";
                    string nameBank = "";
                    string OperatorID = "";
                    //switch (bankCode)
                    //{
                    //    case "TPB":
                    //        phoneNum = "00000937490";
                    //        nameBank = "Hoang Thanh Long";
                    //        OperatorID = "11";
                    //        break;
                    //    case "VTB":
                    //        phoneNum = "109877571048";
                    //        nameBank = "Hoang Thanh Long";
                    //        OperatorID = "9";
                    //        break;
                    //    case "VCB":
                    //        phoneNum = "1033509508";
                    //        nameBank = "Hoang Thanh Long";
                    //        OperatorID = "3";
                    //        break;
                    //        //default:
                    //        //    phoneNum = "BẢO TRÌ";
                    //        //    nameBank = "BẢO TRÌ";
                    //        //    break;
                    //}

                    //if (bankCode.ToLower() == "debug")
                    //{
                    var bankOperator = USDTDAO.Instance.BankOperatorsSecondaryList(ServiceID);
                    if (bankOperator == null || !bankOperator.Any())
                    {
                        return new
                        {
                            ResponseCode = -1006,
                            Message = "Can't find rate configuration",
                        };
                    }
                    var firstBanks = bankOperator.FirstOrDefault();
                    if (firstBanks == null)
                    {
                        return new
                        {
                            ResponseCode = -1006,
                            Message = "Can't find rate configuration",
                        };
                    }
                    long totalRecord;
                    var list = USDTDAO.Instance.GetListBanksSecondary(0, 0, bankCode, null, true, 1);
                    if (list == null || !list.Any())
                    {
                        return new
                        {
                            ResponseCode = -1007,
                            Message = "Can't find rate configuration",
                        };
                    }

                    return new
                    {
                        ResponseCode = 1,
                        Orders = new
                        {
                            List = new
                            {
                                phoneNum = list.FirstOrDefault().BankNumber,
                                phoneName = list.FirstOrDefault().BankName,
                                OperatorID = list.FirstOrDefault().BankOperatorsSecondaryID,
                                code = displayName + DateTime.Now.ToString("yyMMddh"),
                            },
                            Message = displayName + DateTime.Now.ToString("yyMMddh"),
                        }
                    };
                    //}

                    //return new
                    //{
                    //    ResponseCode = 1,
                    //    Orders = new
                    //    {
                    //        List = new {
                    //            phoneNum = phoneNum,
                    //            phoneName = nameBank,
                    //            OperatorID = OperatorID,
                    //            code = displayName + DateTime.Now.ToString("yyMMddh"),
                    //        },
                    //        Message = displayName + DateTime.Now.ToString("yyMMddh"),
                    //    }
                    //};
                }
                else
                {
                    MopayInfoResponse momoInfo = GetJsonMopayInfor(long.Parse(amount), "bank", bankCode, displayName.ToLower());
                    if (momoInfo != null)
                    {
                        if (momoInfo.stt == 1)
                        {

                            return new
                            {
                                ResponseCode = 1,
                                Type = momoInfo.type,
                                Orders = new
                                {
                                    List = momoInfo.data,
                                    Message = displayName.ToLower(),
                                }
                            };
                        }
                        return new
                        {
                            ResponseCode = MOMO_MAINTAIN,
                            Message = ErrorMsg.MOMOLOCK + "-12",
                        };

                    }
                }

                return new
                {
                    ResponseCode = MOMO_MAINTAIN,
                    Message = ErrorMsg.MOMOLOCK + "-13",
                };
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return new
                {
                    ResponseCode = -99,
                    Description = ErrorMsg.InProccessException
                };
            }
        }
    }

    public class BankDataAccount
    {
        public string code { get; set; }
        public string name { get; set; }
    }

    public class MopayListBankResponse
    {
        public int stt { get; set; }
        public string msg { get; set; }
        public List<BankDataAccount> data { get; set; }
    }

    public class MopayData
    {
        public int id { get; set; }
        public string qr_url { get; set; }
        public string payment_url { get; set; }
        public string code { get; set; }
        public string phoneNum { get; set; }
        public int amount { get; set; }
        public string phoneName { get; set; }
        public string chargeType { get; set; }
        public string redirect { get; set; }
        public string bank_provider { get; set; }
        public int timeToExpired { get; set; }
    }

    public class MopayInfoResponse
    {
        public int type { get; set; }
        public int stt { get; set; }
        public string msg { get; set; }
        public MopayData data { get; set; }
    }
}