using System;

namespace MsWebGame.RedisCache.Config
{
    public class PubSubChannels
    {
        public static readonly string NOTIFY_ACCOUNT = "account";
        public static readonly string NOTIFY_ACHIEVEMENT = "achievement";
    }

    public enum PubSubActions
    {
        LOGIN = 1,
        LOGOUT = 2,
        UPDATE_ACCOUNT = 3
    }

    public class PubSubMessage
    {
        public PubSubActions Action { get; set; }
        public long AccountId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }

}
