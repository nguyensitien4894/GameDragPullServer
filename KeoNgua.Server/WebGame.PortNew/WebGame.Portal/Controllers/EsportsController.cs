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
    [RoutePrefix("api/Esports")]
    public class EsportsController : BaseApiController
    {
        public string channel_key = "HkhNY4xtUTX8b2Jm";
        [HttpPost]
        [Route("GetAccountInfo")]
        public dynamic GetAccountInfo()
        {
            try
            {
                var displayName = AccountSession.AccountName;
                EsportsDataBalanceNew objReturn = Esports.GetBalance(displayName);
                return new
                {
                    ResponseCode = 1,
                    Message = "",
                    SabeCoin = objReturn.balance/1000,
                    //CasinoCoin = objReturn.GetCasinoCoin()
                };
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
        [Route("GetLink")]
        public dynamic GetLink()
        {
            try
            {
                string displayName = AccountSession.AccountName.ToLower();
                string sign = MD5("bocv" + displayName + "a13fa80912d69093429f45dd7440940c").ToUpper();
                string url = "http://gslog.336699bet.com/createMember.aspx?operatorcode=bocv&username=" + displayName + "&signature=" + sign;
                NLogManager.Debug(url);
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.ContentType = "application/json";
                request.Method = "GET";//GET
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                var result = new StreamReader(response.GetResponseStream()).ReadToEnd();
                response.Close();

                int device = AccountSession.DeviceType;
                int html5 = device == 1 ? 0 : 1;
                if (displayName == "oknha11")
                {
                    html5 = 1;
                }
                sign = MD5("bocv123456IBSB" + displayName + "a13fa80912d69093429f45dd7440940c").ToUpper();
                url = "http://gslog.336699bet.com/launchGames.aspx?operatorcode=bocv&providercode=IB&username=" + displayName + "&password=123456&type=SB&gameid=0&lang=vi-VN&html5="+ html5 + "&signature=" + sign;
                request = (HttpWebRequest)WebRequest.Create(url);
                request.ContentType = "application/json";
                request.Method = "GET";//GET
                response = (HttpWebResponse)request.GetResponse();
                result = new StreamReader(response.GetResponseStream()).ReadToEnd();
                response.Close();
                NLogManager.Debug(result);
                EsDataUrl objReturn = JsonConvert.DeserializeObject<EsDataUrl>(result);
                if (objReturn != null && objReturn.errCode == 0)
                {

                    return new
                    {
                        ResponseCode = 1,
                        Message = "",
                        SabeLink = new string[] { objReturn.gameUrl, objReturn.gameUrl } ,
                        CasinoLink = new string[] { objReturn.gameUrl },
                    };
                }

            }
            catch (Exception ex)
            {
              
                NLogManager.PublishException(ex);
               
            }
            return new
            {
                ResponseCode = -99,
                Message = ErrorMsg.InProccessException
            };
        }
        [HttpPost]
        [Route("Deposit")]
        public dynamic Deposit(EsPostData esPost)
        {
            //return new
            //{
            //    ResponseCode = -100,
            //    Message = "Chức năng đang bảo trì"
            //};
            try
            {
                var displayName = AccountSession.AccountName.ToLower();

                long accountId = AccountSession.AccountID;

                string keyHu = CachingHandler.Instance.GeneralRedisKey("UsersLive", "Config:" + accountId);

                try
                {
                    MsWebGame.Portal.Models.ParConfigLiveSport parConfigLiveSport = new Models.ParConfigLiveSport();
                    RedisCacheProvider _cachePvd = new RedisCacheProvider();

                    if (_cachePvd.Exists(keyHu))
                    {

                        parConfigLiveSport = _cachePvd.Get<MsWebGame.Portal.Models.ParConfigLiveSport>(keyHu);
                        if (parConfigLiveSport != null && parConfigLiveSport.Game.Count > 0 && parConfigLiveSport.Game.Contains(parConfigLiveSport.GameId))
                        {
                            return new
                            {
                                ResponseCode = 0,
                                Message = "Tài khoản bị hạn chế"
                            };
                        }
                    }
                    
                }
                catch (Exception ex)
                {
                    
                }

                if (accountId <= 0 || String.IsNullOrEmpty(displayName))
                {
                    return new
                    {
                        ResponseCode = Constants.UNAUTHORIZED,
                        Message = ErrorMsg.UnAuthorized
                    };
                }
                long balance = 0;
                long safebalance = 0;

                AccountDAO.Instance.GetBalance(accountId, out balance, out safebalance);

                if (balance >= esPost.Amount)
                {
                    if (esPost.Amount >= 100000)
                    {
                        var coin = Math.Floor(esPost.Amount * 0.0008);
                        var realamount = coin / 0.0008;
                        var money = coin * 1000;
                        long reid = LongRandom(1000000000, 9999999999999, new Random());

                        string sign = MD5(money + "bocv123456IB" + reid + "0" + displayName + "a13fa80912d69093429f45dd7440940c").ToUpper();
                        
                        string url = "http://gslog.336699bet.com/makeTransfer.aspx?operatorcode=bocv&providercode=IB&password=123456&referenceid="+ reid + "&type=0&amount=" + money + "&username=" + displayName + "&signature=" + sign;

                        var request = (HttpWebRequest)WebRequest.Create(url);
                        request.ContentType = "application/json";
                        request.Method = "GET";//GET
                        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                        var result = new StreamReader(response.GetResponseStream()).ReadToEnd();
                        response.Close();
                        NLogManager.Debug(url);
                        NLogManager.Debug(result);
                        EsResponse objReturn = JsonConvert.DeserializeObject<EsResponse>(result);
                        if (objReturn != null)
                        {
                            if (objReturn.errCode == 0)
                            {
                               
                                long Balance = 0;
                                int response_id = 0;
                                EsportsDAO.Instance.Esports_Deposit_To_Game(esPost.GetWallet(), accountId, Convert.ToInt64(realamount), 1, out Balance, out response_id);
                                if (response_id == 1)
                                {
                                    EsportsDataBalanceNew esportsData = Esports.GetBalance(displayName);

                                    //string msg = "[Deposit] Tài khoản : " + displayName + " Vừa nạp " + esPost.Amount + " Vào ví , Số dư ví:" + esportsData.GetSabeCoin() + " Số dư game:" + Balance;
                                    //SendTelePush(msg, 10);


                                    return new
                                    {
                                        ResponseCode = 1,
                                        Message = "Giao dịch thành công!",
                                        Balance = Balance,
                                        EsportBalance = esportsData.balance / 1000
                                    };
                                }
                                else
                                {
                                    return new
                                    {
                                        ResponseCode = -1,
                                        Message = "Có lỗi xảy ra. Liên hệ với admin: "+ response_id//"Có lỗi xảy ra. Liên hệ với admin :200",
                                    };
                                }
                            }
                            else
                            {
                                return new
                                {
                                    ResponseCode = -1,
                                    Message = "Có lỗi xảy ra. Liên hệ với admin"//"Có lỗi xảy ra. Liên hệ với admin :200",
                                };
                            }
                            
                        }
                        else
                        {
                            return new
                            {
                                ResponseCode = 1,
                                Message = result//"Có lỗi xảy ra. Liên hệ với admin Code :100",
                            };
                        }
                    }
                    else
                    {
                        return new
                        {
                            ResponseCode = -1,
                            Message = "Số tiền giao dịch phải từ 100K BOC trở lên",
                        };
                    }
                }
                else
                {
                    return new
                    {
                        ResponseCode = -1,
                        Message = "Số tiền của bạn không đủ để thực hiện giao dịch",
                    };
                }
                return new
                {
                    ResponseCode = -99,
                    Message = ErrorMsg.InProccessException
                };
            }
            catch (Exception ex)
            {

                NLogManager.PublishException(ex);
                return new
                {
                    ResponseCode = -99,
                    Message = ex.Message
                };
            }
           
        }
        long LongRandom(long min, long max, Random rand)
        {
            long result = rand.Next((Int32)(min >> 32), (Int32)(max >> 32));
            result = (result << 32);
            result = result | (long)rand.Next((Int32)min, (Int32)max);
            return result;
        }
        public static string MD5(string data)
        {
            UTF8Encoding encoding1 = new UTF8Encoding();
            MD5CryptoServiceProvider provider1 = new MD5CryptoServiceProvider();
            byte[] buffer1 = encoding1.GetBytes(data);
            byte[] buffer2 = provider1.ComputeHash(buffer1);
            return BitConverter.ToString(buffer2).Replace("-", "").ToLower();
        }
        [HttpPost]
        [Route("Withdrawal")]
        public dynamic Withdrawal(EsPostData esPost)
        {
            //return new
            //{
            //    ResponseCode = -100,
            //    Message = "Chức năng đang bảo trì"
            //};
            try
            {
                var displayName = AccountSession.AccountName.ToLower();

                long accountId = AccountSession.AccountID;
                string keyHu = CachingHandler.Instance.GeneralRedisKey("UsersLive", "Config:" + accountId);

                try
                {
                    MsWebGame.Portal.Models.ParConfigLiveSport parConfigLiveSport = new Models.ParConfigLiveSport();
                    RedisCacheProvider _cachePvd = new RedisCacheProvider();

                    if (_cachePvd.Exists(keyHu))
                    {

                        parConfigLiveSport = _cachePvd.Get<MsWebGame.Portal.Models.ParConfigLiveSport>(keyHu);
                        if (parConfigLiveSport != null && parConfigLiveSport.Game.Count > 0 && parConfigLiveSport.Game.Contains(parConfigLiveSport.GameId))
                        {
                            return new
                            {
                                ResponseCode = 0,
                                Message = "Tài khoản bị hạn chế"
                            };
                        }
                    }

                }
                catch (Exception ex)
                {

                }
                if (accountId <= 0 || String.IsNullOrEmpty(displayName))
                {
                    return new
                    {
                        ResponseCode = Constants.UNAUTHORIZED,
                        Message = ErrorMsg.UnAuthorized
                    };
                }
                EsportsDataBalanceNew Return = Esports.GetBalance(displayName);
                bool Oke = false;
                double Coin = 0;
                double Amount = 0;
          
                if (Return.balance/1000 >= esPost.Amount)
                {
                    Oke = true;
                    Coin = esPost.Amount;
                    Amount = esPost.GetAmountSabaToGame();
                }
        
                if (Oke)
                {

                    var coin = Math.Floor(esPost.Amount * 0.0008);
                    var realamount = esPost.Amount * 1000;
                    long reid = LongRandom(1000000000, 9999999999999, new Random());

                    string sign = MD5(realamount + "bocv123456IB" + reid + "1" + displayName + "a13fa80912d69093429f45dd7440940c").ToUpper();

                    string url = "http://gslog.336699bet.com/makeTransfer.aspx?operatorcode=bocv&providercode=IB&password=123456&referenceid=" + reid + "&type=1&amount=" + realamount + "&username=" + displayName + "&signature=" + sign;

                    var request = (HttpWebRequest)WebRequest.Create(url);
                    request.ContentType = "application/json";
                    request.Method = "GET";//GET
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    var result = new StreamReader(response.GetResponseStream()).ReadToEnd();
                    response.Close();
                    NLogManager.Debug(url);
                    NLogManager.Debug(result);
                    EsResponse objReturn = JsonConvert.DeserializeObject<EsResponse>(result);
                    if (objReturn != null)
                    {
                        if (objReturn.errCode == 0)
                        {

                            long Balance = 0;
                            int response_id = 0;
                            EsportsDAO.Instance.SP_Esports_Withdraw_To_Game(esPost.GetWallet(), accountId, Convert.ToInt64(Amount), 1, out Balance, out response_id);
                            if (response_id == 1)
                            {
                                EsportsDataBalanceNew esportsData = Esports.GetBalance(displayName);

                                //string msg = "[Deposit] Tài khoản : " + displayName + " Vừa nạp " + esPost.Amount + " Vào ví , Số dư ví:" + esportsData.GetSabeCoin() + " Số dư game:" + Balance;
                                //SendTelePush(msg, 10);


                                return new
                                {
                                    ResponseCode = 1,
                                    Message = "Giao dịch thành công!",
                                    Balance = Balance,
                                    EsportBalance = esportsData.balance/1000
                                };
                            }
                            else
                            {
                                return new
                                {
                                    ResponseCode = -1,
                                    Message = "Có lỗi xảy ra. Liên hệ với admin: " + response_id//"Có lỗi xảy ra. Liên hệ với admin :200",
                                };
                            }
                        }
                        else
                        {
                            return new
                            {
                                ResponseCode = -1,
                                Message = "Có lỗi xảy ra. Liên hệ với admin"//"Có lỗi xảy ra. Liên hệ với admin :200",
                            };
                        }

                    }
                    else
                    {
                        return new
                        {
                            ResponseCode = 1,
                            Message = result//"Có lỗi xảy ra. Liên hệ với admin Code :100",
                        };
                    }
                }
                
                else
                {
                    return new
                    {
                        ResponseCode = 1,
                        Message = "Tiền của bạn không đủ để thực hiện giao dịch!",
                    };
                }

                return new
                {
                    ResponseCode = -99,
                    Message = ErrorMsg.InProccessException
                };

            }
            catch (Exception ex)
            {

                NLogManager.PublishException(ex);
                return new
                {
                    ResponseCode = -99,
                    Message = ex.Message
                };

            }

        }
    }
    public class Esports
    {
        private static string channel_key = "HkhNY4xtUTX8b2Jm";
        public static EsportsDataBalanceNew GetBalance(string displayName)
        {
            try
            {
                displayName = displayName.ToLower();
                string sign = MD5("bocv"+ displayName + "a13fa80912d69093429f45dd7440940c").ToUpper();
                string url = "http://gslog.336699bet.com/createMember.aspx?operatorcode=bocv&username="+ displayName + "&signature="+ sign;
                NLogManager.Debug(url);
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.ContentType = "application/json";
                request.Method = "GET";//GET
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                var result = new StreamReader(response.GetResponseStream()).ReadToEnd();
                response.Close();

                sign = MD5("bocv123456IB" + displayName + "a13fa80912d69093429f45dd7440940c").ToUpper();
                url = "http://gslog.336699bet.com/getBalance.aspx?operatorcode=bocv&providercode=IB&username=" + displayName + "&password=123456&signature=" + sign;
                NLogManager.Debug(url);
                request = (HttpWebRequest)WebRequest.Create(url);
                request.ContentType = "application/json";
                request.Method = "GET";//GET
                response = (HttpWebResponse)request.GetResponse();
                result = new StreamReader(response.GetResponseStream()).ReadToEnd();
                response.Close();

                EsportsDataBalanceNew objReturn = JsonConvert.DeserializeObject<EsportsDataBalanceNew>(result);
                return objReturn;
            }
            catch (Exception ex)
            {

                NLogManager.PublishException(ex);
                return new EsportsDataBalanceNew();
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
    }
    public class EsportsDataBalanceNew
    {
        // //{"error_code":1,"message":"success","data":[{"name":"Casino","balance":0},{"name":"Saba","balance":0}]}
        public int errCode { set; get; } = -1;
        public long balance { set; get; } = 1;
        public string errMsg { set; get; } = "none";
    }
    public class EsportsDataBalance
    {
        public class EsportsInfo
        {
            public string name { set; get; }
            public double balance { set; get; }
        }
        // //{"error_code":1,"message":"success","data":[{"name":"Casino","balance":0},{"name":"Saba","balance":0}]}
        public int error_code { set; get; } = -1;
        public string message { set; get; } = "none";
        public List<EsportsInfo> data { set; get; } = new List<EsportsInfo>();
        public double GetSabeCoin()
        {
            EsportsInfo esportsInfo = data.Where(e => { return e.name == "Saba"; }).FirstOrDefault();
            return esportsInfo != null ? esportsInfo.balance : 0;
        }
        public double GetCasinoCoin()
        {
            EsportsInfo esportsInfo = data.Where(e => { return e.name == "Casino"; }).FirstOrDefault();
            return esportsInfo != null ? esportsInfo.balance : 0;
        }
    }
    public class EsDataUrl
    {
        public int errCode { set; get; } = -1;
        public string errMsg { set; get; } = "none";
        public string gameUrl { set; get; } = "#";
    }
    public class EsPostData
    {
        public string Wallet { set; get; }
        public double Amount { set; get; }
        public string Type { set; get; }
        public string GetWallet()
        {
            return this.Wallet == "SabaEsports"? "saba" : "casino";
        }
        public double GetAmountGameToSaba()
        {
            return Amount * 0.00080d;
        }
        public double GetAmountSabaToGame()
        {
            return Amount / 0.00080d;
        }
        public double GetAmountGameToCasino()
        {
            return Amount * 0.00080d;
        }
        public double GetAmountCasinoToGame()
        {
            return Amount / 0.00080d;
        }
    }
    public class EsResponse
    {
        public int errCode { set; get; } = -1;
        public string errMsg { set; get; } = "none";
    }
}