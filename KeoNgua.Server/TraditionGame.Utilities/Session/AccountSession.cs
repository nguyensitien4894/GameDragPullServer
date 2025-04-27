using System;
using System.Web;
using TraditionGame.Utilities.Security;

namespace TraditionGame.Utilities.Session
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
                    var token = accesToken.Trim();
                    userId = TokenHashprovider.UserID(token);
                    //NLogManager.LogMessage(string.Format("AccountID: {0}", userId));
                    return userId;
                }
                else
                {
                    //NLogManager.LogMessage(string.Format("access_token-username null: {0}", HttpContext.Current.Request.Url.ToString()));
                }

                return userId;
            }
        }
        public static int DeviceType
        {
            get
            {
                int DeviceType = 1;
                var accesToken = HttpContext.Current.Request.QueryString["access_token"];
                if (!String.IsNullOrEmpty(accesToken))
                {
                    var token = accesToken.Trim();
                    DeviceType = TokenHashprovider.DeviceType(token);
                    //NLogManager.LogMessage(string.Format("AccountID: {0}", userId));
                    return DeviceType;
                }
                else
                {
                    //NLogManager.LogMessage(string.Format("access_token-username null: {0}", HttpContext.Current.Request.Url.ToString()));
                }

                return DeviceType;
            }
        }
        public static string DeviceId
        {
            get
            {
                string DeviceType = "Empty";
                var accesToken = HttpContext.Current.Request.QueryString["access_token"];
                if (!String.IsNullOrEmpty(accesToken))
                {
                    var token = accesToken.Trim();
                    DeviceType = TokenHashprovider.DeviceId(token);
                    //NLogManager.LogMessage(string.Format("AccountID: {0}", userId));
                    return DeviceType;
                }
                else
                {
                    //NLogManager.LogMessage(string.Format("access_token-username null: {0}", HttpContext.Current.Request.Url.ToString()));
                }

                return DeviceType;
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
                    var token = accesToken.Trim();
                    userName = TokenHashprovider.UserName(token);
                    //NLogManager.LogMessage(string.Format("AccountName: {0}", userName));
                    return userName;
                }
                else
                {
                    //NLogManager.LogMessage(string.Format("access_token-username null: {0}", HttpContext.Current.Request.Url.ToString()));
                }

                return userName;
            }
        }

        public static int AvatarID
        {
            get
            {
                int sID = 0;
                var accesToken = HttpContext.Current.Request.QueryString["access_token"];
                if (!String.IsNullOrEmpty(accesToken))
                {
                    var token = accesToken.Trim();
                    sID = TokenHashprovider.AvatarID(token);
                    //NLogManager.LogMessage(string.Format("AvatarID: {0}", sID));
                    return sID;
                }
                else
                {
                    //NLogManager.LogMessage(string.Format("access_token-username null: {0}", HttpContext.Current.Request.Url.ToString()));
                }

                return sID;
            }
        }

        public static int ServiceID
        {
            get
            {
                int sID = 0;
                var accesToken = HttpContext.Current.Request.QueryString["access_token"];
                if (!String.IsNullOrEmpty(accesToken))
                {
                    var token = accesToken.Trim();
                    sID = TokenHashprovider.ServiceID(token);
                    //NLogManager.LogMessage(string.Format("AccountName: {0}", userName));
                    return sID;
                }
                else
                {
                    //NLogManager.LogMessage(string.Format("access_token-username null: {0}", HttpContext.Current.Request.Url.ToString()));
                }

                return sID;
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
                    //NLogManager.LogMessage("AccessToken:" + HttpContext.Current.User.Identity.Name);
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

        #region Chat
        public static long AccountIDChat
        {
            get
            {
                long userId = 0;
                var accesToken = HttpContext.Current.Request.QueryString["access_token"];
                if (!String.IsNullOrEmpty(accesToken))
                {
                    var token = accesToken.Trim();
                    userId = TokenHashprovider.UserIDChat(token);
                    return userId;
                }

                return userId;
            }
        }

        public static string AccountNameChat
        {
            get
            {
                string userName = string.Empty;
                var accesToken = HttpContext.Current.Request.QueryString["access_token"];
                if (!String.IsNullOrEmpty(accesToken))
                {
                    var token = accesToken.Trim();
                    userName = TokenHashprovider.UserNameChat(token);
                    return userName;
                }

                return userName;
            }
        }

        public static int ServiceIDChat
        {
            get
            {
                int sID = 0;
                var accesToken = HttpContext.Current.Request.QueryString["access_token"];
                if (!String.IsNullOrEmpty(accesToken))
                {
                    var token = accesToken.Trim();
                    sID = TokenHashprovider.ServiceIDChat(token);
                    return sID;
                }

                return sID;
            }
        }
        #endregion Chat

        public static string SessionName(long accountId, string username, int merchantId, int sourceId)
        {
            string ipAddress = HttpContext.Current.Request.UserHostAddress;
            string userAgent = HttpContext.Current.Request.UserAgent;
            return string.Format("{0}|{1}|{2}|{3}|{4}|{5}", accountId, username, ipAddress, userAgent, merchantId, sourceId);
        }
    }
}
