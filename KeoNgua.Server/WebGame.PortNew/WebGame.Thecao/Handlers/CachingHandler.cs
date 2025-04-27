using System;
using System.Collections.Generic;
using System.Runtime.Caching;

namespace MsWebGame.Thecao.Handlers
{
    public static class CachingHandler
    {
        static readonly ObjectCache cache = MemoryCache.Default;

        public static void AddListCache<T>(string key, List<T> value, int seconds)
        {
            CacheItemPolicy policy = new CacheItemPolicy();
            policy.AbsoluteExpiration = DateTimeOffset.Now.AddSeconds(seconds);
            cache.Set(key, value, policy);
        }

        public static List<T> GetListCache<T>(string key)
        {
            return cache.Get(key) as List<T>;
        }

        public static void Remove(string key)
        {
            cache.Remove(key);
        }
    }
}