using MsWebGame.Portal.Database.DAO;
using MsWebGame.RedisCache.Cache;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;
using TraditionGame.Utilities;
using TraditionGame.Utilities.Constants;
using TraditionGame.Utilities.Messages;
using TraditionGame.Utilities.Session;
using System.Security.Cryptography;
using System.Text;
namespace MsWebGame.Portal.Controllers
{
    [RoutePrefix("api/GS")]
    public class GSController : BaseApiController
    {



        public string opcode = "bocv";
        [HttpGet]
        [Route("GetAccountInfoLive69")]
        public dynamic GetAccountInfoLive69(string displayName)
        {
            try
            {
                displayName = "69live" + displayName;
                var request = (HttpWebRequest)WebRequest.Create("http://gsapi.32828w.com/aio/seamless.verialma/launchgame/getGameUrl");
                request.ContentType = "application/json";
                request.Method = "POST";//GET
                                        //request.Accept = "JSON";
                using (Stream requestStream = request.GetRequestStream())
                {
                    var signature = MD5("bocv123123456ELLC" + displayName + "a13fa80912d69093429f45dd7440940c");
                    var json = new
                    {
                        opcode = "bocv",
                        product = "EL",
                        type = "LC",
                        username = displayName,
                        memberId = "123",
                        password = "123456",
                        signature = signature,
                        lang = "vi-VN",
                        html5 = "0",
                        gameid = "0"
                    };
                    byte[] postDatabytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(json));
                    requestStream.Write(postDatabytes, 0, postDatabytes.Length);
                }


                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                var result = new StreamReader(response.GetResponseStream()).ReadToEnd();
                response.Close();
                dynamic parseresult = JsonConvert.DeserializeObject(result);

                //if (displayName == "oknha11")
                //{
                //    return new
                //    {
                //        ResponseCode = 1,
                //        URL = "https://play.go88.live/livestream/taixiulive.html?v=67",
                //    };
                //}
                if (parseresult.code == 0)
                {
                    return new
                    {
                        ResponseCode = 1,
                        URL = parseresult.url + "&?playsinline=1&autoplay=1",
                    };
                }
                else
                {
                    return new
                    {
                        ResponseCode = -99,
                        URL = "",
                    };
                }



            }
            catch (Exception ex)
            {

                NLogManager.PublishException(ex);
                return new
                {
                    ResponseCode = -99,
                    Message = ErrorMsg.InProccessException
                };
            }

        }
        [HttpPost]
        [Route("GetAccountInfo")]
        public dynamic GetAccountInfo()
        {
            try
            {
                var displayName = AccountSession.AccountName;
                var request = (HttpWebRequest)WebRequest.Create("http://gsapi.32828w.com/aio/seamless.verialma/launchgame/getGameUrl");
                request.ContentType = "application/json";
                request.Method = "POST";//GET
                                        //request.Accept = "JSON";
                using (Stream requestStream = request.GetRequestStream())
                {
                    var signature = MD5("bocv123123456ELLC" + displayName + "a13fa80912d69093429f45dd7440940c");
                    var json = new
                    {
                        opcode = "bocv",
                        product = "EL",
                        type = "LC",
                        username = displayName,
                        memberId = "123",
                        password = "123456",
                        signature = signature,
                        lang = "vi-VN",
                        html5 = "0",
                        gameid = "0"
                    };
                    byte[] postDatabytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(json));
                    requestStream.Write(postDatabytes, 0, postDatabytes.Length);
                }


                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                var result = new StreamReader(response.GetResponseStream()).ReadToEnd();
                response.Close();
                dynamic parseresult = JsonConvert.DeserializeObject(result);

                //if (displayName == "oknha11")
                //{
                //    return new
                //    {
                //        ResponseCode = 1,
                //        URL = "https://play.go88.live/livestream/taixiulive.html?v=67",
                //    };
                //}
                if (parseresult.code == 0)
                {
                    return new
                    {
                        ResponseCode = 1,
                        URL = parseresult.url + "&?playsinline=1&autoplay=1",
                    };
                }
                else
                {
                    return new
                    {
                        ResponseCode = -99,
                        URL = "",
                    };
                }
                

                
            }
            catch (Exception ex)
            {

                NLogManager.PublishException(ex);
                return new
                {
                    ResponseCode = -99,
                    Message = ErrorMsg.InProccessException
                };
            }

        }
        [HttpPost]
        [Route("callback")]
        public dynamic callback(dynamic data)
        {
            try
            {
                NLogManager.LogMessage("DATAEVO | " + JsonConvert.SerializeObject(data));
                if (data.method == "GetUserBalance") // lấy thông tin tài khoản
                {
                    string accountname = data.userId;
                    var info = AccountDAO.Instance.GetAccountInfo2(0, accountname, null, ServiceID);
                    if (info == null)
                    {
                        return new
                        {
                           code = "104",
                        };
                    }
                    return new
                    {
                        code = "0",
                        opcode = opcode,
                        userId = info.AccountName,
                        balance = info.Balance
                    };
                }
                if (data.method == "Bet") // Cược
                {
                    string accountname = data.userId;
                    
                    var info = AccountDAO.Instance.GetAccountInfo2(0, accountname, null, ServiceID);
                    data.AccountID = info.AccountID;
                    if (info == null)
                    {
                        return new
                        {
                            code = "104",
                        };
                    }
                    long Balance = 0;
                    int code = 104;

                    EsportsDAO.Instance.EvoBet(data, out Balance, out code);

                    return new
                    {
                        code = code + "",
                        opcode = opcode,
                        userId = info.AccountName,
                        balance = Balance,
                        balancebefore = info.Balance
                    };
                }
                if (data.method == "Result") // kết quả
                {
                    string accountname = data.userId;

                    var info = AccountDAO.Instance.GetAccountInfo2(0, accountname, null, ServiceID);
                    data.AccountID = info.AccountID;
                    if (info == null)
                    {
                        return new
                        {
                           code = "104",
                        };
                    }
                    long Balance = 0;
                    int code = 104;

                    EsportsDAO.Instance.EvoUpdateResult(data, out Balance, out code);

                    return new
                    {
                        code = code + "",
                        opcode = opcode,
                        userId = info.AccountName,
                        balance = Balance,
                        balancebefore = info.Balance
                    };
                }

                return new
                {
                    code = "104",
                };
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return new
                {
                   code = "104",
                };
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
        [HttpGet]
        [Route("callback1")]
        public dynamic callback1(dynamic data)
        {
            try
            {
                NLogManager.LogMessage(JsonConvert.SerializeObject(data));
                return (new
                {
                    code = 0,
                    opcode = "bocv",
                    userId = "test",
                    balance = 952490
                });
            }
            catch (Exception ex)
            {

                NLogManager.PublishException(ex);
                return new
                {
                    ResponseCode = -99,
                    Message = ErrorMsg.InProccessException
                };
            }

        }
    }  
}