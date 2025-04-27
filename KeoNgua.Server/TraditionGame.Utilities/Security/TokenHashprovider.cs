using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TraditionGame.Utilities.Security
{
    public class UserToken
    {
        public long UserID { get; set; }
        public string NickName { get; set; }
        public string IPAddress { get; set; }
        public int ServiceID { get; set; }
        public DateTime ExpiredAt { get; set; }
        public int AvatarID { get; set; }

        public string DeviceId { get; set; } = "Empty";
        public int DeviceType { get; set; } = 1;
        public string MachineName
        {
            get
            {
                return Security.MD5Encrypt(Environment.MachineName);
            }
        }

        public string RandomString
        {
            get
            {
                return Security.RandomString(10);
            }
        }
    }
    public class TokenHashprovider
    {
        //private const string Secret = "NzM3MzTzanRoZPheaGRiM09Ca2orQloFOU5aRHkwdDxXM1RjTmVrckYrMmBvMXNGbldHNEhuVjhUWlkzMGlUT2B0VldKRzhhYld2QjFHbE9nSnVRDmRjRjJMdBFcL2hjI013BT0zNTM=";
        // private const string Secret_Signal = "jTmVrckYrMmXvMXNGblgHBEhuLjhUWlkzTbC3MzYznaRoZGhkaGRiM89Jc2orBlhXOU5aRUkwdDhXM1R";
        private const string Secret = "1e4f6a3b9e0a2d4cf81d47f6c9cbd780";
        private const string Secret_Signal = "15d007f1640f1961e2ab8d7f61a4a881";
        public static string GenerateToken(
            long UserID, 
            string NickName,
            int ServiceID, 
            string IPAddress,
            int ExpiredMinute,
            int AvatarID,
            int DeviceType = 1,// 1 web 2 ios 3 android
            string DeviceId = "Empty"
            )
        {
            var prepareToken = new UserToken
            {
                UserID = UserID,
                NickName = NickName,
                ServiceID = ServiceID,
                ExpiredAt = DateTime.Now.AddMinutes(ExpiredMinute),
                IPAddress = IPAddress,
                AvatarID= AvatarID,
                DeviceType = DeviceType,
                DeviceId = DeviceId

            };
            var jsob = JsonConvert.SerializeObject(prepareToken);

            var token = Security.TripleDESEncrypt(Encoding.UTF8.GetBytes(Secret).ToString(), jsob);
            string signalkey = String.Format("{0}.{1}", Secret_Signal, token);
            var signal = Security.SHA256Encrypt(signalkey);
            return String.Format("{0}.{1}", token, signal);
        }

        private static UserToken GetValidateToken(string authToken)
        {
            try
            {
                var s = System.Web.HttpUtility.UrlDecode(authToken).Replace(" ", "+");
                var arrToken = s.Split('.');
                if (arrToken.Length != 2) return null;

                var token = arrToken[0];
                var signalEncrypt = arrToken[1];
                if (String.IsNullOrEmpty(token) || String.IsNullOrEmpty(signalEncrypt)) return null;

                string signalkey = String.Format("{0}.{1}", Secret_Signal, token);
                string currentSignal = Security.SHA256Encrypt(signalkey);
                if (currentSignal != signalEncrypt)
                {
                    NLogManager.LogMessage(string.Format("ERRTOKEN-Signal:access_token-AccountID: {0}", token));
                    return null;
                }

                var objToken = Security.TripleDESDecrypt(Encoding.UTF8.GetBytes(Secret).ToString(), token);
                var jsob = JsonConvert.DeserializeObject<UserToken>(objToken);
                if (jsob == null || jsob.UserID <= 0)
                {
                    //NLogManager.LogMessage(string.Format("ERRTOKEN-Signal invalid:access_token-AccountID: {0}", token));
                    return null;
                }
                if (jsob.ExpiredAt < DateTime.Now)
                {
                    NLogManager.LogMessage(string.Format("ERRTOKEN-Expired invalid:access_token-AccountID: {0}", token));
                    return null;
                }
                return jsob;

            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return null;
            }
        }

        public static long UserID(string authToken)
        {
            try
            {

                var obj = GetValidateToken(authToken);
                if (obj == null) return 0;
                return obj.UserID;
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return 0;
            }

        }
        public static int DeviceType(string authToken)
        {
            try
            {

                var obj = GetValidateToken(authToken);
                if (obj == null) return 1;
                return obj.DeviceType;
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return 1;
            }

        }
        public static string DeviceId(string authToken)
        {
            try
            {

                var obj = GetValidateToken(authToken);
                if (obj == null) return "Empty";
                return obj.DeviceId;
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return "Empty";
            }

        }
        public static int AvatarID(string authToken)
        {
            try
            {

                var obj = GetValidateToken(authToken);
                if (obj == null) return 0;
                return obj.AvatarID;
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return 0;
            }

        }

        public static int ServiceID(string authToken)
        {
            try
            {
                var obj = GetValidateToken(authToken);
                if (obj == null) return 0;
                return obj.ServiceID;
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return 0;
            }
        }

        public static string UserName(string authToken)
        {
            try
            {
                var obj = GetValidateToken(authToken);
                if (obj == null) return string.Empty;
                return obj.NickName;
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return string.Empty;
            }

        }

        #region Chat
        private static UserToken GetValidateTokenChat(string authToken)
        {
            try
            {
                var s = System.Web.HttpUtility.UrlDecode(authToken).Replace(" ", "+");
                var arrToken = s.Split('.');
                if (arrToken.Length != 2) return null;

                var token = arrToken[0];
                var signalEncrypt = arrToken[1];
                if (String.IsNullOrEmpty(token) || String.IsNullOrEmpty(signalEncrypt)) return null;

                string signalkey = String.Format("{0}.{1}", Secret_Signal, token);
                string currentSignal = Security.SHA256Encrypt(signalkey);
                if (currentSignal != signalEncrypt)
                {
                    NLogManager.LogMessage(string.Format("ERRTOKEN-Signal:access_token-AccountID: {0}", token));
                    return null;
                }

                var objToken = Security.TripleDESDecrypt(Encoding.UTF8.GetBytes(Secret).ToString(), token);
                var jsob = JsonConvert.DeserializeObject<UserToken>(objToken);
                if (jsob == null)
                {
                    return null;
                }
                if (jsob.ExpiredAt < DateTime.Now)
                {
                    NLogManager.LogMessage(string.Format("ERRTOKEN-Expired invalid:access_token-AccountID: {0}", token));
                    return null;
                }
                return jsob;

            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return null;
            }
        }

        public static long UserIDChat(string authToken)
        {
            try
            {
                var obj = GetValidateTokenChat(authToken);
                if (obj == null) return 0;
                return obj.UserID;
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return 0;
            }

        }

        public static string UserNameChat(string authToken)
        {
            try
            {
                var obj = GetValidateTokenChat(authToken);
                if (obj == null) return string.Empty;
                return obj.NickName;
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return string.Empty;
            }
        }

        public static int ServiceIDChat(string authToken)
        {
            try
            {
                var obj = GetValidateTokenChat(authToken);
                if (obj == null) return 0;
                return obj.ServiceID;
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return 0;
            }
        }
        #endregion Chat
    }
}
