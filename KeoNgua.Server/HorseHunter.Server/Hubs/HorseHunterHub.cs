using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using HorseHunter.Server.DataAccess.DTO;
using HorseHunter.Server.DataAccess.Factory;
using HorseHunter.Server.Handlers;
using System;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using TraditionGame.Utilities;
using TraditionGame.Utilities.Caching;
using TraditionGame.Utilities.IpAddress;
using TraditionGame.Utilities.Session;

namespace HorseHunter.Server.Hubs
{
    [HubName("HorseHunterHub")]
    public class HorseHunterHub : Hub
    {
        [HubMethodName("EnterLobby")]
        public void EnterLobby()
        {
            try
            {
                Groups.Add(Context.ConnectionId, "AllUserInHorseHunter");
                long accountId = AccountSession.AccountID;
                string nickname = AccountSession.AccountName;
                if (accountId < 1 || string.IsNullOrEmpty(nickname))
                {
                    NLogManager.LogMessage(string.Format("EL-NotAuthen-Acc:{0}", accountId));
                    return;
                }

                //check connection người chơi
                var lstConn = ConnectionHandler.Instance.GetConnections(accountId);
                foreach (var conn in lstConn.Where(conn => !conn.Equals(Context.ConnectionId)))
                {
                    //Clients.Client(conn).otherDevice((int)ErrorCode.OtherDevice, "Tài khoản của bạn đang chơi game ở thiết bị khác.");
                    Clients.Client(conn).otherDevice((int)ErrorCode.OtherDevice, "Your account is playing games on another device.");
                    ConnectionHandler.Instance.PlayerDisconnect(conn);
                }

                ConnectionHandler.Instance.PlayerConnect(accountId, Context.ConnectionId);
                var lstJp = AbstractFactory.Instance().CreateHorseHunterDao().GetJackpot();
                if (lstJp != null && lstJp.Any())
                {
                    var strJp = HorseHunterHandler.Instance.ConvertJackpot(lstJp);
                    Clients.Caller.updateJackpot(strJp);
                }
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
            }
        }

        [HubMethodName("PlayNow")]
        public int PlayNow(int roomId)
        {
            if (roomId < 1 || roomId > 4)
                return (int)ErrorCode.RoomNotExist;

            try
            {
                long accountId = AccountSession.AccountID;
                string nickname = AccountSession.AccountName;
                if (accountId < 1 || string.IsNullOrEmpty(nickname))
                {
                    NLogManager.LogMessage("PN-NotAuthen-Acc:" + accountId + "| IP:" + IPAddressHelper.GetClientIP());
                    return (int)ErrorCode.NotAuthen;
                }
                int res = HorseHunterHandler.Instance.PlayNow(roomId);
                return res;
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
                return (int)ErrorCode.Exception;
            }
        }

        //[Authorize]
        [HubMethodName("Spin")]
        public int Spin(int roomId, string lines, int deviceId)
        {
            if (roomId < 1 || roomId > 4)
                return (int)ErrorCode.RoomNotExist;

            if (deviceId < 1 || deviceId > 4)
                return (int)ErrorCode.DeviceIDNotExist;

            try
            {
                long accountId = AccountSession.AccountID;
                string nickname = AccountSession.AccountName;
                if (accountId < 1 || string.IsNullOrEmpty(nickname))
                {
                    NLogManager.LogMessage("Spin-NotAuthen-Acc:" + accountId + "| IP:" + IPAddressHelper.GetClientIP());
                    return (int)ErrorCode.NotAuthen;
                }

                if (!CheckValidLines(lines))
                {
                    NLogManager.LogMessage(string.Format("LinesErr:{0}", lines));
                    return (int)ErrorCode.LinesInvalid;
                }

                var milliRq = Int32.Parse(ConfigurationManager.AppSettings["MillisecondsRequestSpin"].ToString());
                int cacheCounter = CacheCounter.CheckAccountActionFrequencyMiliSecond(string.Format("{0}_{1}", "minibar", accountId), milliRq, "spin");
                if (cacheCounter > 1)
                {
                    NLogManager.LogMessage(string.Format("BlockAccFast=> {0} ({1}) đặt cược 1 giây {2} lần.", nickname, accountId, cacheCounter));
                    return (int)ErrorCode.Duplicate;
                }

                if (CacheCounter.AccountActionCounter(accountId.ToString(), "SpinAm") > 5)
                {
                    NLogManager.LogMessage(string.Format("BlockAccAm=> {0} ({1}) bắn âm > 5 lần.", nickname, accountId));
                    return (int)ErrorCode.BlockSpin;
                }

                int res = HorseHunterHandler.Instance.Spin(roomId, lines, deviceId);
                if (res < 0)
                    CacheCounter.CheckAccountActionFrequency(accountId.ToString(), 15, "SpinAm");

                return res;
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
                if (accountId < 1 || string.IsNullOrEmpty(nickname))
                    return;
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
                Groups.Remove(Context.ConnectionId, "AllUserInHorseHunter");
                long accountId = ConnectionHandler.Instance.PlayerDisconnect(Context.ConnectionId);
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
            }

            return base.OnDisconnected(true);
        }

        public override Task OnReconnected()
        {
            return base.OnReconnected();
        }
        #endregion persistance


        #region Valid Data
        private bool CheckValidLines(string lines)
        {
            if (string.IsNullOrEmpty(lines)) return false;

            int maxline = Int32.Parse(ConfigurationManager.AppSettings["DefaulTotalLine"]);
            string[] ArrLines = lines.Split(',');
            if (ArrLines.Length < 1 || ArrLines.Length > maxline) return false;

            var groups = ArrLines.GroupBy(v => v);
            foreach (var group in groups)
            {
                if (group.Count() > 1) return false;
            }

            for (int i = 0; i < ArrLines.Length; i++)
            {
                int n = 0;
                if (!int.TryParse(ArrLines[i], out n)) return false;
                if (n < 1 || n > maxline) return false;
            }
            return true;
        }
        #endregion
    }
}