using MsWebGame.RedisCache.Cache;
using System;
using System.Collections.Generic;
using System.Runtime.Caching;
using TraditionGame.Utilities;

namespace MsWebGame.Portal.Handlers
{
    public static class CachingHandler
    {
        static readonly ObjectCache cache = MemoryCache.Default;
        private static string cacheFormat = "Portal:{0}.{1}";

        public static void AddListCache<T>(string key, int ServiceID, List<T> value, int second)
        {
            try
            {
                RedisCacheProvider _RedisCacheProvider = new RedisCacheProvider();

                var cacheName = String.Format(cacheFormat, key, ServiceID);


                _RedisCacheProvider.SetSecond(cacheName, value, second);
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);

            }

        }
        public static void AddObjectCache<T>(string key, int ServiceID, T value, int second)
        {
            try
            {
                RedisCacheProvider _RedisCacheProvider = new RedisCacheProvider();

                var cacheName = String.Format(cacheFormat, key, ServiceID);


                _RedisCacheProvider.SetSecond(cacheName, value, second);
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);

            }

        }
        public static T GetObjectCache<T>(string key, int ServiceID)
        {
            try
            {
                RedisCacheProvider _RedisCacheProvider = new RedisCacheProvider();
                var cacheName = String.Format(cacheFormat, key, ServiceID);
                return _RedisCacheProvider.Get<T>(cacheName);
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return default(T);
            }

        }
        public static List<T> GetListCache<T>(string key, int ServiceID)
        {
            try
            {
                RedisCacheProvider _RedisCacheProvider = new RedisCacheProvider();
                var cacheName = String.Format(cacheFormat, key, ServiceID);
                return _RedisCacheProvider.Get<List<T>>(cacheName);
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return null;
            }

        }

        public static string GeneralRedisKey(string gameName, string typeName)
        {
            string val = string.Empty;
            val = string.Format("Game.{0}:{1}", gameName, typeName);
            return val;
        }

        public static bool ExistCache(string key,int ServiceID)
        {
            try
            {
                RedisCacheProvider _RedisCacheProvider = new RedisCacheProvider();
                var cacheName = String.Format(cacheFormat, key, ServiceID);
                return _RedisCacheProvider.Exists(cacheName);
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return false;
            }
        }
        public static void Remove(string key, int ServiceID)
        {
            try
            {
                RedisCacheProvider _RedisCacheProvider = new RedisCacheProvider();
                var cacheName = String.Format(cacheFormat, key, ServiceID);
                _RedisCacheProvider.Remove(cacheName);
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);

            }

        }
    }
}