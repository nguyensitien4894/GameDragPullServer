using KeoNgua.Server.DataAccess.Dao;
using KeoNgua.Server.DataAccess.Dto;
using KeoNgua.Server.DataAccess.Factory;
using KeoNgua.Server.Hubs;
using KeoNgua.Server.Models;
using Microsoft.AspNet.SignalR;
using MsWebGame.RedisCache.Cache;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TraditionGame.Utilities;

namespace KeoNgua.Server.Handlers
{
    public class KeoNguaHandler : IDisposable
    {
        private static readonly Lazy<KeoNguaHandler> _instance = new Lazy<KeoNguaHandler>(() => new KeoNguaHandler());
        private readonly IKeoNguaDao _gameDao = AbstractFactory.Instance().CreateGameDao();

        //Waiting:1; Shaking:2; Betting:20; EndBetting:3,  OpenPlate:3; ShowResult:7
        private readonly int[] _timeConfig = { 1, 2, 20, 3, 3, 7 };
        private readonly int _betSideEnumCount = Enum.GetNames(typeof(BetSide)).Length;

        //gourd-crab-shrimp-fish-chicken-deer
        private readonly ConcurrentDictionary<long, long> _betGourd = new ConcurrentDictionary<long, long>();
        private readonly ConcurrentDictionary<long, long> _betCrab = new ConcurrentDictionary<long, long>();
        private readonly ConcurrentDictionary<long, long> _betFish = new ConcurrentDictionary<long, long>();
        private readonly ConcurrentDictionary<long, long> _betChicken = new ConcurrentDictionary<long, long>();
        private readonly ConcurrentDictionary<long, long> _betShrimp = new ConcurrentDictionary<long, long>();
        private readonly ConcurrentDictionary<long, long> _betDeer = new ConcurrentDictionary<long, long>();

        IHubContext Context = GlobalHost.ConnectionManager.GetHubContext<KeoNguaHub>();
        private object _locker = new object();
        private ConcurrentDictionary<int, List<BetLog>> _betLogs = new ConcurrentDictionary<int, List<BetLog>>();
        private List<ResultDice> resultDices = new List<ResultDice>();
        private bool _returningData { get; set; }
        private Timer _sessionTimer;
        private Timer _elapsedTimer;

        public static KeoNguaHandler Instance
        {
            get { return _instance.Value; }
        }

        #region properties
        public Phrases Phrase { get; private set; }
        public long SessionID { get; private set; }
        public int Elapsed { get; private set; }

        public long TotalBetGourd { get { return _betGourd.Sum(x => x.Value); } }
        public long TotalBetCrab { get { return _betCrab.Sum(x => x.Value); } }
        public long TotalBetFish { get { return _betFish.Sum(x => x.Value); } }
        public long TotalBetChicken { get { return _betChicken.Sum(x => x.Value); } }
        public long TotalBetShrimp { get { return _betShrimp.Sum(x => x.Value); } }
        public long TotalBetDeer { get { return _betDeer.Sum(x => x.Value); } }

        public DiceResult Result = new DiceResult();
        #endregion properties

        public int Bet(string nickName, GamePlayer player, long betValue, int betSide, out long sumaryBet, out long balance, out string msgError)
        {
            sumaryBet = 0;
            balance = 0;
            msgError = string.Empty;
            try
            {
                long accountId = player.AccountID;
                int serviceId = player.Account.ServiceID;
                int deviceId = player.Account.DeviceID;

                if (Monitor.TryEnter(_locker, _timeConfig[(int)Phrases.Betting] * 1000))
                {
                    if (this.Phrase != Phrases.Betting)
                    {
                        msgError = "Vui lòng chờ phiên tiếp theo!";
                        return (int)ErrorCode.InvalidTime;
                    }

                    int response = _gameDao.Bet(this.SessionID, accountId, serviceId, deviceId, betSide, betValue, out balance);
                    if (response < 0)
                    {
                        switch (response)
                        {
                            case -1:
                                msgError = "Phiên không tồn tại!";
                                return response;
                            case -504:
                                msgError = "Số dư của bạn không đủ!";
                                return response;
                            case -105:
                                msgError = "Tài khoản không tồn tại!";
                                return response;
                            case -106:
                                msgError = "Dữ liệu đặt cược không hợp lệ!";
                                return response;
                            case -107:
                                msgError = "Đặt cược quá giới hạn!";
                                return response;
                            case -108:
                                msgError = "Đặt sai cửa!";
                                return response;
                            default:
                                msgError = "Lỗi không xác định!";
                                return response;
                        }
                    }

                    this.UpdateBalance(player, balance);
                    Context.Clients.All.playerBet(accountId, betValue, betSide, balance);

                    if (betSide == (int)BetSide.Gourd)
                    {
                        sumaryBet = _betGourd.AddOrUpdate(accountId, betValue, (k, v) => v += betValue);
                    }

                    if (betSide == (int)BetSide.Crab)
                    {
                        sumaryBet = _betCrab.AddOrUpdate(accountId, betValue, (k, v) => v += betValue);
                    }

                    if (betSide == (int)BetSide.Fish)
                    {
                        sumaryBet = _betFish.AddOrUpdate(accountId, betValue, (k, v) => v += betValue);
                    }

                    if (betSide == (int)BetSide.Chicken)
                    {
                        sumaryBet = _betChicken.AddOrUpdate(accountId, betValue, (k, v) => v += betValue);
                    }

                    if (betSide == (int)BetSide.Shrimp)
                    {
                        sumaryBet = _betShrimp.AddOrUpdate(accountId, betValue, (k, v) => v += betValue);
                    }

                    if (betSide == (int)BetSide.Deer)
                    {
                        sumaryBet = _betDeer.AddOrUpdate(accountId, betValue, (k, v) => v += betValue);
                    }

                    List<BetLog> betLogs;
                    if (!_betLogs.TryGetValue(betSide, out betLogs))
                    {
                        betLogs = new List<BetLog>();
                        betLogs.Add(new BetLog(accountId, betValue, betSide, nickName,false));
                        _betLogs.TryAdd(betSide, betLogs);
                    }
                    else
                    {
                        betLogs.Add(new BetLog(accountId, betValue, betSide, nickName,false));
                    }

                    ThreadPool.QueueUserWorkItem(
                    o => {
                        string keyHu = CachingHandler.Instance.GeneralRedisKey("KeoNgua", "UserBet:" + this.SessionID + ":" + (BetSide)betSide);
                        RedisCacheProvider _cachePvd = new RedisCacheProvider();
                        _cachePvd.Set(keyHu, betLogs, 1);
                    });

                    string keyHuNewResult = CachingHandler.Instance.GeneralRedisKey("KeoNgua", "Betting:"+ this.SessionID+":"+((BetSide)betSide).ToString());
                    RedisCacheProvider _cachePvdNewResult = new RedisCacheProvider();
                    if (_cachePvdNewResult.Exists(keyHuNewResult))
                    {
                        _cachePvdNewResult.IncrementValue(keyHuNewResult, betValue);
                    }
                    else
                    {
                        _cachePvdNewResult.Set(keyHuNewResult, betValue);
                    }

                    this.OnChangedBettingData();
                    return (int)ErrorCode.Success;
                }
                else
                {
                    msgError = "Hệ thống của chúng tôi đang bận, xin bạn vui lòng thử lại!";
                    return (int)ErrorCode.Undefined;
                }
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                msgError = ex.Message;
                return (int)ErrorCode.Exception;
            }
            finally
            {
                Monitor.Exit(_locker);
            }
        }
        
        public List<BetSuccess> GetBetOfAccount(long accountId)
        {
            try
            {
                List<BetSuccess> lstRs = new List<BetSuccess>();
                if (_betGourd.ContainsKey(accountId))
                {
                    long betVal = 0;
                    _betGourd.TryGetValue(accountId, out betVal);
                    BetSuccess result = new BetSuccess(accountId, betVal, (int)BetSide.Gourd, betVal);
                    lstRs.Add(result);
                }

                if (_betCrab.ContainsKey(accountId))
                {
                    long betVal = 0;
                    _betCrab.TryGetValue(accountId, out betVal);
                    BetSuccess result = new BetSuccess(accountId, betVal, (int)BetSide.Crab, betVal);
                    lstRs.Add(result);
                }

                if (_betFish.ContainsKey(accountId))
                {
                    long betVal = 0;
                    _betFish.TryGetValue(accountId, out betVal);
                    BetSuccess result = new BetSuccess(accountId, betVal, (int)BetSide.Fish, betVal);
                    lstRs.Add(result);
                }

                if (_betChicken.ContainsKey(accountId))
                {
                    long betVal = 0;
                    _betChicken.TryGetValue(accountId, out betVal);
                    BetSuccess result = new BetSuccess(accountId, betVal, (int)BetSide.Chicken, betVal);
                    lstRs.Add(result);
                }

                if (_betShrimp.ContainsKey(accountId))
                {
                    long betVal = 0;
                    _betShrimp.TryGetValue(accountId, out betVal);
                    BetSuccess result = new BetSuccess(accountId, betVal, (int)BetSide.Shrimp, betVal);
                    lstRs.Add(result);
                }

                if (_betDeer.ContainsKey(accountId))
                {
                    long betVal = 0;
                    _betDeer.TryGetValue(accountId, out betVal);
                    BetSuccess result = new BetSuccess(accountId, betVal, (int)BetSide.Deer, betVal);
                    lstRs.Add(result);
                }

                return lstRs;
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
            }
            return null;
        }

        public void SummaryPlayer()
        {
            try
            {
                int count = PlayerHandler.Instance.Count();
                int summary = count - VipHandler.Instance.GetMaxVip();
                if (summary < 0) summary = 0;

                Context.Clients.All.summaryPlayer(summary);
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
            }
        }

        public List<List<BetLog>> GetBetLogs()
        {
            try
            {
                if (_betLogs == null) return new List<List<BetLog>>();

                List<List<BetLog>> lstRs = new List<List<BetLog>>(_betSideEnumCount);
                for (int i = 1; i <= _betSideEnumCount; i++)
                {
                    List<BetLog> gateLogs;
                    _betLogs.TryGetValue(i, out gateLogs);
                    if (gateLogs == null || !gateLogs.Any())
                    {
                        lstRs.Add(new List<BetLog>(1));
                    }
                    else
                    {
                        lstRs.Add(new List<BetLog>(gateLogs));
                    }
                }

                return lstRs;
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
            }

            return null;
        }
        
        #region private
        private void Init()
        {
            List<string> Tmp = new List<string>();
            for (int i = 1; i < 7; i++)
            {
                for (int ii = 1; ii < 7; ii++)
                {
                    for (int iii = 1; iii < 7; iii++)
                    {
                        List<int> Dice = new List<int> { i, ii, iii };
                        Dice.Sort((a,b) => { return a - b; });
                        string keyVal = string.Join("-", Dice);
                        if (!Tmp.Contains(keyVal))
                        {
                            Tmp.Add(keyVal);
                            Dictionary<BetSide, double> Hs = new Dictionary<BetSide, double>();
                            int Percent = 0;
                            for (int iiii = 0; iiii < 3; iiii++)
                            {
                                BetSide key = (BetSide)Dice[iiii];
                                if (!Hs.ContainsKey(key))
                                {
                                    Hs[key] = 0;
                                    Percent+=1;
                                }
                                Hs[key] += 1d;
                            }
                            this.resultDices.Add(new ResultDice(Dice.ToArray(), Hs, Percent));
                        }
                    }
                }
            }
        }
        private KeoNguaHandler()
        {
            try
            {
                Init();
                this.Phrase = Phrases.Waiting;
                _sessionTimer = new Timer(new TimerCallback(SessionCallBack), null, 3000, -1);
                _elapsedTimer = new Timer(new TimerCallback(SynchronizeTime), null, -1, -1);
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
            }
        }

        private void ReturnChangeVips()
        {
            try
            {
                VipHandler.Instance.ProcessChangeVips();
                this.SummaryPlayer();
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
            }
        }

        private void UpdateBalance(GamePlayer player, long balance)
        {
            try
            {
                player.Account.Balance = balance;

                if (VipHandler.Instance.CheckVip(player.AccountID)) VipHandler.Instance.UpdateBalanceVip(player.AccountID, balance);
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
            }
        }

        private void GameHistory()
        {
            try
            {
                List<SoiCau> lstRs = _gameDao.GetSoiCau();
                if (lstRs != null && lstRs.Any()) Context.Clients.All.gameHistory(lstRs);
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
            }
        }
        private void Calculator(out int dice1, out int dice2, out int dice3)
        {
            dice1 = 0;
            dice2 = 0;
            dice3 = 0;
            RedisCacheProvider _cachePvdNewResult = new RedisCacheProvider();
            Dictionary<BetSide,long> SumSide = new Dictionary<BetSide,long>();


            string keyHu = CachingHandler.Instance.GeneralRedisKey("KeoNgua", "RoomBank");
            RedisCacheProvider _cachePvd = new RedisCacheProvider();
            double prizeFund = 0;
            if (_cachePvd.Exists(keyHu))
            {
                prizeFund = _cachePvd.Get<double>(keyHu);
            }


            foreach (BetSide betSide in (BetSide[])Enum.GetValues(typeof(BetSide)))
            {
                string keyHuNewResult = CachingHandler.Instance.GeneralRedisKey("KeoNgua", "Betting:" + this.SessionID + ":" + ((BetSide)betSide).ToString());

                if (_cachePvdNewResult.Exists(keyHuNewResult))
                {
                    SumSide.Add(betSide, _cachePvdNewResult.Get<long>(keyHuNewResult));
                    
                }
                else
                {
                    SumSide.Add(betSide,0);
                }
            }

            List<ResultIndex> userWin = new List<ResultIndex>();
            List<ResultIndex> userNonmal = new List<ResultIndex>();
            List<ResultIndex> botWin = new List<ResultIndex>();
            NLogManager.LogError("prizeFund:"+ prizeFund);
            for (int i = 0 ; i < resultDices.Count ; i++)
            {
                ResultDice a = resultDices[i];
                double TongThang = 0;
                double BotThang = 0;

                foreach (var val in SumSide)
                {
                    if (val.Value > 0)
                    {
                        if (a.Rate.ContainsKey(val.Key) && a.Rate[val.Key] > 0) {
                            TongThang += (val.Value * a.Rate[val.Key]);
                        }else
                        {
                            BotThang += val.Value;
                        }
                    }
                }
               
                if (TongThang > 0)
                {
                    NLogManager.LogError("TongThang(" + i + "): TongThang:" + TongThang + " BotThang:" + BotThang + " TongThang >= prizeFund=" + (TongThang <= prizeFund ? "true" : "false"));
                    if (TongThang <= prizeFund/2)
                    {
                        userWin.Add(new ResultIndex() { Index = i, Win = TongThang, Lost = BotThang });
                    }
                    else
                    {
                        userNonmal.Add(new ResultIndex() { Index = i, Win = TongThang, Lost = BotThang });
                    }
                }
                else
                {
                    botWin.Add(new ResultIndex() { Index = i, Win = TongThang, Lost = BotThang });
                }
            }
            double[] Idx = new double[] { 0, 0, 0 };

            NLogManager.LogError(Newtonsoft.Json.JsonConvert.SerializeObject(SumSide));
            NLogManager.LogError(Newtonsoft.Json.JsonConvert.SerializeObject(userWin));
            NLogManager.LogError(Newtonsoft.Json.JsonConvert.SerializeObject(botWin));
            NLogManager.LogError(Newtonsoft.Json.JsonConvert.SerializeObject(userNonmal));

            if (userWin.Count > 0 || botWin.Count > 0) 
            {
                {
                    List<double[]> rate = new List<double[]>();
                    Random rnd = new Random();

                    for (int ii = 0; ii < botWin.Count; ii++)
                    {
                        var val = botWin[ii];
                        ResultDice result = resultDices[val.Index];
                        int n = result.Percent * 20;
                        for (int i = 0; i < n; i++)
                        {
                            rate.Add(new double[] { val.Index, val.Win, val.Lost });
                        }
                    }
                    double _prizeFund = prizeFund / 2d;
                    for (int ii =0; ii < userWin.Count; ii++) 
                    {
                        var val = userWin[ii];
                        ResultDice result = resultDices[val.Index];
                        double Coin = val.Win - val.Lost;
                        int n = result.Percent * 5;

                        if (Coin > 0)
                        {
                             n = n - n * (Convert.ToInt32(Coin / _prizeFund)) + 1;
                        }

                        for (int i = 0; i < n; i++)
                        {
                            rate.Add(new double[] { val.Index, val.Win,val.Lost });
                        }
                    }
                    
                    List<double[]> MyRandomArray = rate.OrderBy(x => rnd.Next()).ToList();
                    NLogManager.LogError("MyRandomArray:"+ MyRandomArray.Count);
                    Idx = MyRandomArray[rnd.Next(0, MyRandomArray.Count)];
                }
            }
            else
            {
                Dictionary<double, List<double[]>> keyValuePairs = new Dictionary<double, List<double[]>>();
                List<double> CoinWin = new List<double>();
                Random rnd = new Random();
                for (int ii = 0; ii < userNonmal.Count; ii++)
                {
                    var val = userNonmal[ii];
                    if (!keyValuePairs.ContainsKey(val.Win))
                    {
                        keyValuePairs[val.Win] = new List<double[]>();
                        CoinWin.Add(val.Win);
                    }
                    keyValuePairs[val.Win].Add(new double[] { val.Index, val.Win, val.Lost });
                }

                List<double> ascendingOrder = CoinWin.OrderBy(i => i).ToList();

                if (ascendingOrder.Count > 0)
                {
                    if (keyValuePairs.ContainsKey(ascendingOrder[0]))
                    {
                        List<double[]> Tmp = keyValuePairs[ascendingOrder[0]];
                        Idx = Tmp[rnd.Next(0, Tmp.Count)];
                    }
                }
            }
            if (Idx[0] + Idx[1] + Idx[2] > 0)
            {
                ResultDice resultDice = resultDices[int.Parse(Idx[0].ToString())];



                int[] dice = resultDice.GetDice();

                dice1 = dice[0];
                dice2 = dice[1];
                dice3 = dice[2];


                double bankWin = Idx[2] - Idx[1];
                if (bankWin > 0)
                    prizeFund += bankWin * 0.95d;// trinh 95 va bank
                else
                    prizeFund += bankWin;

                _cachePvd.Set(keyHu, prizeFund);

                NLogManager.LogError(resultDice.StringDice(dice));
            }

            foreach (BetSide betSide in (BetSide[])Enum.GetValues(typeof(BetSide)))
            {
                string keyHuNewResult = CachingHandler.Instance.GeneralRedisKey("KeoNgua", "Betting:" + this.SessionID + ":" + ((BetSide)betSide).ToString());

                if (_cachePvdNewResult.Exists(keyHuNewResult))
                {
                    _cachePvdNewResult.Remove(keyHuNewResult);
                }    
            }

        }
        private void CreateManual()
        {
            try
            {
                int dice1 = 0;
                int dice2 = 0;
                int dice3 = 0;

                try
                {
					 string keyHu = CachingHandler.Instance.GeneralRedisKey("KeoNgua", "RoomFunds");
                    string keyHuNewResult = CachingHandler.Instance.GeneralRedisKey("KeoNgua", "NewResult");
                    RedisCacheProvider _cachePvdNewResult = new RedisCacheProvider();
                    //_cachePvdNewResult.Set(keyHu, -9999999999999);
                    if (_cachePvdNewResult.Exists(keyHuNewResult))
                    {
                        int[] NewResult = _cachePvdNewResult.Get<int[]>(keyHuNewResult);
                        if (NewResult != null)
                        {
                            Random rnd = new Random();

                            NewResult = NewResult.OrderBy(x => rnd.Next()).ToArray();
                            dice1 = NewResult[0];
                            dice2 = NewResult[1];
                            dice3 = NewResult[2];
                    // string keyHuNewResult = CachingHandler.Instance.GeneralRedisKey("KeoNgua", "NewResult");
                    // RedisCacheProvider _cachePvdNewResult = new RedisCacheProvider();

                    // if (_cachePvdNewResult.Exists(keyHuNewResult))
                    // {
                        // int[] NewResult = _cachePvdNewResult.Get<int[]>(keyHuNewResult);
                        // if (NewResult != null)
                        // {
                            // Random rnd = new Random();
                             
                            // NewResult = NewResult.OrderBy(x => rnd.Next()).ToArray();
                            // dice1 = NewResult[0];
                            // dice2 = NewResult[1];
                            // dice3 = NewResult[2];
                        }
                    }
                    _cachePvdNewResult.Set(keyHuNewResult, new int[] { 0, 0, 0 });
                }
                catch (Exception ex)
                {
                    NLogManager.PublishException(ex);
                }
                if (dice1 + dice2 + dice3 < 3)
                    this.Calculator(out dice1,out dice2,out dice3);

                if (dice1 + dice2 + dice3 < 3)
                {
                    _gameDao.CreateManualDice(this.SessionID, out dice1, out dice2, out dice3);
                }
                if (dice1 + dice2 + dice3 >= 3)
                {
                    this.Result.GetNewResult(dice1, dice2, dice3);
                }
                else
                {
                    this.Result.GetNewResult();
                }

            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
            }
        }

        private void CallFinishSession()
        {
            try
            {
                var result = this.Result.Clone() as DiceResult;
                if (result.Dice1 < 1)
                {
                    this.CreateManual();
                    result = this.Result.Clone() as DiceResult;
                }

                _gameDao.FinishSession(this.SessionID, result);
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
            }
        }

        private void GetAwardSession()
        {
            try
            {
                List<SessionResult> lstRs = _gameDao.GetAwardSession(this.SessionID);
                if (lstRs != null && lstRs.Any())
                {
                    foreach (var result in lstRs)
                    {
                        if (result.AccountID <= Config.MAX_BOTID) continue;

                        var lstConn = ConnectionHandler.Instance.GetConnections(result.AccountID);
                        if (lstConn != null && lstConn.Any())
                        {
                            foreach (var conn in lstConn)
                            {
                                Context.Clients.Client(conn).winResult(result);
                            }
                        }
                    }

                    List<long> accounts = VipHandler.Instance.GetVipKeys();
                    if (accounts != null && accounts.Any())
                    {
                        var results = lstRs.Where(x => accounts.Contains(Math.Abs(x.AccountID))).ToList();
                        if (results != null && results.Any())
                        {
                            foreach (var item in results)
                            {
                                if (item.AccountID > Config.MAX_BOTID) continue;

                                item.AccountID = Math.Abs(item.AccountID);
                                item.Balance = BotHandler.Instance.GetBalance(item.AccountID, item.Award);
                            }

                            Context.Clients.All.winResultVip(results);
                        }
                    }

                    //gun total winner money
                    long totalWin = lstRs.Where(x => !accounts.Contains(Math.Abs(x.AccountID))).Sum(x => x.Award);
                    Context.Clients.All.totalWinMoney(totalWin);
                    NLogManager.LogMessage(string.Format("totalWinMoney-SessionID:{0}", this.SessionID));
                }
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
            }
        }

        private void AfterWaitingPhrase()
        {
            try
            {
                this.SessionID = _gameDao.CreateSession();
                if (this.SessionID < 1)
                {
                    //NLogManager.LogMessage(string.Format("InitErr-SessionID:{0}", this.SessionID));
                    throw new Exception(string.Format("InitErr-SessionID:{0}", this.SessionID));
                }

                NLogManager.LogMessage(string.Format("Init-SessionID:{0}", this.SessionID));
                this.NotifyChangePhrase(Phrases.Shaking);
                return;
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
            }
        }

        private void AfterShakingPhrase()
        {
            this.NotifyChangePhrase(Phrases.Betting);
            return;
        }

        private void AfterBettingPhrase()
        {
            this.NotifyChangePhrase(Phrases.EndBetting);
            return;
        }

        private void AfterEndBettingPhrase()
        {
            this.CreateManual();
            this.NotifyChangePhrase(Phrases.OpenPlate);
            return;
        }

        private void AfterOpenPlatePhrase()
        {
            this.NotifyChangePhrase(Phrases.ShowResult);
            return;
        }

        private void AfterShowResultPhrase()
        {
            ThreadPool.QueueUserWorkItem(_ =>
            {
                try
                {
                    BotHandler.Instance.CheckBotInValid();
                }
                catch (Exception ex)
                {
                    NLogManager.PublishException(ex);
                }
            });
            this.RefreshSession();
            this.NotifyChangePhrase(Phrases.Waiting);
            return;
        }

        private void NotifyChangePhrase(Phrases phrase)
        {
            this.Phrase = phrase;
            int interval = _timeConfig[(int)phrase];
            NLogManager.LogMessage(string.Format("NotifyChangePhrase:{0}-{1}", phrase, interval));
            try
            {
                switch (phrase)
                {
                    case Phrases.Waiting:
                        this.ReturnChangeVips();
                        this.GameHistory();
                        break;
                    case Phrases.Shaking:
                        break;
                    case Phrases.Betting:
                        _betLogs = new ConcurrentDictionary<int, List<BetLog>>(Environment.ProcessorCount, _betSideEnumCount);
                        ThreadPool.QueueUserWorkItem(_ =>
                        {
                            try
                            {
                                BotHandler.Instance.StartBet();
                            }
                            catch (Exception ex)
                            {
                                NLogManager.PublishException(ex);
                            }
                        });
                        break;
                    case Phrases.EndBetting:
                        break;
                    case Phrases.OpenPlate:
                        ThreadPool.QueueUserWorkItem(_ =>
                        {
                            try
                            {
                                this.CallFinishSession();
                            }
                            catch (Exception ex)
                            {
                                NLogManager.PublishException(ex);
                            }
                        });
                        break;
                    case Phrases.ShowResult:
                        ThreadPool.QueueUserWorkItem(_ =>
                        {
                            try
                            {
                                this.GetAwardSession();
                            }
                            catch (Exception ex)
                            {
                                NLogManager.PublishException(ex);
                            }
                        });
                        break;
                }

                ThreadPool.QueueUserWorkItem(_ =>
                {
                    try
                    {
                        Context.Clients.All.notifyChangePhrase(this, interval, phrase);
                    }
                    catch (Exception ex)
                    {
                        NLogManager.PublishException(ex);
                    }
                    finally
                    {
                        this.StartTimer(interval);
                    }
                });
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                this.StartTimer(interval);
            }
        }

        private void RestartGame()
        {
            try
            {
                this.RefreshSession();
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
            }
            finally
            {
                this.NotifyChangePhrase(Phrases.Waiting);
            }
        }

        private void OnChangedBettingData()
        {
            if (!_returningData) _returningData = true;
        }

        private void SessionCallBack(object o)
        {
            //if (!Monitor.TryEnter(_locker, 3000)) return;

            NLogManager.LogMessage(string.Format("SessionCallBack:{0}-{1}", this.Phrase, this.Elapsed));
            try
            {
                switch (this.Phrase)
                {
                    case Phrases.Waiting:
                        this.AfterWaitingPhrase();
                        break;
                    case Phrases.Shaking:
                        this.AfterShakingPhrase();
                        break;
                    case Phrases.Betting:
                        this.AfterBettingPhrase();
                        break;
                    case Phrases.EndBetting:
                        this.AfterEndBettingPhrase();
                        break;
                    case Phrases.OpenPlate:
                        this.AfterOpenPlatePhrase();
                        break;
                    case Phrases.ShowResult:
                        this.AfterShowResultPhrase();
                        break;
                }
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                this.RestartGame();
            }
            //finally
            //{
            //    Monitor.Exit(_locker);
            //}
        }

        private void SynchronizeTime(object o)
        {
            if (this.Elapsed > 0)
            {
                Context.Clients.All.updateRoomTime(this.Elapsed);
                ThreadPool.QueueUserWorkItem(oo => {
                    string keyHu = CachingHandler.Instance.GeneralRedisKey("KeoNgua", "Session");
                    RedisCacheProvider _cachePvd = new RedisCacheProvider();
                    _cachePvd.Set(keyHu, this.SessionID + "|" + this.Phrase + "|" + this.Elapsed);
                });
                --this.Elapsed;
            }

            if (_returningData)
            {
                _returningData = false;
                Context.Clients.All.sessionInfo(this);
            }
        }

        private void StartTimer(int interval)
        {
            int tmp = this.Phrase == Phrases.Betting ? interval - 3 : interval; 
            _sessionTimer.Change(tmp * 1000, Timeout.Infinite);
            this.Elapsed = interval;
            _elapsedTimer.Change(0, 1000);
        }

        private void StopTimer()
        {
            this.Elapsed = 0;
            _sessionTimer.Change(Timeout.Infinite, Timeout.Infinite);
            _elapsedTimer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        private void RefreshSession()
        {
            this.Phrase = Phrases.Waiting;
            _betGourd.Clear();
            _betCrab.Clear();
            _betFish.Clear();
            _betChicken.Clear();
            _betShrimp.Clear();
            _betDeer.Clear();

            _betLogs = new ConcurrentDictionary<int, List<BetLog>>();
            _returningData = false;
            this.Result.ClearResult();
        }
        #endregion private


        #region Bot
        public long BotBet(GamePlayer player, BotBetInfo betInfo)
        {
            long summaryBet = 0;
            long accountId = betInfo.AccountID;
            long betValue = betInfo.BetValue;
            int betSide = betInfo.BetSide;
            int deviceId = player.Account.DeviceID;
            int serviceId = player.Account.ServiceID;
            try
            {
                if (Monitor.TryEnter(_locker, (_timeConfig[(int)Phrases.Betting]) * 1000))
                {
                    if (this.Phrase != Phrases.Betting || this.SessionID < 1)
                        throw new Exception("Bot trạng thái game không hợp lệ");

                    long botId = betInfo.BotID;
                    int response = _gameDao.BotBet(this.SessionID, botId, serviceId, deviceId, betSide, betValue);
                    if (response < 0) throw new Exception("Bot bet không thành công");

                    BotHandler.Instance.UpdateBotInfo(accountId, -betValue, this.SessionID);
                    long balance = BotHandler.Instance.GetBalance(accountId, 0);
                    this.UpdateBalance(player, balance);
                    Context.Clients.All.playerBet(accountId, betValue, betSide, balance);

                    if (betSide == (int)BetSide.Gourd)
                    {
                        summaryBet = _betGourd.AddOrUpdate(accountId, betValue, (k, v) => v += betValue);
                    }

                    if (betSide == (int)BetSide.Crab)
                    {
                        summaryBet = _betCrab.AddOrUpdate(accountId, betValue, (k, v) => v += betValue);
                    }
                    
                    if (betSide == (int)BetSide.Fish)
                    {
                        summaryBet = _betFish.AddOrUpdate(accountId, betValue, (k, v) => v += betValue);
                    }

                    if (betSide == (int)BetSide.Chicken)
                    {
                        summaryBet = _betChicken.AddOrUpdate(accountId, betValue, (k, v) => v += betValue);
                    }

                    if (betSide == (int)BetSide.Shrimp)
                    {
                        summaryBet = _betShrimp.AddOrUpdate(accountId, betValue, (k, v) => v += betValue);
                    }

                    if (betSide == (int)BetSide.Deer)
                    {
                        summaryBet = _betDeer.AddOrUpdate(accountId, betValue, (k, v) => v += betValue);
                    }

                    List<BetLog> betLogs;
                    if (!_betLogs.TryGetValue(betSide, out betLogs))
                    {
                        betLogs = new List<BetLog>();
                        betLogs.Add(new BetLog(accountId, betValue, betSide));
                        _betLogs.TryAdd(betSide, betLogs);
                    }
                    else
                    {
                        betLogs.Add(new BetLog(accountId, betValue, betSide));
                    }

                    this.OnChangedBettingData();
                    return betValue;
                }
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
            }
            finally
            {
                Monitor.Exit(_locker);
            }

            return 0;
        }
        #endregion Bot


        public void Dispose()
        {
            this.RefreshSession();
            if (_elapsedTimer != null) _elapsedTimer.Dispose();

            if (_sessionTimer != null) _sessionTimer.Dispose();
        }
    }
}