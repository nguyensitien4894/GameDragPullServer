using System;
using System.Collections.Concurrent;

namespace TraditionGame.Utilities.Caching
{
    public class GameFilter
    {
        private static readonly Lazy<GameFilter> _instance = new Lazy<GameFilter>(() => new GameFilter());

        private readonly ConcurrentDictionary<long, long> _mapBanAccount = new ConcurrentDictionary<long, long>();

        public static GameFilter Instance
        {
            get { return _instance.Value; }
        }

        public void AddAccount(long accountId)
        {
            try
            {
                if (IsExist(accountId))
                    return;

                _mapBanAccount.TryAdd(accountId, accountId);
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
            }
            
        }

        public bool IsExist(long accountId)
        {
            try
            {
               return _mapBanAccount.ContainsKey(accountId);
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return false;
            }
        }

        public void Reset()
        {
            try
            {
                _mapBanAccount.Clear();
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
            }
        }
    }
}