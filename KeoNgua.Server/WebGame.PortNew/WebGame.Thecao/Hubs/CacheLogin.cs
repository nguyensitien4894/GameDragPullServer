using System;

using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using TraditionGame.Utilities.IpAddress;

namespace MsWebGame.Thecao.Server.Hubs
{
   
    public class CacheLogin
    {

        private static string cacheFormat = "@Post_{0}_{1}_{2}";
        #region BlackList
        /// <summary>
        /// Kiểm tra ip thực hiện 1 hành động trong sô giây (tự cộng số lượt mỗi lần gọi hàm check)
        /// </summary>
        /// <param name="totalSecond"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public  static  int AddCache(int totalSecond,string userName, string action)
        {
            string ip = IPAddressHelper.GetClientIP();
            string cacheName = string.Format(cacheFormat, ip.ToLower(), userName, action);
            System.Runtime.Caching.ObjectCache cache = System.Runtime.Caching.MemoryCache.Default;
            System.Runtime.Caching.CacheItemPolicy policy = new System.Runtime.Caching.CacheItemPolicy()
            {
                AbsoluteExpiration = DateTime.Now.AddSeconds(totalSecond)
            };
            object cacheCounter = cache.Get(cacheName);
            if (cacheCounter == null)
            {
                cache.Set(cacheName, 1, policy);
                return 0;
            }
            cache.Set(cacheName, Convert.ToInt32(cacheCounter) + 1, policy);
            return Convert.ToInt32(cacheCounter);
        }

        /// <summary>
        /// Chỉ đếm số lượt trong cache
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public static int CheckStatusFrequency(string userName, string action)
        {
            string ip = IPAddressHelper.GetClientIP();
            string cacheName = string.Format(cacheFormat, ip.ToLower(), userName, action);
            System.Runtime.Caching.ObjectCache cache = System.Runtime.Caching.MemoryCache.Default;
            object cacheCounter = cache.Get(cacheName);
            if (cacheCounter == null)
            {
                return 0;
            }
            return Convert.ToInt32(cacheCounter);
        }
        public static int ClearCache(string userName, string action)
        {
            string ip = IPAddressHelper.GetClientIP();
            string cacheName = string.Format(cacheFormat, ip.ToLower(), userName, action);
            System.Runtime.Caching.ObjectCache cache = System.Runtime.Caching.MemoryCache.Default;
            if(cache.Any(c=>c.Key== cacheName))
            {
                object cacheCounter = cache.Remove(cacheName);
            }
           
            return 0;
        }


        #endregion BlackList
    }
}