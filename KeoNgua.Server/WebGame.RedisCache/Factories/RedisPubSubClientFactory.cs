//using System;
//using System.Configuration;
//using ServiceStack.Redis;
//using MsWebGame.RedisCache.Clients;

//namespace MsWebGame.RedisCache.Factories
//{
//    public class RedisPubSubClientFactory : AbstractPubSubClientFactory
//    {
//        static readonly string HOST = ConfigurationManager.AppSettings["redis_host"];
//        static readonly string PORT = ConfigurationManager.AppSettings["redis_port"];
       
//        private readonly PooledRedisClientManager _clientManager;

//        public RedisPubSubClientFactory()
//        {
//            _clientManager = new PooledRedisClientManager(RedisPubSubClientFactory.HOST);
//        }

//        public override AbstractPubSubClient CreateClient()
//        {
//            return new RedisPubSubClient(_clientManager.GetClient());
//        }

//        public override void Dispose()
//        {
//            _clientManager.Dispose();
//        }
//    }
//}
