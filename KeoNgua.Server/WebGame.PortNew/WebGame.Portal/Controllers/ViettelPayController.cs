using MsWebGame.Portal.Database.DAO;
using MsWebGame.Portal.Models.ViettelPay;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using TraditionGame.Utilities;
using TraditionGame.Utilities.Constants;
using TraditionGame.Utilities.Http;
using TraditionGame.Utilities.Messages;
using TraditionGame.Utilities.Session;

namespace MsWebGame.Portal.Controllers
{
    [RoutePrefix("api/ViettelPay")]
    public class ViettelPayController : BaseApiController
    {
        // GET: ViettelPay
        private int MOMO_MAINTAIN = -8;

        private string ViettelPayType = "viettelpay";


        public static ViettelPayInfo GetViettelPayInforRequest()
        {
            //ViettelPayInfo card = new ViettelPayInfo();
            //string url = ConfigurationManager.AppSettings["HDP_VIETTELPAY_CHARGING_URL"].ToString();
            //string merchant_id = ConfigurationManager.AppSettings["HDP_MERCHANT_ID"].ToString();
            //try
            //{
            //    byte[] postBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new ViettelPayRequest()
            //    {
            //        merchant_id = merchant_id
            //    }));

            //    var webrequest = System.Net.HttpWebRequest.Create(url);
            //    webrequest.Method = "POST";
            //    webrequest.ContentType = "application/json";
            //    webrequest.ContentLength = postBytes.Length;

            //    using (var stream = webrequest.GetRequestStream())
            //    {
            //        stream.Write(postBytes, 0, postBytes.Length);
            //        stream.Flush();
            //    }

            //    var sendresponse = webrequest.GetResponse();
            //    string sendresponsetext = "";
            //    using (var streamReader = new StreamReader(sendresponse.GetResponseStream()))
            //    {
            //        sendresponsetext = streamReader.ReadToEnd().Trim();
            //    }

            //    if (!string.IsNullOrEmpty(sendresponsetext))
            //    {
            //        card = JsonConvert.DeserializeObject<ViettelPayInfo>(sendresponsetext);
            //    }
            //    return card;
            //}
            //catch (Exception ex)
            //{
            //    card.code = -1;
            //}
            //return card;

            String result = string.Empty;
            ViettelPayInfo card = new ViettelPayInfo();
            string url = ConfigurationManager.AppSettings["HDP_VIETTELPAY_CHARGING_URL"].ToString();
            string momoApiKey = ConfigurationManager.AppSettings["HDP_MERCHANT_ID"].ToString();
            //string signature = CardVinaHelper.GenerateSignature(requestId, cardNumber, serialNumber, VINA_TELCO, cardValue, secretKey);
            String urlParameter = String.Format("merchant_id={0}", momoApiKey);
            NLogManager.LogMessage("Url Param : " + urlParameter);
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(String.Format("{0}?{1}", url, urlParameter));
                request.KeepAlive = false;
                request.ProtocolVersion = HttpVersion.Version10;
                request.Method = "GET";
                request.ContentType = "application/x-www-form-urlencoded";
                request.UserAgent = "Mozilla/5.0";
                WebHeaderCollection headerReader = request.Headers;
                headerReader.Add("Accept-Language", "en-US,en;q=0.5");
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                result = new StreamReader(response.GetResponseStream()).ReadToEnd();
                response.Close();

                card = JsonConvert.DeserializeObject<ViettelPayInfo>(result);

                return card;

            }
            catch (Exception e)
            {
                card.status = 0;
            }
            return card;

        }
        /// <summary>
        /// Lấy danh sách banks
        /// </summary>
        /// <returns></returns>
        [ActionName("GetInfor")]
        [HttpGet]
        public dynamic GetInfor()
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
                            Message = ErrorMsg.VIETTELPAYLOCK + "-1"
                        };
                    }
                    listP = listPartners.Where(c => c.ViettelPay != "0").Select(c => c.ViettelPay).ToList();
                    if (listP == null || !listP.Any())
                    {
                        return new
                        {
                            ResponseCode = MOMO_MAINTAIN,
                            Message = ErrorMsg.VIETTELPAYLOCK + "-2"
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
                            Message = ErrorMsg.VIETTELPAYLOCK + "-3"
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
                        Message = ErrorMsg.VIETTELPAYLOCK + "-4"
                    };
                }
                var momoHappy = new List<long> { 1, 2 };
                var momoShopTheNhanh = new List<long> { 3, 4 };
                var monoconfig = string.Empty;
                if (momoHappy.Contains(lngValue))
                {
                    monoconfig = VIETTELPAYConfig;

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
                        Message = "Can't find rate configuration"
                    };
                }
                // lngValue = 1 hoac 2 Momo
                if (String.IsNullOrEmpty(monoconfig))
                {
                    return new
                    {
                        ResponseCode = -1019,
                        Message = "Can't find rate configuration"
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
                        Message = ErrorMsg.VIETTELPAYLOCK + "-9",
                    };
                }

                var firstBanks = bankOperator.FirstOrDefault();
                if (firstBanks == null)
                {
                    return new
                    {
                        ResponseCode = MOMO_MAINTAIN,
                        Message = ErrorMsg.VIETTELPAYLOCK + "-10",
                    };
                }
                if (!firstBanks.Status)
                {
                    return new
                    {
                        ResponseCode = MOMO_MAINTAIN,
                        Message = ErrorMsg.VIETTELPAYLOCK + "-11",
                    };
                }
                rate = firstBanks.Rate;
                if (rate <= 0)
                {
                    return new
                    {
                        ResponseCode = MOMO_MAINTAIN,
                        Message = "Failed to get scale configuration",
                    };
                }

                ViettelPayInfo momoInfo = GetViettelPayInforRequest();
                if (momoInfo != null)
                {
                    if (momoInfo.status == 1)
                    {
                        var lsstDisplay = new List<ViettelPayResult> {
                            new ViettelPayResult(10000,rate),
                            new ViettelPayResult(20000,rate),
                            new ViettelPayResult(100000,rate),
                            new ViettelPayResult(200000,rate),
                            new ViettelPayResult(500000,rate),
                            new ViettelPayResult(1000000,rate),
                        };
                        return new
                        {
                            ResponseCode = 1,
                            Orders = new
                            {
                                Message = displayName.ToLower(),// ten hien thi
                                WalletAccountName = momoInfo.info.name,//
                                WalletAccount = momoInfo.info.phone,
                                Rate = rate,
                                List = lsstDisplay,
                            }
                        };
                    }
                    return new
                    {
                        ResponseCode = MOMO_MAINTAIN,
                        Message = ErrorMsg.VIETTELPAYLOCK + "-12",
                    };

                }
                return new
                {
                    ResponseCode = MOMO_MAINTAIN,
                    Message = ErrorMsg.VIETTELPAYLOCK + "-13",
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
}