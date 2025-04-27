using System;
using System.Collections.Concurrent;
using System.Configuration;
using TraditionGame.Utilities;

namespace MsWebGame.CSKH.Helpers
{
    public class SendRequestApi
    {
        private static readonly Lazy<SendRequestApi> _instance = new Lazy<SendRequestApi>(() => new SendRequestApi());

        private static ConcurrentDictionary<int, string> _mapUrl = new ConcurrentDictionary<int, string>();
        private static ConcurrentDictionary<int, string> _mapServiceIDToName = new ConcurrentDictionary<int, string>();
        private static readonly string serviceId_str = ConfigurationManager.AppSettings["SERVICEIDS"];

        public static SendRequestApi Instance
        {
            get { return _instance.Value; }
        }

        private SendRequestApi()
        {
            //string[] lstUrl = ConfigurationManager.AppSettings["REQUEST_URL"].Split(',');
            //for (int i = 0; i < lstUrl.Length; i++)
            //{
            //    if (_mapUrl.ContainsKey(i + 1))
            //    {
            //        string val = string.Empty;
            //        _mapUrl.TryRemove(i + 1, out val);
            //    }

            //    _mapUrl.TryAdd(i + 1, lstUrl[i]);
            //}

            if (!string.IsNullOrEmpty(serviceId_str))
            {
                string[] lstMap = serviceId_str.Split(',');
                for (int i = 0; i < lstMap.Length; i++)
                {
                    if (_mapServiceIDToName.ContainsKey(i + 1))
                    {
                        string val = string.Empty;
                        _mapServiceIDToName.TryRemove(i + 1, out val);
                    }

                    _mapServiceIDToName.TryAdd(i + 1, lstMap[i]);
                }
            }
        }

        public string GetServiceName(int serviceId)
        {
            string val = string.Empty;
            if (serviceId < 1)
                return val;

            if (_mapServiceIDToName.Count < serviceId)
                return val;

            _mapServiceIDToName.TryGetValue(serviceId, out val);
            return val;
        }

        public bool SendLockChat(string adminName, string nickName, string uri, int serviceId)
        {
            try
            {
                string url = ConfigurationManager.AppSettings["REQUEST_URL"].ToString();
               // _mapUrl.TryGetValue(serviceId, out url);
                var api = new ApiUtil<bool>();
                api.ApiAddress = url;
                api.URI = uri;
                string nickNameBaned = string.Format("[{0}]{1}", GetServiceName(serviceId), nickName);
                var res = api.Send(new { AdminName = adminName, NickName = nickNameBaned });
                return res;
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return false;
            }
        }

        public string SendUnLockChat(string adminName, string nickName, string uri, int serviceId)
        {
            try
            {
                string url = ConfigurationManager.AppSettings["REQUEST_URL"].ToString();
               // _mapUrl.TryGetValue(serviceId, out url);
                var api = new ApiUtil<string>();
                api.ApiAddress = url;
                api.URI = uri;
                string nickNameBaned = string.Format("[{0}]{1}", GetServiceName(serviceId), nickName);
                var res = api.Send(new { AdminName = adminName, NickName = nickNameBaned });
                return res;
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return string.Empty;
            }
        }
    }
}