
using StackExchange.Redis;
using StackExchange.Redis.Extensions.Core;
using StackExchange.Redis.Extensions.Core.Configuration;
using StackExchange.Redis.Extensions.Newtonsoft;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TraditionGame.Utilities;

namespace MsWebGame.RedisCache.Master
{
   public class RedisStore
    {
        private static readonly Lazy<StackExchangeRedisCacheClient> cacheClient;
        static readonly string HOST = ConfigurationManager.AppSettings["redis_host"];
        static readonly int PORT = Convert.ToInt32(ConfigurationManager.AppSettings["redis_port"]);
        static readonly string PASSWORD = ConfigurationManager.AppSettings["redis_password"];
        static readonly string DB = "1";
        static RedisStore()
        {
            
            // Connection setup
            var configurationOptions = new RedisConfiguration
            {
                //EndPoints = { String.Format("{0}:{1}", HOST, PORT) }

                 //KeepAlive = 10,
                //AbortOnConnectFail = false,
                //ConfigurationChannel = "",
                //TieBreaker = "",
                //ConfigCheckSeconds = 0,
                //CommandMap = CommandMap.Create(new HashSet<string>
                //{ // EXCLUDE a few commands
                //    "SUBSCRIBE", "UNSUBSCRIBE", "CLUSTER"
                //}, available: false),
                Password = PASSWORD, //will ingnore in case of no password is set
                //AllowAdmin = true

                AbortOnConnectFail = false,
                Database=ConvertUtil.ToInt(DB),
                //KeyPrefix = "_my_key_prefix_",
                Hosts = new RedisHost[]
                {
                    new RedisHost(){Host = HOST, Port = PORT},
                   

                },
                
            };
            var serializer = new NewtonsoftSerializer();
            
            cacheClient = new Lazy<StackExchangeRedisCacheClient>(() => new StackExchangeRedisCacheClient(serializer, configurationOptions));
        }

        public static StackExchangeRedisCacheClient RedisCache => cacheClient.Value;

        

        //public static IServer GetServer => Connection.GetServer(String.Format("{0}:{1}", HOST, PORT));
    }
}
