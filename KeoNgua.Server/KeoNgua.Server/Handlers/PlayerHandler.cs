using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using TraditionGame.Utilities;
using KeoNgua.Server.Models;
using KeoNgua.Server.DataAccess.Dao;
using KeoNgua.Server.DataAccess.Factory;
using KeoNgua.Server.DataAccess.Dto;
using System.Linq;

namespace KeoNgua.Server.Handlers
{
    public class PlayerHandler
    {
        private static readonly Lazy<PlayerHandler> _instance = new Lazy<PlayerHandler>(() => new PlayerHandler());
        private readonly InnerPlayerHandler<GamePlayer> _inner = new InnerPlayerHandler<GamePlayer>();

        public static InnerPlayerHandler<GamePlayer> Instance
        {
            get { return _instance.Value._inner; }
        }

        private PlayerHandler()
        {
        }

        public class InnerPlayerHandler<T> where T : AbstractGamePlayer
        {
            // Singleton instance       
            private readonly ConcurrentDictionary<long, T> _players;
            private readonly IAccountDao _accountDao;

            public InnerPlayerHandler()
            {
                _players = new ConcurrentDictionary<long, T>();
                _accountDao = AbstractFactory.Instance().CreateAccountDao();
            }

            public List<AbstractGamePlayer> Players()
            {
                return _players.Values.Select(p => p as AbstractGamePlayer).ToList();
            }

            public List<long> KeyPlayers()
            {
                return _players.Keys.ToList();
            }

            #region dictionary_methods
            private T CreatePlayer(params object[] args)
            {
                return (T)Activator.CreateInstance(typeof(T), args);
            }

            public bool Contains(long accountId)
            {
                return _players.ContainsKey(accountId);
            }

            public T GetPlayer(long accountId)
            {
                if (accountId < 1) return null;

                T player = null;
                if (this.Contains(accountId))
                {
                    _players.TryGetValue(accountId, out player);
                }
                return player;
            }

            /// <summary>
            /// Thêm player vào mảng
            /// </summary>
            /// <param name="player"></param>
            public bool AddPlayer(T player)
            {
                if (player == null) return false;

                if (!_players.ContainsKey(player.AccountID)) return _players.TryAdd(player.AccountID, player);

                AccountDB oldAccount = player.Account;
                //update account
                if (!Monitor.TryEnter(oldAccount, 5000)) return true;

                try
                {
                    this.Copy(player.Account, oldAccount);
                }
                finally
                {
                    Monitor.Exit(oldAccount);
                }
                return true;
            }

            public T AddPlayer(AccountDB account)
            {
                if (account == null) return null;

                if (account.AccountID < 1) return null;

                T player;
                if (_players.ContainsKey(account.AccountID))
                {
                    player = this.GetPlayer(account.AccountID);
                    AccountDB oldAccount = player.Account;
                    //update account
                    if (!Monitor.TryEnter(oldAccount, 5000)) return player;

                    try
                    {
                        this.Copy(player.Account, oldAccount);
                    }
                    finally
                    {
                        Monitor.Exit(oldAccount);
                    }
                    return player;
                }

                player = this.CreatePlayer(account);
                if (player == null) return null;

                this.AddPlayer(player);
                return player;
            }

            public T AddPlayer(long accountId, string nickName, int deviceId, int serviceId, int avatar, int vip = 1)
            {
                if (accountId < 1 || String.IsNullOrEmpty(nickName)) return null;

                if (_players.ContainsKey(accountId)) return this.GetPlayer(accountId);

                try
                {
                    AccountDB account = _accountDao.GetAccount(accountId, nickName, deviceId, serviceId, avatar, vip);
                    return this.AddPlayer(account);
                }
                catch (Exception ex)
                {
                    NLogManager.PublishException(ex);
                }
                return null;
            }

            public bool RemovePlayer(long accountId)
            {
                T player;
                return _players.TryRemove(accountId, out player);
            }

            public int Count()
            {
                return _players.Count;
            }

            public bool Any()
            {
                return _players.Any();
            }
            #endregion dictionary_methods


            #region update_from_db
            public void Update(long accountId)
            {
                try
                {
                    if (!_players.ContainsKey(accountId)) return;

                    T player;
                    if (!_players.TryGetValue(accountId, out player)) return;

                    AccountDB oldAcc = player.Account;
                    AccountDB account = null;
                    if (accountId > Config.MAX_BOTID)
                    {
                        account = _accountDao.GetAccount(accountId, oldAcc.NickName, oldAcc.DeviceID, oldAcc.ServiceID, oldAcc.Avatar, oldAcc.Vip);
                    }
                    else
                    {
                        BotDB bot = BotHandler.Instance.GetBot(accountId);
                        if (bot == null) return;
                        account = new AccountDB(bot.AbsAccountID, bot.NickName, bot.Star, bot.DeviceID, bot.ServiceID, bot.Avatar, 1);
                    }

                    //update account
                    if (!Monitor.TryEnter(oldAcc, 5000)) return;
                    try
                    {
                        this.Copy(account, oldAcc);
                    }
                    finally
                    {
                        Monitor.Exit(oldAcc);
                    }
                }
                catch (Exception ex)
                {
                    NLogManager.PublishException(ex);
                }
            }

            public void UpdateAll(List<long> accounts)
            {
                try
                {
                    for (int i = 0; i < accounts.Count; i++)
                    {
                        this.Update(accounts[i]);
                    }
                }
                catch (Exception ex)
                {
                    NLogManager.PublishException(ex);
                }
            }

            public AccountDB UpdateAndGet(long accountId)
            {
                try
                {
                    if (!_players.ContainsKey(accountId)) return null;

                    T player;
                    if (!_players.TryGetValue(accountId, out player)) return null;

                    AccountDB oldAcc = player.Account;
                    AccountDB account = _accountDao.GetAccount(accountId, oldAcc.NickName, oldAcc.DeviceID, oldAcc.ServiceID, oldAcc.Avatar, oldAcc.Vip);
                    //update account
                    if (Monitor.TryEnter(oldAcc, 5000))
                    {
                        try
                        {
                            this.Copy(account, oldAcc);
                            return account;
                        }
                        finally
                        {
                            Monitor.Exit(oldAcc);
                        }
                    }
                }
                catch (Exception ex)
                {
                    NLogManager.PublishException(ex);
                }
                return null;
            }
            #endregion update_from_db


            #region util_methods
            /// <summary>
            /// Copy from first to second.
            /// </summary>
            /// <param name="first"></param>
            /// <param name="second"></param>
            public void Copy(AccountDB first, AccountDB second)
            {
                second.Balance = first.Balance;
                second.DeviceID = first.DeviceID;
                second.Avatar = first.Avatar;
                second.Vip = first.Vip;
            }

            public JToken Clone(T player)
            {
                try
                {
                    var obj = JToken.FromObject(player);
                    return obj;
                }
                catch (Exception ex)
                {
                    NLogManager.PublishException(ex);
                }
                return null;
            }

            public AccountDB GetAccount(long accountId)
            {
                if (accountId < 1) return null;

                T player;
                return _players.TryGetValue(accountId, out player) ? new AccountDB(player.Account) : null;
            }
            #endregion util_methods
        }
    }
}