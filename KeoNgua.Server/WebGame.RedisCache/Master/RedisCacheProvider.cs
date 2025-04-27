using System;
using System.Collections.Generic;
using TraditionGame.Utilities;

namespace MsWebGame.RedisCache.Master
{
    public class RedisCacheProvider : ICacheProvider
    {

        public void Set<T>(string key, T value)
        {
            try
            {
                RedisStore.RedisCache.Add<T>(key, value);
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                throw ex;
            }

        }
        public void SetSecond<T>(string key, T value, int second)
        {
            try
            {

                RedisStore.RedisCache.Add<T>(key, value, DateTimeOffset.Now.AddSeconds(second));
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                throw ex;
            }

        }

        public void Set<T>(string key, T value, int minutue)
        {
            try
            {

                RedisStore.RedisCache.Add<T>(key, value, DateTimeOffset.Now.AddMinutes(minutue));
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                throw ex;
            }

        }

        public void IncrementValue(string key, long Value)
        {
            try
            {

                RedisStore.RedisCache.Database.StringIncrement(key, Value);
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                throw ex;
            }

        }

        public T Get<T>(string key)
        {
            try
            {
                T result = default(T);

                result = RedisStore.RedisCache.Get<T>(key);
                return result;
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                throw ex;
            }
        }

        public bool Remove(string key)
        {
            try
            {
                bool removed = false;

                removed = RedisStore.RedisCache.Remove(key);

                return removed;
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                throw ex;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keyPattern"></param>
        public IEnumerable<string> GetByKeyPartnerns<String>(string keyPattern)
        {
            try
            {
                IEnumerable<string> result = default(IEnumerable<string>);

                result = RedisStore.RedisCache.SearchKeys(keyPattern);

                return result;
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                throw ex;
            }
        }

        public bool Exists(string key)
        {
            try
            {
                bool isInCache = false;

                isInCache = RedisStore.RedisCache.Exists(key);

                return isInCache;
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                throw ex;
            }
        }

        IEnumerable<String> ICacheProvider.GetByKeyPartnerns(string keyPattern)
        {
            try
            {
                IEnumerable<string> result = default(IEnumerable<string>);

                result = RedisStore.RedisCache.SearchKeys(keyPattern);

                return result;
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                throw ex;
            }
        }

        public void RemoveByParttern(string pattern)
        {
            try
            {
                var result = RedisStore.RedisCache.SearchKeys(pattern);
                if (result != null)
                {

                    foreach (var key in result)
                    {
                        //NLogManager.LogMessage(string.Format("RemoveByParttern:{0}", key));
                        RedisStore.RedisCache.Database.KeyDelete(key);
                    }

                }
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                throw ex;
            }
        }
    }
}
