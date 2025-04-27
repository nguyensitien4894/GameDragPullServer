//using System;
//using MsWebGame.RedisCache.Clients;

//namespace MsWebGame.RedisCache.Factories
//{
//    public abstract class AbstractPubSubClientFactory
//    {
//        private readonly static Lazy<AbstractPubSubClientFactory> _instance = new Lazy<AbstractPubSubClientFactory>(() => new RedisPubSubClientFactory());
//        public static AbstractPubSubClientFactory Instance { get { return _instance.Value; } }
//        public abstract AbstractPubSubClient CreateClient();
//        public abstract void Dispose();
//    }
//}
