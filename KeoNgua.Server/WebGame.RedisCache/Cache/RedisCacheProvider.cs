using System;
using System.Collections.Generic;
using StackExchange.Redis;
using TraditionGame.Utilities;

namespace MsWebGame.RedisCache.Cache
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
        public long GetBalance(long UserId)
        {
            string keyHuTai = "Balances:" + UserId;
            return this.Get<long>(keyHuTai);
        }
        public void CCU (string Username,long UserId,int DeviceType,string Game = "TX")
        {
            HashEntry[] redisBookHash = {
                new HashEntry(Username, UserId)
            };
         
            RedisStore.RedisCache.Database.HashSet("CCU:"+ Game +":"+ DeviceType + ":"+DateTime.Now.ToString("yyyyMMddHHmm"), redisBookHash);
        }
        public int[] Count(DateTime Time,string Game = "TX")
        {
            int[] counts = new int[] { -1 , -1,  -1 };
            string Key = Time.ToString("yyyyMMddHHmm");
            for (int i=1 ; i <= counts.Length;i++)
            {
                string hashKey = "CCU:"+Game +":"+ i + ":" + Key;
                if (RedisStore.RedisCache.Database.KeyExists(hashKey))
                {
                    var allHash = RedisStore.RedisCache.Database.HashGetAll(hashKey);
                    counts[i - 1] = allHash.Length;
                }
            }
            for (int i = 1; i <= counts.Length; i++)
            {
                string hashKey = "CCU:" + Game + ":" + i + ":" + Key;
                if (counts[i - 1]>-1)
                {
                    RedisStore.RedisCache.Database.KeyDelete(hashKey);
                }
            }
            return counts;
        }
    }
}
