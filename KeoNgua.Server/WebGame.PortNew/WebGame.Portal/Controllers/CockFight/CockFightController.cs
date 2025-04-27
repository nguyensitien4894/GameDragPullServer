using MsWebGame.Portal.Database.DAO;
using MsWebGame.Portal.Lib;
using MsWebGame.Portal.Models.USDTBanks;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using TraditionGame.Utilities;
using TraditionGame.Utilities.Security;

namespace MsWebGame.Portal.Controllers.CockFight
{
    public class CockFightController : BaseApiController
    {
        /// <summary>
        /// Get balance
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public dynamic Balance()
        {
            StringBuilder l = new StringBuilder();
            l.AppendLine("==================Cock Fight: get balance==========================");
            try
            {
                string requestBody = "";
                //kiểm tra chữ ký
                if (!checkSignature(ref requestBody, ref l))
                {
                    return new
                    {
                        code = -1,
                        balance = 0,
                        msg = "Wrong signature"
                    };
                }
                var input = JsonConvert.DeserializeObject<CFUserParam>(requestBody);
                string username = input.username;
                if (username == null || username == "") {
                    return new
                    {
                        code = -2,
                        balance = 0,
                        msg = "Params is invalid"
                    };
                }


                var user = AccountDAO.Instance.GetAccountInfo(0, username, null, ServiceID); ;
                if (user == null)
                {

                    return new
                    {
                        code = -3,
                        balance = 0,
                        msg = "Not found data"
                    };
                }

                return new
                {
                    code = 0,
                    balance = user.Balance,
                    msg = "OK"
                };

            }
            catch (Exception ex)
            {
                l.AppendLine("Error  " + ex);
                return new
                {
                    code = -4,
                    balance = 0,
                    msg = "Something went wrong"
                };

            }
            finally
            {
                l.AppendLine("***************************");
                NLogManager.CockFightLog(l.ToString());
            }

        }


        /// <summary>
        /// Get balance
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public dynamic PlaceABet()
        {
            StringBuilder l = new StringBuilder();
            l.AppendLine("==================Cock Fight: get balance==========================");
            try
            {
                string requestBody = "";
                //kiểm tra chữ ký
                if (!checkSignature(ref requestBody, ref l))
                {
                    return new
                    {
                        code = -1,
                        balance = 0,
                        msg = "Wrong signature"
                    };
                }
                var input = JsonConvert.DeserializeObject<CFBetParam>(requestBody);
                string username = input.username;
                if (username == null || username == "")
                {
                    return new
                    {
                        code = -2,
                        balance = 0,
                        msg = "Param is invalid"
                    };
                }
                var user = AccountDAO.Instance.GetAccountInfo(0, username, null, ServiceID); ;
                if (user == null)
                {

                    return new
                    {
                        code = -3,
                        balance = 0,
                        msg = "User not found"
                    };
                }


                if (user.Balance < input.amount)
                {
                    return new
                    {
                        code = -4,
                        balance = user.Balance,
                        msg = "Balance is not enough"
                    };
                }
                //trừ tiền 

                return new
                {
                    code = 0,
                    balance = user.Balance,
                    msg = "OK"
                };

            }
            catch (Exception ex)
            {
                l.AppendLine("Error  " + ex);
                return new
                {
                    code = -4,
                    balance = 0,
                    msg = "Something went wrong"
                };

            }
            finally
            {
                l.AppendLine("***************************");
                NLogManager.CockFightLog(l.ToString());
            }

        }




        private bool checkSignature(ref string requestBody, ref StringBuilder l )
        {
            string t = HttpContext.Current.Request.QueryString["t"];
            string s = HttpContext.Current.Request.QueryString["s"];
            requestBody = WebClass.GetRequestBody();
            l.AppendLine("RawUrl: " + HttpContext.Current.Request.RawUrl);
            l.AppendLine("requestBody: " + requestBody);
            l.AppendLine("signature: " + s);
            l.AppendLine("timestamp: " + t);
            string expectSignature = Security.MD5Encrypt(requestBody + "*" + t);
            return s == expectSignature;
        }

    }

    public class CFUserParam
    {
        public string username { get; set; }
            
        public string currency { get; set; }

    }



    public class CFBetParam
    {
        public string username { get; set; }
        public string currency { get; set; }
        public long amount { get; set; }
        public List<BetDetail> betDetails { get; set; }
        public string txd { get; set; }
        public int gameType { get; set; }
        public string gameId { get; set; }
        public string tableId { get; set; }
        public DateTime betTime { get; set; }
    }

    public class BetDetail
    {
        public int type { get; set; }
        public long amount { get; set; }
        public double odds { get; set; }
    }
}
