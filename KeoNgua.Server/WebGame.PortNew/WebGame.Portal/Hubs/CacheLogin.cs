using System;

using TraditionGame.Utilities.Session;
using TraditionGame.Utilities;

using TraditionGame.Utilities.IpAddress;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using  MsWebGame.RedisCache.Cache;

namespace MsWebGame.Portal.Server.Hubs
{
   
    public class CacheLogin
    {

        private static string cacheFormat = "Portal:{0}.{1}.{2}.{3}";
        #region BlackList
        /// <summary>
        /// Kiểm tra ip thực hiện 1 hành động trong sô giây (tự cộng số lượt mỗi lần gọi hàm check)
        /// </summary>
        /// <param name="totalSecond"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public  static  int AddCache(int totalSecond,string userName,int ServiceID, string action)
        {
            string ip = IPAddressHelper.GetClientIP();
            string cacheName = string.Format(cacheFormat, ip.ToLower(), userName, action, ServiceID);
            RedisCacheProvider _RedisCacheProvider = new RedisCacheProvider();
            var cacheCounter= _RedisCacheProvider.Get<int>(cacheName);
            //System.Runtime.Caching.ObjectCache cache = System.Runtime.Caching.MemoryCache.Default;
            //System.Runtime.Caching.CacheItemPolicy policy = new System.Runtime.Caching.CacheItemPolicy()
            //{
            //    AbsoluteExpiration = DateTime.Now.AddSeconds(totalSecond)
            //};
           
           
            _RedisCacheProvider.Set(cacheName, Convert.ToInt32(cacheCounter) + 1,10);
            return Convert.ToInt32(cacheCounter);
        }

        /// <summary>
        /// Chỉ đếm số lượt trong cache
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public static int CheckStatusFrequency(string userName,int ServiceID, string action)
        {
            string ip = IPAddressHelper.GetClientIP();
            string cacheName = string.Format(cacheFormat, ip.ToLower(), userName, action, ServiceID);
            RedisCacheProvider _RedisCacheProvider = new RedisCacheProvider();
            var cacheCounter = _RedisCacheProvider.Get<int>(cacheName);
            //System.Runtime.Caching.ObjectCache cache = System.Runtime.Caching.MemoryCache.Default;
           // object cacheCounter = cache.Get(cacheName);
            if (cacheCounter == 0)
            {
                return 0;
            }
            return Convert.ToInt32(cacheCounter);
        }
        public static int ClearCache(string userName,int ServiceID, string action)
        {
            string ip = IPAddressHelper.GetClientIP();
            string cacheName = string.Format(cacheFormat, ip.ToLower(), userName, action, ServiceID);
            RedisCacheProvider _RedisCacheProvider = new RedisCacheProvider();
            var isInCache = _RedisCacheProvider.Remove(cacheName);
            if (isInCache)
            {
                _RedisCacheProvider.Remove(cacheName);
            }
            //System.Runtime.Caching.ObjectCache cache = System.Runtime.Caching.MemoryCache.Default;
            //if(cache.Any(c=>c.Key== cacheName))
            //{
            //    object cacheCounter = cache.Remove(cacheName);
            //}
           
            return 0;
        }


        #endregion BlackList
    }
}