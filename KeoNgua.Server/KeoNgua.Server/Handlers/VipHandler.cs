using KeoNgua.Server.DataAccess.Dto;
using KeoNgua.Server.Hubs;
using KeoNgua.Server.Models;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using TraditionGame.Utilities;

namespace KeoNgua.Server.Handlers
{
    public class VipHandler : IDisposable
    {
        private static readonly Lazy<VipHandler> _instance = new Lazy<VipHandler>(() => new VipHandler());

        private readonly ConcurrentDictionary<long, int> _mapAccountToPosition = new ConcurrentDictionary<long, int>();
        private readonly ConcurrentDictionary<int, GamePlayer> _mapPositionToPlayer = new ConcurrentDictionary<int, GamePlayer>();

        private List<long> _leaveGameList = new List<long>();
        private readonly int _maxIdle = 120;
        private readonly int _maxVip = 7;

        IHubContext Context = GlobalHost.ConnectionManager.GetHubContext<KeoNguaHub>();

        public static VipHandler Instance
        {
            get { return _instance.Value; }
        }

        public int GetMaxVip()
        {
            return _maxVip;
        }

        public List<GamePlayer> GetListVipPlayers()
        {
            try
            {
                List<GamePlayer> results = new List<GamePlayer>(_maxVip);
                for (int i = 1; i <= _maxVip; i++)
                {
                    var result = new GamePlayer(new AccountDB());
                    if (_mapPositionToPlayer.ContainsKey(i)) _mapPositionToPlayer.TryGetValue(i, out result);

                    results.Add(result);
                }

                return results;
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
            }
            return null;
        }

        public List<GamePlayer> GetPlayersNotInGame(long accountId)
        {
            try
            {
                if (this.Count() < _maxVip) return null;

                List<GamePlayer> confirmList = new List<GamePlayer>(PlayerHandler.Instance.Players().Select(p => p as GamePlayer));
                List<long> accounts = this.GetVipKeys();
                bool isVip = this.CheckVip(accountId);
                List<GamePlayer> pNotInGame = null;

                int maxNotInGame = 15;
                int countPlayer = confirmList.Count;
                if (countPlayer < 22) maxNotInGame = countPlayer - _maxVip;
                

                if (isVip)
                {
                    pNotInGame = confirmList.Where(x => !accounts.Contains(Math.Abs(x.AccountID)))
                        .OrderByDescending(x => x.Account.Balance).Take(maxNotInGame).ToList();
                }
                else
                {
                    if (this.Count() >= _maxVip)
                    {
                        foreach (var item in accounts.ToList())
                        {
                            int pos = 0;
                            if (!_mapAccountToPosition.TryGetValue(item, out pos)) return null;

                            if (pos != _maxVip) continue;

                            accounts.Remove(item);
                        }
                    }

                    pNotInGame = confirmList.Where(x => x.AccountID != accountId && !accounts.Contains(Math.Abs(x.AccountID)))
                        .OrderByDescending(x => x.Account.Balance).Take(maxNotInGame).ToList();
                }

                KeoNguaHandler.Instance.SummaryPlayer();
                return pNotInGame;
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
            }
            return null;
        }

        public List<long> GetVipKeys()
        {
            return _mapAccountToPosition.Keys.ToList();
        }

        public int Count()
        {
            return _mapAccountToPosition.Count;
        }

        public void ProcessChangeVips()
        {
            try
            {
                this.RemovePlayerAfterEndSession();
                if (!PlayerHandler.Instance.Any()) throw new Exception(string.Format("{0} ko có người chơi", KeoNguaHandler.Instance.SessionID));

                int countVip = this.Count();
                if (countVip >= _maxVip) throw new Exception(string.Format("{0} vips đã đủ chỗ", KeoNguaHandler.Instance.SessionID));

                //cap nhat lai tien cho toan bo nguoi choi
                List<long> accounts = new List<long>(PlayerHandler.Instance.KeyPlayers());
                PlayerHandler.Instance.UpdateAll(accounts);

                List<GamePlayer> confirmList = new List<GamePlayer>(PlayerHandler.Instance.Players().Select(p => p as GamePlayer));
                if (!confirmList.Any()) throw new Exception(string.Format("{0} ko có người chơi đủ điều kiện", KeoNguaHandler.Instance.SessionID));

                int seatsLeft = _maxVip - countVip;
                List<long> accountsVip = this.GetVipKeys();
                List<GamePlayer> newVips = confirmList.Where(x => !accountsVip.Contains(x.AccountID))
                    .OrderByDescending(c => c.Account.Balance).Take(seatsLeft).ToList();

                foreach (var vip in newVips)
                {
                    if (_mapAccountToPosition.ContainsKey(vip.AccountID)) continue;

                    for (int i = 1; i <= _maxVip; i++)
                    {
                        if (_mapPositionToPlayer.ContainsKey(i)) continue;

                        this.AddVip(vip, i);
                    }
                }

                var currVips = this.GetListVipPlayers();
                //NLogManager.LogMessage(string.Format("{2} PlayerCount:{0} - VipCount:{1} - {3}", confirmList.Count, this.Count(),
                //    KeoNguaHandler.Instance.SessionID, JsonConvert.SerializeObject(currVips)));
                Context.Clients.All.vipPlayers(currVips);
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
            }
        }

        public void ProcessVipPlayNow(long accountId, string connection = null)
        {
            var currVips = this.GetListVipPlayers();
            try
            {
                if (this.Count() >= _maxVip)
                {
                    if (!string.IsNullOrEmpty(connection)) Context.Clients.Client(connection).vipPlayers(currVips);

                    return;
                }

                Phrases phrase = KeoNguaHandler.Instance.Phrase;
                if (phrase == Phrases.ShowResult)
                    throw new Exception(string.Format("{0} trạng thái game không hợp lệ", KeoNguaHandler.Instance.SessionID));

                GamePlayer vip = PlayerHandler.Instance.GetPlayer(accountId);
                if (vip == null) throw new Exception(string.Format("{0} vip null", KeoNguaHandler.Instance.SessionID));
                if (_mapAccountToPosition.ContainsKey(accountId))
                    throw new Exception(string.Format("{0} bạn đã nằm trong vips", KeoNguaHandler.Instance.SessionID));

                for (int i = 1; i <= _maxVip; i++)
                {
                    if (_mapPositionToPlayer.ContainsKey(i)) continue;

                    this.AddVip(vip, i);
                }

                currVips = this.GetListVipPlayers();
                Context.Clients.All.vipPlayers(currVips);
                return;
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
            }

            if (!string.IsNullOrEmpty(connection)) Context.Clients.Client(connection).vipPlayers(currVips);
        }

        public void UpdateBalanceVip(long accountId, long balance)
        {
            try
            {
                var vip = this.GetVip(accountId);
                if (vip != null) vip.Account.Balance = balance;
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
            }
        }

        public bool CheckVip(long accountId)
        {
            try
            {
                return _mapAccountToPosition.ContainsKey(accountId);
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
            }
            return false;
        }

        public GamePlayer GetVip(long accountId)
        {
            if (accountId < 1) return null;

            try
            {
                GamePlayer vip = null;
                int pos = 0;
                if (!_mapAccountToPosition.TryGetValue(accountId, out pos)) return null;

                _mapPositionToPlayer.TryGetValue(pos, out vip);
                return vip;
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
            }
            return null;
        }

        public void AddVip(GamePlayer vip, int position)
        {
            if (vip == null || position < 1 || position > _maxVip) return;

            try
            {
                if (!_mapAccountToPosition.ContainsKey(vip.AccountID))
                {
                    _mapAccountToPosition.TryAdd(vip.AccountID, position);
                    _mapPositionToPlayer.TryAdd(position, vip);
                }
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
            }
        }

        public GamePlayer RemoveVip(long accountId)
        {
            if (accountId < 1) return null;

            try
            {
                GamePlayer vip = null;
                int pos = 0;
                if (!_mapAccountToPosition.TryGetValue(accountId, out pos)) return null;

                _mapPositionToPlayer.TryRemove(pos, out vip);
                _mapAccountToPosition.TryRemove(accountId, out pos);
                return vip;
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
            }
            return null;
        }

        public void RegisterLeaveGame(long accountId)
        {
            if (_leaveGameList.Contains(accountId)) return;
            _leaveGameList.Add(accountId);
        }

        public void UnregisterLeaveGame(long accountId)
        {
            if (!_leaveGameList.Contains(accountId)) return;
            _leaveGameList.Remove(accountId);
        }

        public List<long> RemovePlayerAfterEndSession()
        {
            this.RemovePlayerNotAction();

            List<long> outGame = new List<long>();
            outGame.AddRange(_leaveGameList);
            for (int i = 0; i < _leaveGameList.Count; i++)
            {
                this.RemoveVip(_leaveGameList[i]);
            }

            _leaveGameList.Clear();
            return outGame;
        }

        public void RemovePlayerNotAction()
        {
            var players = PlayerHandler.Instance.Players();
            foreach (var player in players)
            {
                if (player.IdleTime() < _maxIdle) continue;

                ConnectionHandler.Instance.PlayerLeave(player.AccountID, "Bạn đã không có hành động gì trong 2 phút!");
                NLogManager.LogMessage(string.Format("{0} - {1} bạn đã không có hành động gì trong 2 phút", KeoNguaHandler.Instance.SessionID, player.AccountID));
                PlayerHandler.Instance.RemovePlayer(player.AccountID);
                KeoNguaHandler.Instance.SummaryPlayer();
                if (this.CheckVip(player.AccountID)) this.RegisterLeaveGame(player.AccountID);
            }
        }

        public void Refresh()
        {
            _mapAccountToPosition.Clear();
            _mapPositionToPlayer.Clear();
            _leaveGameList.Clear();
        }

        public void Dispose()
        {
            this.Refresh();
        }
    }
}