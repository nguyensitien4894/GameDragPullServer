using System;
using System.Collections.Generic;
using TraditionGame.Utilities;

namespace MsWebGame.RedisCache.Cache
{
    public class CachingHandler
    {
        private static readonly Lazy<CachingHandler> _instance = new Lazy<CachingHandler>(() => new CachingHandler());

        public static CachingHandler Instance
        {
            get { return _instance.Value; }
        }

        public void AddCache<T>(string key, T value, int second)
        {
            try
            {
                RedisCacheProvider _RedisCacheProvider = new RedisCacheProvider();
                _RedisCacheProvider.SetSecond(key, value, second);
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
            }
        }

        public T GetCache<T>(string key)
        {
            try
            {
                RedisCacheProvider _RedisCacheProvider = new RedisCacheProvider();
                return _RedisCacheProvider.Get<T>(key);
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return default(T);
            }
        }

        public void AddListCache<T>(string key, List<T> value, int second)
        {
            try
            {
                RedisCacheProvider _RedisCacheProvider = new RedisCacheProvider();
                _RedisCacheProvider.SetSecond(key, value, second);
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
            }
        }

        public List<T> GetListCache<T>(string key)
        {
            try
            {
                RedisCacheProvider _RedisCacheProvider = new RedisCacheProvider();
                return _RedisCacheProvider.Get<List<T>>(key);
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return null;
            }
        }

        public string GeneralRedisKey(string gameName, string typeName)
        {
            string val = string.Empty;
            val = string.Format("Game.{0}:{1}", gameName, typeName);
            return val;
        }

        public string GeneralRedisKey(string gameName, string typeName, int serviceId)
        {
            string val = string.Empty;
            val = string.Format("Game.{0}:{1}.{2}", gameName, typeName, serviceId);
            return val;
        }

        public string GeneralRedisKeyTool(string gameName, string typeName)
        {
            string val = string.Empty;
            val = string.Format("Tool.{0}:{1}", gameName, typeName);
            return val;
        }
    }
}