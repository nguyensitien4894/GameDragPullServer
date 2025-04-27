
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MsWebGame.RedisCache.Master
{
    public interface ICacheProvider
    {
        void Set<T>(string key, T value);

        void Set<T>(string key, T value, int  minute);

        T Get<T>(string key);

        bool Remove(string key);

        bool Exists(string key);
        IEnumerable<String> GetByKeyPartnerns(string keyPattern);
        void IncrementValue(string key , long value);
        void SetSecond<T>(string key, T value, int second);
        void RemoveByParttern(string pattern);

    }
}
