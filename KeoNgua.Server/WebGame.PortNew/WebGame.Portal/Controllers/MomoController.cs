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
using TraditionGame.Utilities.Api;
using System.Security.Cryptography;
using System.Web.Http.Results;
using MsWebGame.Portal.Models;
using System.Security.Policy;

namespace MsWebGame.Portal.Controllers
{
    [RoutePrefix("api/Momo")]
    public class MomoController : BaseApiController
    {
        private int MOMO_MAINTAIN = -8;

        private string MomoType = "momo";
        private static MomoInfoObject PostJsonContent(string sdt, string pHash)
        {
            string sdtmomo = string.Empty;
            string taikhoanmomo = string.Empty;
            long sodutk = 0;
            try
            {
                string url = "https://owa.momo.vn/public/login";
                MomoSendObject root = new MomoSendObject();
                root.user = sdt;
                root.msgType = "USER_LOGIN_MSG";
                root.pass = "123456";
                root.cmdId = "1606757712706000000";
                root.lang = "vi";
                root.time = 1606757712706;
                root.channel = "APP";
                root.appVer = 30011;
                root.appCode = "3.0.1";
                root.deviceOS = "ANDROID";
                root.result = true;
                root.errorCode = 0;
                root.errorDesc = "";
                root.momoMsg = new MomoMsg()
                {
                    _class = "mservice.backend.entity.msg.LoginMsg",
                    isSetup = false
                };
                root.extra = new Extra()
                {
                    pHash = pHash
                };
                byte[] postBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(root));

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

                if (!string.IsNullOrEmpty(sendresponsetext))
                {
                    JObject momo = JObject.Parse(sendresponsetext);
                    sdtmomo = momo["extra"]["originalPhone"].ToString();
                    taikhoanmomo = momo["extra"]["FULL_NAME"].ToString();
                    sodutk = long.Parse(momo["extra"]["BALANCE"].ToString());
                    return new MomoInfoObject()
                    {
                        MomoPhoneNumber = sdtmomo,
                        MomoUserName = taikhoanmomo,
                        MomoBalance = sodutk
                    };
                }
                NLogManager.LogError("Get momo info json IsNullOrEmpty ");
                return null;
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return null;
            }
        }

        public static string MD5(string data)
        {
            UTF8Encoding encoding1 = new UTF8Encoding();
            MD5CryptoServiceProvider provider1 = new MD5CryptoServiceProvider();
            byte[] buffer1 = encoding1.GetBytes(data);
            byte[] buffer2 = provider1.ComputeHash(buffer1);
            return BitConverter.ToString(buffer2).Replace("-", "").ToLower();
        }

        public int RND()
        {
            int _min = 1000;
            int _max = 9999;
            Random _rdm = new Random();
            return _rdm.Next(_min, _max);
        }

        private MomoInforNew GetJsonMomoInfor(long amount, string accountName)
        {
            //return new
            //{
            //    ResponseCode = 1,
            //    Orders = new
            //    {
            //        List = new
            //        {
            //            phoneNum = phoneNum,
            //            phoneName = nameBank,
            //            OperatorID = OperatorID,
            //            code = displayName + DateTime.Now.ToString("yyMMddh"),
            //        },
            //        Message = displayName + DateTime.Now.ToString("yyMMddh"),
            //    }
            //};

            var code = accountName + DateTime.Now.ToString("yyMMddh");

            string MOMO_NUMBER, MOMO_NAME;
            ParaConfigDAO.Instance.GetCoreConfig("ADDON_MOMO", "MOMO_NUMBER", out MOMO_NUMBER);
            ParaConfigDAO.Instance.GetCoreConfig("ADDON_MOMO", "MOMO_NAME", out MOMO_NAME);

            string result = "{ \"stt\":1, \"msg\":\"OK\", \"data\":{ \"id\":2, \"qr_url\": \"https://chart.googleapis.com/chart?cht=qr&chs=300x300&chl=momo_charging\", \"payment_url\": \"https://chart.googleapis.com/chart?cht=qr&chs=300x300&chl=momo_charging\", \"code\":\"" + code + "\", \"phoneNum\": \"" + MOMO_NUMBER + "\", \"amount\": 10000, \"phoneName\": \"" + MOMO_NAME + "\", \"chargeType\": \"visa\", \"redirect\": \"/payment/201200\", \"bank_provider\": \"MOMO\", \"timeToExpired\": 600 }}";

            MomoInforNew objReturn = JsonConvert.DeserializeObject<MomoInforNew>(result);
            return objReturn;


            //string apiKey = "4e344611-b324-47b1-9bb8-df353da48d03";
            //string signKey = "dh3287dy239fjh2398fu";
            //string rnd = RND().ToString();

            //string signCode = MD5(amount + "momo" + accountName + "_" + rnd + signKey);

            //string url = "http://cucku.net:10007/api/MM/RegCharge?apiKey=" + apiKey+ "&chargeType=momo&amount="+ amount+"&requestId="+ accountName + "_" + rnd + "&callback=" + "https://callback01052023.sicbet.net/api/Momo/MomoCallBackResultAction" + "&sign=" + signCode;
            //try
            //{
            //    var request = (HttpWebRequest)WebRequest.Create(url);
            //    request.ContentType = "application/json";
            //    request.Method = "GET";//GET
            //    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            //    var result = new StreamReader(response.GetResponseStream()).ReadToEnd();
            //    response.Close();
            //    NLogManager.LogMessage("GetJsonMopayInfor : " + result);
            //    NLogManager.LogMessage("url : " + url);
            //    MomoInforNew objReturn = JsonConvert.DeserializeObject<MomoInforNew>(result);
            //    return objReturn;
            //}
            //catch (Exception ex)
            //{
            //    MomoInforNew objReturn = new MomoInforNew();
            //    objReturn.stt = -99;
            //    NLogManager.PublishException(ex);
            //    return objReturn;
            //}
        }


        public static MomoInfoResponse GetMomoInforRequest()
        {
            String result = string.Empty;
            MomoInfoResponse card = new MomoInfoResponse();
            string url = ConfigurationManager.AppSettings["MOMO_CHARGING_URL"].ToString();
            string momoApiKey = ConfigurationManager.AppSettings["MOMO_API_KEY_MM"].ToString();
            //string signature = CardVinaHelper.GenerateSignature(requestId, cardNumber, serialNumber, VINA_TELCO, cardValue, secretKey);
            String urlParameter = String.Format("{0}", momoApiKey);
            NLogManager.LogMessage("Url Param : " + urlParameter);
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(String.Format("{0}{1}", url, urlParameter));

                var postData = string.Empty;
                var data = Encoding.ASCII.GetBytes(postData);

                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = data.Length;

                using (var stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }

                var response = (HttpWebResponse)request.GetResponse();
                if (response != null)
                {
                    var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                    card = JsonConvert.DeserializeObject<MomoInfoResponse>(responseString);
                }
                else
                {
                    card.status = 0;
                }
            }
            catch (Exception e)
            {
                card.status = 0;
            }
            return card;
        }
        //[ActionName("GetInfor")]
        //[HttpGet]
        //public dynamic GetInfor()
        //{
        //    try
        //    {
        //        var isOption = HttpContext.Current.Request.HttpMethod == "OPTIONS";
        //        if (isOption)
        //        {
        //            return HttpUtils.CreateResponseNonReason(HttpStatusCode.OK, string.Empty);
        //        }
        //        var accountId = AccountSession.AccountID;

        //        var displayName = AccountSession.AccountName;
        //        if (accountId <= 0 || String.IsNullOrEmpty(displayName))
        //        {
        //            return new
        //            {
        //                ResponseCode = Constants.UNAUTHORIZED,
        //                Message = ErrorMsg.UnAuthorized
        //            };
        //        }
        //        var tkServiceID = AccountSession.ServiceID;
        //        if (tkServiceID != ServiceID)
        //        {
        //            return new
        //            {
        //                ResponseCode = Constants.NOT_IN_SERVICE,
        //                Message = ErrorMsg.NOTINSERVICE
        //            };
        //        }
        //        var account = AccountDAO.Instance.GetProfile(accountId, ServiceID);
        //        if (account == null)
        //        {
        //            return new
        //            {
        //                ResponseCode = Constants.UNAUTHORIZED,
        //                Message = ErrorMsg.UnAuthorized
        //            };
        //        }
        //        if (account.Status != 1)
        //        {
        //            return new
        //            {
        //                ResponseCode = -1005,
        //                Message = ErrorMsg.AccountLock
        //            };
        //        }
        //        //random key
        //        long lngValue;
        //        try
        //        {
        //            var listP = new List<string>();
        //            var listPartners = BankPartnersDAO.Instance.GetList(ServiceID);
        //            if (listPartners == null || !listPartners.Any())
        //            {
        //                return new
        //                {
        //                    ResponseCode = MOMO_MAINTAIN,
        //                    Message = ErrorMsg.MOMOLOCK + "-1"
        //                };
        //            }
        //            listP = listPartners.Where(c => c.Momo != "0").Select(c => c.Momo).ToList();
        //            if (listP == null || !listP.Any())
        //            {
        //                return new
        //                {
        //                    ResponseCode = MOMO_MAINTAIN,
        //                    Message = ErrorMsg.MOMOLOCK + "-2"
        //                };
        //            }
        //            var strRandom = String.Join(",", listP);
        //            var listAcitve = strRandom.Split(',');
        //            var ListIntActive = listAcitve.Select(long.Parse).ToList().Where(c => c > 0).ToList();
        //            if (ListIntActive == null || !ListIntActive.Any())
        //            {
        //                return new
        //                {
        //                    ResponseCode = MOMO_MAINTAIN,
        //                    Message = ErrorMsg.MOMOLOCK + "-3"
        //                };
        //            }
        //            Random rndActives = new Random();
        //            var intRandom = rndActives.Next(0, ListIntActive.Count);
        //            lngValue = ListIntActive[intRandom];
        //        }
        //        catch
        //        {
        //            return new
        //            {
        //                ResponseCode = MOMO_MAINTAIN,
        //                Message = ErrorMsg.MOMOLOCK + "-4"
        //            };
        //        }
        //        var momoHappy = new List<long> { 1, 2 };
        //        var momoShopTheNhanh = new List<long> { 3, 4 };
        //        var monoconfig = string.Empty;
        //        if (momoHappy.Contains(lngValue))
        //        {
        //            monoconfig = MOMOConfig;

        //        }
        //        else if (momoShopTheNhanh.Contains(lngValue))
        //        {
        //            monoconfig = MOMOConfigShopTheNhanh;
        //        }
        //        else
        //        {
        //            return new
        //            {
        //                ResponseCode = -1009,
        //                Message = "Không lấy được cấu hình tỉ lệ"
        //            };
        //        }
        //        // lngValue = 1 hoac 2 Momo
        //        if (String.IsNullOrEmpty(monoconfig))
        //        {
        //            return new
        //            {
        //                ResponseCode = -1019,
        //                Message = "Không lấy được cấu hình tỉ lệ"
        //            };
        //        }
        //        //lấy ra rate  trong dbs 
        //        double rate = 0;
        //        var bankOperator = USDTDAO.Instance.BankOperatorsList(ServiceID, monoconfig);
        //        if (bankOperator == null || !bankOperator.Any())
        //        {
        //            return new
        //            {
        //                ResponseCode = MOMO_MAINTAIN,
        //                Message = ErrorMsg.MOMOLOCK + "-9",
        //            };
        //        }

        //        var firstBanks = bankOperator.FirstOrDefault();
        //        if (firstBanks == null)
        //        {
        //            return new
        //            {
        //                ResponseCode = MOMO_MAINTAIN,
        //                Message = ErrorMsg.MOMOLOCK + "-10",
        //            };
        //        }
        //        if (!firstBanks.Status)
        //        {
        //            return new
        //            {
        //                ResponseCode = MOMO_MAINTAIN,
        //                Message = ErrorMsg.MOMOLOCK + "-11",
        //            };
        //        }
        //        rate = firstBanks.Rate;
        //        if (rate <= 0)
        //        {
        //            return new
        //            {
        //                ResponseCode = MOMO_MAINTAIN,
        //                Message = "Không tìm được cấu hình tỉ giá",
        //            };
        //        }

        //        MomoInfoResponse momoInfo = GetMomoInforRequest();
        //        if (momoInfo != null)
        //        {
        //            if (momoInfo.status == 1)
        //            {
        //                var lsstDisplay = new List<MomoResult> {
        //                    new MomoResult(10000,rate),
        //                    new MomoResult(20000,rate),
        //                    new MomoResult(100000,rate),
        //                    new MomoResult(200000,rate),
        //                    new MomoResult(500000,rate),
        //                    new MomoResult(1000000,rate),
        //                };
        //                return new
        //                {
        //                    ResponseCode = 1,
        //                    Orders = new
        //                    {
        //                        Message = displayName.ToLower(),// ten hien thi
        //                        WalletAccountName = momoInfo.name,//
        //                        WalletAccount = momoInfo.phone,
        //                        Rate = rate,
        //                        List = lsstDisplay,
        //                    }
        //                };
        //            }
        //            return new
        //            {
        //                ResponseCode = MOMO_MAINTAIN,
        //                Message = ErrorMsg.MOMOLOCK + "-12",
        //            };

        //        }
        //        return new
        //        {
        //            ResponseCode = MOMO_MAINTAIN,
        //            Message = ErrorMsg.MOMOLOCK + "-13",
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        NLogManager.PublishException(ex);
        //        return new
        //        {
        //            ResponseCode = -99,
        //            Description = ErrorMsg.InProccessException
        //        };
        //    }
        //}


        [ActionName("RegCharge")]
        [HttpPost]
        public dynamic RegCharge([FromBody] Momo input)
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
                if (input == null)
                {
                    return new
                    {
                        ResponseCode = -1006,
                        Message = "The system pauses this function. Please come back later",
                    };
                }
                string captcha = input.Captcha ?? string.Empty;//lấy ra capcha
                if ((String.IsNullOrEmpty(input.PrivateKey) || string.IsNullOrEmpty(captcha)))
                {

                    return new
                    {
                        ResponseCode = -4,
                        Message = ErrorMsg.CapchaRequired
                    };
                }
                else
                {
                    if (CaptchaCache.Instance.VerifyCaptcha(captcha, input.PrivateKey) < 0)//kiểm tra mã cap cha <0 error
                    {
                        return new
                        {
                            ResponseCode = -100,
                            Message = ErrorMsg.InValidCaptCha
                        };
                    }
                }
                string NoiDung = input.NoiDung ?? string.Empty;//lấy ra noi dung ck
                if (string.IsNullOrEmpty(NoiDung))
                {
                    return new
                    {
                        ResponseCode = -1006,
                        Message = "The system pauses this function. Please come back later",
                    };
                }
                else
                {
                    //displayName + DateTime.Now.ToString("yyMMddh") // nickname2301133
                    if (String.Compare(NoiDung.ToLower(), (displayName + DateTime.Now.ToString("yyMMddh")).ToLower()) != 0)
                    {
                        return new
                        {
                            ResponseCode = -100,
                            Message = ErrorMsg.InvalidNote
                        };
                    }
                }

                string minValue;
                ParaConfigDAO.Instance.GetCoreConfig("TRANSLIMIT", "MOMO_LOWER_LIMIT", out minValue);
                var min = ConvertUtil.ToLong(minValue);
                string maxValue;
                ParaConfigDAO.Instance.GetCoreConfig("TRANSLIMIT", "MOMO_UPPER_LIMIT", out maxValue);
                var max = ConvertUtil.ToLong(maxValue);
                //string Amount = input.Amount ?? string.Empty; //Tiền VND
                var lngAmount = ConvertUtil.ToLong(input.Amount);
                if (lngAmount <= 0)
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message = String.Format("Invalid amount {0}", lngAmount),
                    };
                }
                if (lngAmount < min)
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message = String.Format(ErrorMsg.MinAmountTranfer, min.LongToMoneyFormat()),
                    };
                }
                if (lngAmount > max)
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message = String.Format("Maximum amount to be traded {0}", max.LongToMoneyFormat()),
                    };
                }
                //string opID = input.OperatorID ?? string.Empty;
                //int operatorID = ConvertUtil.ToInt(input.OperatorID);
                if (String.IsNullOrEmpty(input.Receive_MomoPhoneNumber))
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message = String.Format("Invalid MOMO phone number: ", input.Receive_MomoPhoneNumber),
                    };
                }
                if (String.IsNullOrEmpty(input.Receive_MomoHolderName))
                {
                    return new
                    {
                        ResponseCode = -1005,
                        Message = String.Format("Invalid MOMO account name: ", input.Receive_MomoHolderName),
                    };
                }

                //double AmountRt = 0;
                //string CodeRt = "";
                //double RemainRt = 0;
                //long TimeOutRt = 0;
                //double AmountReceivedRt = 0;
                //string BankNameRt = string.Empty;
                //string MasterBankAccountRt = string.Empty;
                //string MasterBankNameRt = string.Empty;

                //{

                //    //lấy ra rate  trong dbs 
                double rate = 0;

                //    var bankOperator = USDTDAO.Instance.BankOperatorsSecondaryList(ServiceID);
                //    if (bankOperator == null || !bankOperator.Any())
                //    {
                //        return new
                //        {
                //            ResponseCode = -1006,
                //            Message = "Hệ thống tạm dừng chức năng này. Xin quay lại sau",
                //        };
                //    }
                //    var firstBanks = bankOperator.FirstOrDefault();
                //    if (firstBanks == null)
                //    {
                //        return new
                //        {
                //            ResponseCode = -1006,
                //            Message = "Hệ thống tạm dừng chức năng này. Xin quay lại sau",
                //        };
                //    }

                //    if (!firstBanks.Status)
                //    {
                //        return new
                //        {
                //            ResponseCode = -1006,
                //            Message = "Hệ thống tạm dừng chức năng này. Xin quay lại sau",
                //        };
                //    }
                rate = 1;
                if (rate <= 0)
                {
                    return new
                    {
                        ResponseCode = -1006,
                        Message = "Can't find rate configuration",
                    };
                }
                ///Lấy ra phí cấu hình
                var dbMoney = Math.Round(lngAmount * rate);
                var AmountReceived = Convert.ToInt64(dbMoney);//Tiền Game
                //var description = String.Format("User:{0} nạp {1}", displayName, lngAmount);
                var description = String.Format("Code: {0} | Deposit to MOMO account: {1}", NoiDung,input.Receive_MomoHolderName);
                int Response;
                long RemainBalance;
                long RequestID;
                long TransID;
                //Khởi tạo order trong dbs 
                int PartnerID = 1;
                //USDTDAO.Instance.UserBankRequestChargeCreate(RequestType, accountId, lngAmount, 0, AmountReceived, PENDING_STATUS, PartnerID
                //, ServiceID, description, null, null, null, rate, out Response, out RemainBalance, out RequestID, out TransID);
                MOMODAO.Instance.UserMomoRequestInsert(accountId, 1, PENDING_STATUS, PartnerID, ServiceID, DateTime.Now, DateTime.Now, 1, 0, lngAmount, AmountReceived, 0, accountId, input.Receive_MomoPhoneNumber, null, null, null, null, null, null, null,null, description, out Response, out RequestID);
                if (Response != 1 || RequestID <= 0)
                {
                    return new
                    {
                        ResponseCode = -1006,
                        Message = "Unable to initialize order. Please come back later " + ". Code: 936" + ". Response:" + Response,
                    };
                }

                //MOMODAO.Instance.UserMomoRequestPartnerCheck(
                //        acount.AccountID, RequestType, chargeAmount, PartnerID, null,
                //        "1", PartnerErrorCode,
                //        trimRequestID, null,
                //        null, momoTransId, momoTransId, signature, out Response, out RequestID, out ReceivedMoney, out RemainBalance, out RequestRate, out outServiceID
                //    );
                //NLogManager.LogMessage(acount.AccountID + "|" + RequestType + "|");

                //if (Response == 1)
                //{
                //    //var dnaHelper = new DNAHelpers(outServiceID, DNA_PLATFORM);
                //    //dnaHelper.SendTransactionPURCHASE(acount.AccountID, 10, null, amount, amount);
                //    LoggerHelper.LogUSDTMessage(String.Format("DNA>accountId:{0}|transType:{1}|amount:{2}|amountGame:{3}|requestID:{4}", acount.AccountID, 7, chargeAmount, chargeAmount, RequestID));
                //    string msg = "Tài khoản " + acount.AccountName + " Nạp Momo số tiền :" + chargeAmount;
                //    //SendTelePush(msg,8);
                //    SendChargingHub(acount.AccountID, RemainBalance, "Nạp Momo thành công " + ReceivedMoney, 1, ServiceID);
                //    return JsonMomoResult(0, "Success");

                //}
                //else
                //{
                //    NLogManager.LogMessage(String.Format("MOMODB ERROR:UserID:{0}|ERROR:{1}", acount.AccountID, Response));
                //    return JsonMomoResult(0, "Success");
                //}




                return new
                {
                    ResponseCode = 1,
                    Orders = new
                    {
                        Amount = lngAmount,//Số tiền
                        AmountReceived = 0,
                        Content = displayName,
                        //Banks = new
                        //{
                        //    BankName = "BIDV",//Tên ngân hàng như vietcombank,bidv
                        //    MasterBankAccount = "1234567890",//Số tài khoản cần chuyển vào
                        //    MasterBankName = "NGUYEN VAN A"//Tên chủ tài khoản cần chuyển tiền vào
                        //}
                    }
                };

            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return new
                {
                    ResponseCode = -99,
                    Description = "The system is busy, please come back later"
                };
            }
        }

        [ActionName("GetInfor")]
        [HttpGet]
        public dynamic GetInfor(long amount)
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
                //random key
                long lngValue;
                try
                {
                    var listP = new List<string>();
                    var listPartners = BankPartnersDAO.Instance.GetList(ServiceID);
                    if (listPartners == null || !listPartners.Any())
                    {
                        return new
                        {
                            ResponseCode = MOMO_MAINTAIN,
                            Message = ErrorMsg.MOMOLOCK + "-1"
                        };
                    }
                    listP = listPartners.Where(c => c.Momo != "0").Select(c => c.Momo).ToList();
                    if (listP == null || !listP.Any())
                    {
                        return new
                        {
                            ResponseCode = MOMO_MAINTAIN,
                            Message = ErrorMsg.MOMOLOCK + "-2"
                        };
                    }
                    var strRandom = String.Join(",", listP);
                    var listAcitve = strRandom.Split(',');
                    var ListIntActive = listAcitve.Select(long.Parse).ToList().Where(c => c > 0).ToList();
                    if (ListIntActive == null || !ListIntActive.Any())
                    {
                        return new
                        {
                            ResponseCode = MOMO_MAINTAIN,
                            Message = ErrorMsg.MOMOLOCK + "-3"
                        };
                    }
                    Random rndActives = new Random();
                    var intRandom = rndActives.Next(0, ListIntActive.Count);
                    lngValue = ListIntActive[intRandom];
                }
                catch
                {
                    return new
                    {
                        ResponseCode = MOMO_MAINTAIN,
                        Message = ErrorMsg.MOMOLOCK + "-4"
                    };
                }
                var momoHappy = new List<long> { 1, 2 };
                var momoShopTheNhanh = new List<long> { 3, 4 };
                var monoconfig = string.Empty;
                if (momoHappy.Contains(lngValue))
                {
                    monoconfig = MOMOConfig;

                }
                else if (momoShopTheNhanh.Contains(lngValue))
                {
                    monoconfig = MOMOConfigShopTheNhanh;
                }
                else
                {
                    return new
                    {
                        ResponseCode = -1009,
                        Message = "Failed to get scale configuration"
                    };
                }
                // lngValue = 1 hoac 2 Momo
                if (String.IsNullOrEmpty(monoconfig))
                {
                    return new
                    {
                        ResponseCode = -1019,
                        Message = "Failed to get scale configuration"
                    };
                }
                //lấy ra rate  trong dbs 
                double rate = 0;
                var bankOperator = USDTDAO.Instance.BankOperatorsList(ServiceID, monoconfig);
                if (bankOperator == null || !bankOperator.Any())
                {
                    return new
                    {
                        ResponseCode = MOMO_MAINTAIN,
                        Message = ErrorMsg.MOMOLOCK + "-9",
                    };
                }

                var firstBanks = bankOperator.FirstOrDefault();
                if (firstBanks == null)
                {
                    return new
                    {
                        ResponseCode = MOMO_MAINTAIN,
                        Message = ErrorMsg.MOMOLOCK + "-10",
                    };
                }
                if (!firstBanks.Status)
                {
                    return new
                    {
                        ResponseCode = MOMO_MAINTAIN,
                        Message = ErrorMsg.MOMOLOCK + "-11",
                    };
                }
                rate = firstBanks.Rate;
                if (rate <= 0)
                {
                    return new
                    {
                        ResponseCode = MOMO_MAINTAIN,
                        Message = "Can't find rate configuration",
                    };
                }

                MomoInforNew momoInfo = GetJsonMomoInfor(amount, displayName);
                if (momoInfo != null)
                {
                    if (momoInfo.stt == 1)
                    {
                        var lsstDisplay = new List<MomoResult> {
                            new MomoResult(10000,rate),
                            new MomoResult(20000,rate),
                            new MomoResult(100000,rate),
                            new MomoResult(200000,rate),
                            new MomoResult(500000,rate),
                            new MomoResult(1000000,rate),
                            new MomoResult(10000000,rate),
                            new MomoResult(100000000,rate),
                        };
                        return new
                        {
                            ResponseCode = 1,
                            Orders = new
                            {
                                // Message = displayName.ToLower(),// ten hien thi
                                Message = momoInfo.data.code,
                                WalletAccountName = momoInfo.data.phoneName,//
                                Amount = momoInfo.data.amount,
                                WalletAccount = momoInfo.data.phoneNum,
                                Rate = rate,
                                List = lsstDisplay,
                            }
                        };
                    }
                    return new
                    {
                        ResponseCode = MOMO_MAINTAIN,
                        Message = ErrorMsg.MOMOLOCK + "-12",
                    };

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

    public class Data
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

    public class MomoInforNew
    {
        public int stt { get; set; }
        public string msg { get; set; }
        public Data data { get; set; }
    }
}