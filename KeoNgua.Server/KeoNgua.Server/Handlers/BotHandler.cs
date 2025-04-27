using KeoNgua.Server.DataAccess.Dao;
using KeoNgua.Server.DataAccess.Factory;
using KeoNgua.Server.DataAccess.Dto;
using KeoNgua.Server.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using TraditionGame.Utilities;
using TraditionGame.Utilities.Utils;
using System.Threading.Tasks;

namespace KeoNgua.Server.Handlers
{
    public class BotHandler : IDisposable
    {
        private static readonly Lazy<BotHandler> _instance = new Lazy<BotHandler>(() => new BotHandler());

        private readonly IKeoNguaDao _gameDao = AbstractFactory.Instance().CreateGameDao();
        private readonly int _gameId = Int32.Parse(ConfigurationManager.AppSettings["GameID"].ToString());
        private readonly int _betSideEnumCount = Enum.GetNames(typeof(BetSide)).Length;
        private readonly int _timeCallback = Int32.Parse(ConfigurationManager.AppSettings["TimeCallback"].ToString());
        private readonly ConcurrentDictionary<long, BotDB> _bots = new ConcurrentDictionary<long, BotDB>();

        #region config bot
        private readonly int[] _groupBotNumber = { 15, 17 };
        private readonly int _timeActiveBot = 17;
        private readonly int _maxIdle = 120;
        private readonly int _maxBetSmall = 50000;
        private readonly int _minBetMedium = 5000;
        private readonly int _maxBetMedium = 500000;
        private readonly int _minBetLarge = 50000;
        private readonly long _minBalance = 5000;
        private readonly long _maxBalanceSmall = 1000000;
        private readonly long _maxBalanceMedium = 10000000;
        private readonly int _numberMaxBet = 4;
        #endregion config bot

        private int[] _betValues = new int[100];
        private int[] _largeBetValues = new int[100];
        private int[] _mediumBetValues = new int[100];
        private int[] _smallBetValues = new int[100];
        private Timer _initBotTimer;


        public static BotHandler Instance
        {
            get { return _instance.Value; }
        }

        private BotHandler()
        {
            _initBotTimer = new Timer(InitBotCallBack, null, 1000, Timeout.Infinite);
        }


        #region hub_bot
        private AccountDB EnterLobby(BotDB bot)
        {
            try
            {
                if (bot == null) return null;

                if (PlayerHandler.Instance.Contains(bot.AbsAccountID)) PlayerHandler.Instance.RemovePlayer(bot.AbsAccountID);

                var account = new AccountDB(bot.AbsAccountID, bot.NickName, bot.Star, bot.DeviceID, bot.ServiceID, bot.Avatar, 1);
                NLogManager.LogMessage(string.Format("{0}({1}) - Balance:{2} - Step:{3}", bot.NickName, bot.AccountID, bot.Star, bot.Step));
                var player = PlayerHandler.Instance.AddPlayer(account);
                if (player != null)
                {
                    BotDB tmp;
                    if (_bots.ContainsKey(bot.AbsAccountID)) _bots.TryRemove(bot.AbsAccountID, out tmp);

                    _bots.TryAdd(bot.AbsAccountID, bot);
                    return account;
                }
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
            }

            return null;
        }

        private void ExitLobby(long accountId)
        {
            try
            {
                if (VipHandler.Instance.CheckVip(accountId)) VipHandler.Instance.RegisterLeaveGame(accountId);

                PlayerHandler.Instance.RemovePlayer(accountId);
                KeoNguaHandler.Instance.SummaryPlayer();
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
            }
        }

        private void PlayNow(long accountId, string nickName)
        {
            try
            {
                if (accountId < 1 || string.IsNullOrEmpty(nickName)) throw new Exception("Dữ liệu đầu vào không hợp lệ");

                VipHandler.Instance.UnregisterLeaveGame(accountId);
                var player = PlayerHandler.Instance.GetPlayer(accountId);
                if (player == null) throw new Exception(string.Format("Bot playerNull=> {0}({1})", nickName, accountId));

                player.LastActiveTime = DateTime.Now;
                VipHandler.Instance.ProcessVipPlayNow(accountId);
                KeoNguaHandler.Instance.SummaryPlayer();
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
            }
        }

        private void Bet(BotBetInfo betInfo)
        {
            if (betInfo == null) return;

            Phrases phrase = KeoNguaHandler.Instance.Phrase;
            if (phrase != Phrases.Betting) return;

            var player = PlayerHandler.Instance.GetPlayer(betInfo.AccountID);
            if (player == null) return;

            //check balance
            if (!this.CheckBalance(betInfo.AccountID, betInfo.BetValue)) return;

            var betValue = KeoNguaHandler.Instance.BotBet(player, betInfo);
            if (betValue != 0) player.LastActiveTime = DateTime.Now;
        }
        #endregion hub_bot


        private void GroupBotBet(List<BotBetInfo> botBets)
        {
            try
            {
                if (botBets == null || !botBets.Any()) throw new Exception("Bot dữ liệu đầu vào không hợp lệ");

                foreach (var item in botBets)
                {
                    this.Bet(item);
                }
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
            }
        }

        private void CalculatorBotBet(List<BotBetInfo> botBets)
        {
            try
            {
                if (botBets == null || !botBets.Any()) throw new Exception("Danh sách bot không hợp lệ");

                int count = botBets.Count;
                int groups = _groupBotNumber.Last();
                if (count >= groups)
                {
                    int rnd = RandomUtil.NextByte(_groupBotNumber.Count());
                    groups = _groupBotNumber[rnd];
                }
                else
                {
                    groups = count;
                }

                int botTimeSleep = (_timeActiveBot * 1000) / groups;
                //add bot into groups
                for (int i = 0; i < count; i++)
                {
                    int g = (i % groups) + 1;
                    botBets[i].Group = g;
                }

                for (int i = 0; i < groups; i++)
                {
                    var groupBot = botBets.Where(x => x.Group == (i + 1)).ToList();
                    if (groupBot == null || !groupBot.Any()) continue;

                    this.GroupBotBet(groupBot);
                    Thread.Sleep(botTimeSleep);
                }
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
            }
        }

        private void ProcessBotBetInfo(List<GamePlayer> playerBots)
        {
            try
            {
                if (playerBots == null || !playerBots.Any()) throw new Exception("Danh sách bot không hợp lệ");

                List<BotBetInfo> results = new List<BotBetInfo>();
                foreach (var bot in playerBots)
                {
                    long botId = ConvertUtil.AccountIdBot(bot.AccountID);
                    List<BotBetInfo> result = this.GenerateBotBet(botId, bot.Account.Balance, _numberMaxBet);
                    if (result != null && result.Any())
                    {
                        results.AddRange(result);
                    }
                }

                if (results != null && results.Any())
                {
                    this.CalculatorBotBet(results);
                }
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
            }
        }

        private void ProcessBotBetFast(List<GamePlayer> playerBots)
        {
            if (playerBots == null || !playerBots.Any()) throw new Exception("Danh sách bot không hợp lệ");

            int sleep = 1 + RandomUtil.NextByte(3);
            Thread.Sleep(sleep * 1000);
            List<BotBetInfo> results = new List<BotBetInfo>();
            foreach (var bot in playerBots)
            {
                long botId = ConvertUtil.AccountIdBot(bot.AccountID);
                List<BotBetInfo> result = this.GenerateBotBet(botId, bot.Account.Balance, _numberMaxBet);

                if (result != null && result.Any())
                {
                    foreach (var item in result)
                    {
                        this.Bet(item);
                        Thread.Sleep(150);
                    }
                }

                Thread.Sleep(1000);
            }
        }

        public void StartBet()
        {
            try
            {
                Phrases phrase = KeoNguaHandler.Instance.Phrase;
                if (phrase == Phrases.Betting)
                {
                    var bots = PlayerHandler.Instance.Players().Where(p => p.AccountID <= Config.MAX_BOTID && p.Account.Balance > _minBalance)
                        .Select(p => p as GamePlayer).ToList();

                    if (bots == null || !bots.Any()) throw new Exception("Danh sách bot không hợp lệ");

                    int count = bots.Count;
                    NLogManager.LogMessage(string.Format("StartBet:{0}", count));
                    // chia bot bet
                    int numTake = count / 4;
                    if (numTake > 0)
                    {
                        List<GamePlayer> lstFast = bots.Take(numTake).ToList();
                        if (lstFast != null && lstFast.Any())
                        {
                            foreach (var item in lstFast)
                            {
                                bots.Remove(item);
                            }
                        }

                        ThreadPool.QueueUserWorkItem(_ =>
                        {
                            try
                            {
                                this.ProcessBotBetFast(lstFast);
                            }
                            catch (Exception ex)
                            {
                                NLogManager.PublishException(ex);
                            }
                        });
                    }

                    this.ProcessBotBetInfo(bots);
                }
                return;
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                this.RestartBet();
            }
        }

        private void RestartBet()
        {
            try
            {
                Phrases phrase = KeoNguaHandler.Instance.Phrase;
                if (phrase == Phrases.Betting)
                {
                    var bots = PlayerHandler.Instance.Players().Where(p => p.AccountID <= Config.MAX_BOTID && p.Account.Balance > _minBalance)
                        .Select(p => p as GamePlayer).ToList();

                    if (bots == null || !bots.Any()) throw new Exception("Danh sách bot không hợp lệ");

                    NLogManager.LogMessage(string.Format("RestartBet:{0}", bots.Count));
                    this.ProcessBotBetInfo(bots);
                }
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
            }
        }

        public void CheckBotInValid()
        {
            var bots = _bots.Values.ToList();
            if (bots == null || !bots.Any()) return;

            foreach (var bot in bots)
            {
                if (bot.Step > 0 && bot.Star > _minBalance && bot.IdleTime() < _maxIdle) continue;

                this.RemoveBot(bot.AbsAccountID);
            }
        }

        public void UpdateBotInfo(long accountId, long money, long sessionId)
        {
            var bot = this.GetBot(accountId);
            if (bot == null) return;

            if (money != 0) bot.Star += money;

            if (bot.SessionID != sessionId)
            {
                bot.Step--;
                bot.SessionID = sessionId;
            }

            bot.SetActive();
            //NLogManager.LogMessage(string.Format("BotInfo:{0}({1}) - Balance:{2}({3}) - {4}", sessionId, accountId, bot.Star, money, bot.Step));
        }

        public bool CheckBalance(long accountId, long betValue)
        {
            try
            {
                if (accountId < 1) return false;

                if (!_bots.ContainsKey(accountId)) return false;

                BotDB bot = null;
                _bots.TryGetValue(accountId, out bot);
                if (bot != null)
                {
                    bool isEnough = (bot.Star - betValue) >= 0;
                    return isEnough;
                }
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
            }
            return false;
        }

        public long GetBalance(long accountId, long money)
        {
            try
            {
                if (accountId < 1) return 0;

                if (!_bots.ContainsKey(accountId)) return 0;

                BotDB bot = null;
                _bots.TryGetValue(accountId, out bot);
                if (bot != null)
                {
                    bot.Star += money;
                    return bot.Star;
                }
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
            }
            return 0;
        }

        public BotDB GetBot(long accountId)
        {
            if (accountId < 1) return null;

            BotDB bot = null;
            if (_bots.ContainsKey(accountId))
            {
                _bots.TryGetValue(accountId, out bot);
            }
            return bot;
        }

        public void RemoveBot(long accountId)
        {
            try
            {
                if (accountId < 1) throw new Exception("Bot dữ liệu đầu vào không hợp lệ");

                BotDB bot = null;
                _bots.TryGetValue(accountId, out bot);
                if (bot != null)
                {
                    this.ExitLobby(accountId);
                    _bots.TryRemove(accountId, out bot);
                    NLogManager.LogMessage(string.Format("RemoveBot:{0}({1}) - Balance:{2} - Step:{3}", bot.NickName, accountId, bot.Star, bot.Step));
                }
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
            }
        }

        private async void BotBetAfterAddBots(BotDB bot)
        {
            try
            {
                Phrases phrase = KeoNguaHandler.Instance.Phrase;
                if (phrase != Phrases.Betting) return;

                int elapsed = KeoNguaHandler.Instance.Elapsed;
                int numMaxBet = (elapsed - 3) / 3;
                if (numMaxBet <= 0) return;

                if (numMaxBet > _numberMaxBet) numMaxBet = _numberMaxBet;

                List<BotBetInfo> result = this.GenerateBotBet(bot.AccountID, bot.Star, numMaxBet);
                if (result != null && result.Any())
                {
                    int time = 1 + RandomUtil.NextByte(3);
                    await Task.Delay(time * 150);
                    foreach (var item in result)
                    {
                        int n = RandomUtil.NextInt(10) + 5;
                        for (int ii = 0; ii < n; ii++)
                            this.Bet(item);
                    }
                }
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
            }
        }

        private List<BotBetInfo> GenerateBotBet(long botId, long balance, int numMaxBet)
        {
            try
            {
                List<BotBetInfo> results = new List<BotBetInfo>();
                int[] arrBetValues = this.GetBetValues(balance);
                int randBet = 1 + RandomUtil.NextByte(numMaxBet);

                long betVal = arrBetValues[RandomUtil.NextByte(arrBetValues.Count())];
                int betSide = 1 + RandomUtil.NextByte(_betSideEnumCount);

                for (int i = 0; i < randBet; i++)
                {
                    BotBetInfo betInfo = new BotBetInfo(botId, betVal, betSide);
                    results.Add(betInfo);
                }

                return results;
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
            }

            return null;
        }

        private int[] GetBetValues(long balance)
        {
            try
            {
                int[] result = new int[100];
                if (balance > _maxBalanceSmall && balance < _maxBalanceMedium)
                {
                    result = _mediumBetValues;
                }
                else if (balance >= _maxBalanceMedium)
                {
                    result = _largeBetValues;
                }
                else
                {
                    result = _smallBetValues;
                }

                return result;
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
            }

            return null;
        }

        private async void ProcessAddBots(List<BotDB> bots)
        {
            if (bots == null || !bots.Any()) throw new Exception("Bot dữ liệu đầu vào không hợp lệ");

            foreach (var bot in bots)
            {
                var response = this.EnterLobby(bot);
                if (response != null) this.PlayNow(bot.AbsAccountID, bot.NickName);

                int rnd = 1 + RandomUtil.NextByte(3);
                await Task.Delay(rnd * 150);
                ThreadPool.QueueUserWorkItem(_ =>
                {
                    try
                    {
                        this.BotBetAfterAddBots(bot);
                    }
                    catch (Exception ex)
                    {
                        NLogManager.PublishException(ex);
                    }
                });
            }
        }

        private void InitBotCallBack(object o)
        {
            try
            {
                string betValues = string.Empty;
                List<BotDB> bots = _gameDao.GetListCardBot(_gameId, out betValues);
                if (bots == null || !bots.Any()) throw new Exception("Danh sách bot không hợp lệ");

                NLogManager.LogMessage(string.Format("BotCallBack: {0} - {1}", bots.Count, DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")));
                if (!string.IsNullOrEmpty(betValues))
                {
                    _betValues = new int[100];
                    _largeBetValues = new int[100];
                    _mediumBetValues = new int[100];
                    _smallBetValues = new int[100];

                    string[] arrBetValue = betValues.Split(',').ToArray();
                    _betValues = Array.ConvertAll(arrBetValue, x => Int32.Parse(x));
                    _largeBetValues = Array.FindAll(_betValues, x => x >= _minBetLarge);
                    _mediumBetValues = Array.FindAll(_betValues, x => x >= _minBetMedium && x <= _maxBetMedium);
                    _smallBetValues = Array.FindAll(_betValues, x => x <= _maxBetSmall);
                }

                foreach (var item in bots)
                {
                    item.SetActive();
                }

                ThreadPool.QueueUserWorkItem(_ =>
                {
                    try
                    {
                        this.ProcessAddBots(bots);
                    }
                    catch (Exception ex)
                    {
                        NLogManager.PublishException(ex);
                    }
                });
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
            }
            finally
            {
                this.StartInitBotTimer();
            }
        }

        private void StartInitBotTimer()
        {
            _initBotTimer.Change(_timeCallback * 1000, Timeout.Infinite);
        }

        public void Refresh()
        {
            _bots.Clear();
        }

        public void Dispose()
        {
            this.Refresh();
            if (_initBotTimer != null) _initBotTimer.Dispose();
        }
    }
}