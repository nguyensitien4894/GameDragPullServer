using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using MsTraditionGame.Utilities.Log;
using MsTraditionGame.Utilities.Security;

namespace MsTraditionGame.Utilities.Session
{
    public class AccountSession
    {
        public static long AccountID
        {
            get
            {
                long userId = 0;
                var accesToken = HttpContext.Current.Request.QueryString["access_token"];
                if (!String.IsNullOrEmpty(accesToken))
                {
                    var token = accesToken.Replace("Bearer", string.Empty).Trim();
                    userId = JWTTokenProvider.UserID(token);
                    return userId;
                }
                else
                {
                    if (HttpContext.Current != null && HttpContext.Current.User.Identity.IsAuthenticated)
                    {
                        var s = HttpContext.Current.User.Identity.Name.Split('|');
                        /*
                         * if (s != null && s.Length > 3)
                        {
                            string userAgent = HttpContext.Current.Request.UserAgent;
                            string ipAddress = HttpContext.Current.Request.UserHostAddress;
                            NLogManager.LogMessage(ipAddress + " --- " + userAgent);
                            if (!ipAddress.Equals(s[2]) || !userAgent.Equals(s[3]))
                                return 0;
                        }
                         * */
                        userId = Convert.ToInt64(s[0]);
                    }
                }
                

                return userId;
            }
        }

        public static string AccountName
        {
            get
            {
                string userName = string.Empty;

                var accesToken = HttpContext.Current.Request.QueryString["access_token"];
                if (!String.IsNullOrEmpty(accesToken))
                {
                    var token = accesToken.Replace("Bearer", string.Empty).Trim();
                    userName = JWTTokenProvider.UserName(token);
                    return userName;
                }

                return userName;
            }
        }
        //public static string AccountName
        //{
        //    get
        //    {
        //        string userName = string.Empty;

        //        if (HttpContext.Current != null && HttpContext.Current.User.Identity.IsAuthenticated && !string.IsNullOrEmpty(HttpContext.Current.User.Identity.Name))
        //        {
        //            var s = HttpContext.Current.User.Identity.Name.Split('|');
        //            if (s != null && s.Length > 1)
        //                userName = s[1];
        //        }

        //        return userName;
        //    }
        //} 

        public static int MerchantID
        {
            get
            {
                int merchantId = 0;

                if (HttpContext.Current != null && HttpContext.Current.User.Identity.IsAuthenticated)
                {
                    var s = HttpContext.Current.User.Identity.Name.Split('|');
                    if (s.Length > 4)
                        merchantId = Convert.ToInt32(s[4]);
                }

                return merchantId;
            }
        }

        public static string AccessToken
        {
            get
            {
                string accessToken = "";

                if (HttpContext.Current != null && HttpContext.Current.User.Identity.IsAuthenticated)
                {
                    var s = HttpContext.Current.User.Identity.Name.Split('|');
                    NLogManager.LogMessage("AccessToken:" + HttpContext.Current.User.Identity.Name);
                    if (s.Length > 2)
                        accessToken = s[3];
                }

                return accessToken;
            }
        }

        public static int SourceID
        {
            get
            {
                int sourceId = 1;

                if (HttpContext.Current != null && HttpContext.Current.User.Identity.IsAuthenticated)
                {
                    var s = HttpContext.Current.User.Identity.Name.Split('|');
                    if (s.Length > 5)
                        sourceId = Convert.ToInt32(s[5]);
                }
                if (sourceId < 1)
                {
                    sourceId = 1;
                }

                return sourceId;
            }
        }

        public static string SessionName(long accountId, string username, int merchantId, int sourceId)
        {
            string ipAddress = HttpContext.Current.Request.UserHostAddress;
            string userAgent = HttpContext.Current.Request.UserAgent;
            return string.Format("{0}|{1}|{2}|{3}|{4}|{5}", accountId, username, ipAddress, userAgent, merchantId, sourceId);
        }
    }
}
