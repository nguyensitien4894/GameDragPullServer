using KeoNgua.Server.DataAccess.Dto;
using KeoNgua.Server.DataAccess.Factory;
using KeoNgua.Server.Handlers;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;
using TraditionGame.Utilities;
using TraditionGame.Utilities.Caching;
using TraditionGame.Utilities.Chat;
using TraditionGame.Utilities.Session;
using TraditionGame.Utilities.Api;

namespace KeoNgua.Server.Hubs
{
    [HubName("KeoNguaHub")]
    public class KeoNguaHub : Hub
    {
        [HubMethodName("EnterLobby")]
        public AccountDB EnterLobby(int deviceId, int vip)
        {
            if (deviceId < 1 || deviceId > 4) throw new Exception("DeviceID không hợp lệ");

            if (vip < 1) throw new Exception("dữ liệu đầu vào không hợp lệ");

            try
            {
                long accountId = AccountSession.AccountID;
                string nickName = AccountSession.AccountName;
                int serviceId = AccountSession.ServiceID;
                int avatar = AccountSession.AvatarID;
                if (accountId < 1 || string.IsNullOrEmpty(nickName) || serviceId < 1) throw new Exception("AccountID not in token");

                ConnectionHandler.Instance.PlayerConnect(accountId, Context.ConnectionId);
                if (PlayerHandler.Instance.Contains(accountId))
                {
                    return PlayerHandler.Instance.UpdateAndGet(accountId);
                }
                else
                {
                    var player = PlayerHandler.Instance.AddPlayer(accountId, nickName, deviceId, serviceId, avatar, vip);
                    if (player != null) return new AccountDB(player.Account);
                }
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
            }
            return null;
        }

        [HubMethodName("ExitLobby")]
        public void ExitLobby()
        {
            try
            {
                long accountId = AccountSession.AccountID;
                string nickName = AccountSession.AccountName;
                if (accountId < 1 || string.IsNullOrEmpty(nickName)) throw new Exception("AccountID not in token");

                if (VipHandler.Instance.CheckVip(accountId)) VipHandler.Instance.RegisterLeaveGame(accountId);

                PlayerHandler.Instance.RemovePlayer(accountId);
                KeoNguaHandler.Instance.SummaryPlayer();
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
            }
        }

        [HubMethodName("PlayNow")]
        public long PlayNow()
        {
            try
            {
                long accountId = AccountSession.AccountID;
                string nickName = AccountSession.AccountName;
                if (accountId < 1 || string.IsNullOrEmpty(nickName)) throw new Exception("AccountID not in token");

                VipHandler.Instance.UnregisterLeaveGame(accountId);
                var player = PlayerHandler.Instance.GetPlayer(accountId);
                if (player == null) throw new Exception(string.Format("PlayerNull=> {0}({1})", nickName, accountId));

                player.LastActiveTime = DateTime.Now;

                VipHandler.Instance.ProcessVipPlayNow(accountId, Context.ConnectionId);
                KeoNguaHandler.Instance.SummaryPlayer();

                List<SoiCau> gameHis = AbstractFactory.Instance().CreateGameDao().GetSoiCau();
                List<List<BetLog>> betLogs = new List<List<BetLog>>();
                List<BetSuccess> betOfAccount = new List<BetSuccess>();
                Phrases phrase = KeoNguaHandler.Instance.Phrase;
                int elapsed = KeoNguaHandler.Instance.Elapsed;
                if (phrase != Phrases.ShowResult || elapsed >= 6)
                {
                    betLogs = KeoNguaHandler.Instance.GetBetLogs();
                    betOfAccount = KeoNguaHandler.Instance.GetBetOfAccount(accountId);
                }

                var session = KeoNguaHandler.Instance;
                Clients.Caller.joinGame(player, session, gameHis, betOfAccount, betLogs);
                return accountId;
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
            }
            return (int)ErrorCode.Exception;
        }

        [HubMethodName("SendMessage")]
        public void SendMessage(string content)
        {
            try
            {
                if (string.IsNullOrEmpty(content)) throw new Exception("Dữ liệu đầu vào không hợp lệ");

                long accountId = AccountSession.AccountID;
                string nickName = AccountSession.AccountName;
                if (accountId < 1 || string.IsNullOrEmpty(nickName)) throw new Exception("AccountID not in token");

                var player = PlayerHandler.Instance.GetPlayer(accountId);
                if (player == null) throw new Exception(string.Format("PlayerNull=> {0}({1})", nickName, accountId));

                bool isVip = VipHandler.Instance.CheckVip(accountId);
                content = ChatFilter.RemoveBadWords(content);
                Clients.All.receiveMessage(nickName, content, player.Account.ServiceID, accountId, isVip, player.Account.Vip);
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
            }
        }

        [HubMethodName("Bet")]
        public int Bet(long betValue, int betSide)
        {
            try
            {
                if (betValue < 1000)
                {
                    Clients.Caller.message("Giá trị đặt cửa tối thiểu là 1000!");
                    return (int)ErrorCode.InputInvalid;
                }

                if (betSide < 1 || betSide > 6)
                {
                    Clients.Caller.message("Cửa đặt không hợp lệ!");
                    return (int)(ErrorCode.InvalidBetSide);
                }

                long accountId = AccountSession.AccountID;
                string nickName = AccountSession.AccountName;
                int serviceId = AccountSession.ServiceID;
                if (accountId < 1 || string.IsNullOrEmpty(nickName) || serviceId < 1)
                {
                    NLogManager.LogMessage(string.Format("Bet-NotAuthen-Acc:{0}", accountId));
                    Clients.Caller.message("Vui lòng đăng nhập!");
                    return (int)ErrorCode.NotAuthen;
                }

                Phrases phrase = KeoNguaHandler.Instance.Phrase;
                if (phrase != Phrases.Betting)
                {
                    Clients.Caller.message("Vui lòng chờ phiên tiếp theo!");
                    return (int)ErrorCode.InvalidTime;
                }

                float maxBetPerSecond = Int32.Parse(ConfigurationManager.AppSettings["MaxSpinPerSecond"]);
                if (maxBetPerSecond % 2 > 0) maxBetPerSecond += 1;

                long sessionId = KeoNguaHandler.Instance.SessionID;
                int cacheCounter = CacheCounter.CheckAccountActionFrequencyMiliSecond(string.Format("{0}_{1}", sessionId, accountId), 1000 / maxBetPerSecond, "Bet");
                if (cacheCounter > 1)
                {
                    NLogManager.LogMessage(string.Format("BlockBetFast=> {0} ({1}) đặt cược 1 giây {2} lần.", nickName, accountId, maxBetPerSecond));
                    return (int)ErrorCode.Duplicate;
                }

                var player = PlayerHandler.Instance.GetPlayer(accountId);
                if (player == null)
                {
                    NLogManager.LogMessage(string.Format("PlayerNull=> {0}({1})", nickName, accountId));
                    return (int)ErrorCode.NotAuthen;
                }

                long outBalance;
                long summaryBet = 0;
                string msgError = string.Empty;
                int response = KeoNguaHandler.Instance.Bet(nickName, player, betValue, betSide, out summaryBet, out outBalance, out msgError);
                if (response <= 0)
                {
                    Clients.Caller.message(msgError);
                    return response;
                }

                player.LastActiveTime = DateTime.Now;
                BetSuccess result = new BetSuccess(accountId, betValue, betSide, summaryBet);

                string cuadat = "";

                if (betSide == (int)BetSide.Chicken)
                {
                    cuadat = "Cửa Gà".ToUpper();
                }
                else if (betSide == (int)BetSide.Crab)
                {
                    cuadat = "Cửa Cua".ToUpper();
                }

                else if (betSide == (int)BetSide.Deer)
                {
                    cuadat = "Cửa Hươu".ToUpper();
                }

                else if (betSide == (int)BetSide.Fish)
                {
                    cuadat = "Cửa Cá".ToUpper();
                }

                else if (betSide == (int)BetSide.Gourd)
                {
                    cuadat = "Cửa Bầu".ToUpper();
                }
                else if (betSide == (int)BetSide.Shrimp)
                {
                    cuadat = "Cửa Tôm".ToUpper();
                }

                string msg = $"<strong>🎲[BẦU CUA]</strong>\r\n" +
                           $"<strong>⏳Phiên</strong>: {KeoNguaHandler.Instance.SessionID}\r\n" +
                           $"<strong>🤵🏼‍Tài khoản:</strong> {nickName} ({accountId})\r\n" +
                           $"<strong>🏗Cửa đặt:</strong> {cuadat}\r\n" +
                           $"<strong>💸Tiền cược:</strong> {string.Format("{0:C0}", Convert.ToDecimal(betValue))}\r\n" +
                           $"<strong>💰Tổng cược:</strong> {string.Format("{0:C0}", Convert.ToDecimal(summaryBet))}\r\n";

                SendRequestApi.SendTelePushAll(msg, 15);

                Clients.Caller.betSuccess(result, outBalance);
                return response;
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return (int)ErrorCode.Exception;
            }
        }

        #region persistance
        [HubMethodName("PingPong")]
        public void PingPong()
        {
            try
            {
                long accountId = AccountSession.AccountID;
                string nickname = AccountSession.AccountName;
                if (accountId < 1 || string.IsNullOrEmpty(nickname)) return;
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
            }
        }

        public override Task OnConnected()
        {
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            try
            {
                if (!stopCalled) return base.OnDisconnected(stopCalled);

                long accountId = ConnectionHandler.Instance.PlayerDisconnect(Context.ConnectionId);
                NLogManager.LogMessage(string.Format("{0} mất kết nối {1}", accountId, stopCalled));
                if (accountId < 1) return base.OnDisconnected(stopCalled);

                if (VipHandler.Instance.CheckVip(accountId)) VipHandler.Instance.RegisterLeaveGame(accountId);

                PlayerHandler.Instance.RemovePlayer(accountId);
                KeoNguaHandler.Instance.SummaryPlayer();
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
            }

            return base.OnDisconnected(stopCalled);
        }

        public override Task OnReconnected()
        {
            return base.OnReconnected();
        }
        #endregion persistance
    }
}