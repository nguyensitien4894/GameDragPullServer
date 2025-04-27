//using ServiceStack.Redis;
//using System;

//namespace MsWebGame.RedisCache.Clients
//{
//    public class RedisPubSubClient : AbstractPubSubClient
//    {
//        private IRedisClient _client;
//        private IRedisSubscription _sub;

//        public RedisPubSubClient(IRedisClient client)
//        {
//            this._client = client;
//            this._sub = this._client.CreateSubscription();
//        }

//        public override void SubscribeToChannels(params string[] channels)
//        {
//            this._sub.SubscribeToChannels(channels);
//        }

//        public override void UnSubscribeFromChannels(params string[] channels)
//        {
//            this._sub.UnSubscribeFromChannels(channels);
//        }

//        public override void UnSubscribeFromAllChannels()
//        {
//            this._sub.UnSubscribeFromAllChannels();
//        }

//        public override void PublishMessage(string channel, string message)
//        {
//            this._client.PublishMessage(channel, message);
//        }

//        public override long SubscriptionCount
//        {
//            get { return this._sub.SubscriptionCount; }
//        }

//        public override Action<string> OnSubscribe
//        {
//            get
//            {
//                return this._sub.OnSubscribe;
//            }
//            set
//            {
//                this._sub.OnSubscribe = value;
//            }
//        }

//        public override Action<string, string> OnMessage
//        {
//            get
//            {
//                return this._sub.OnMessage;
//            }
//            set
//            {
//                this._sub.OnMessage = value;
//            }
//        }

//        public override Action<string> OnUnSubscribe
//        {
//            get
//            {
//                return this._sub.OnUnSubscribe;
//            }
//            set
//            {
//                this._sub.OnUnSubscribe = value;
//            }
//        }
//    }
//}
