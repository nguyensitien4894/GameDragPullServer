//using System;

//namespace MsWebGame.RedisCache.Clients
//{
//    public abstract class AbstractPubSubClient
//    {
//        public abstract void SubscribeToChannels(params string[] channels);
//        public abstract void UnSubscribeFromChannels(params string[] channels);
//        public abstract void UnSubscribeFromAllChannels();
//        public abstract void PublishMessage(string channel, string message);

//        public abstract long SubscriptionCount { get; }
//        public abstract Action<string> OnSubscribe { get; set; }
//        public abstract Action<string, string> OnMessage { get; set; }
//        public abstract Action<string> OnUnSubscribe { get; set; }
//    }
//}
