using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Threading.Tasks;
using TraditionGame.Utilities.IpAddress;
using TraditionGame.Utilities.Session;
using MsWebGame.Portal.Handlers;
using TraditionGame.Utilities;

namespace MsWebGame.Portal.Hubs
{
    [HubName("PortalHub")]
    public class PortalHub : Hub
    {
        //[Authorize]
        [HubMethodName("EnterLobby")]
        public void EnterLobby()
        {
            try
            {
                Groups.Add(Context.ConnectionId, "AllUserInPortal");
                long accountId = AccountSession.AccountID;
                string username = AccountSession.AccountName;
                if (accountId < 1 || string.IsNullOrEmpty(username))
                {
                    NLogManager.LogMessage("EnterLobby-NotAuthen AccountId: " + accountId + "|ClientIP: " + IPAddressHelper.GetClientIP());
                    return;
                }

                ConnectionHandler.Instance.PlayerConnect(accountId, Context.ConnectionId);
            }
            catch (Exception ex)
            {
                NLogManager.PublishException(ex);
            }
        }

        #region persistance
        [HubMethodName("PingPong")]
        public void PingPong()
        {
            try
            {
                long accountId = AccountSession.AccountID;
                string userName = AccountSession.AccountName;
                if (accountId < 1 || string.IsNullOrEmpty(userName))
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
                Groups.Remove(Context.ConnectionId, "AllUserInPortal");
                long accountId = ConnectionHandler.Instance.PlayerDisconnect(Context.ConnectionId);
                //NLogManager.LogMessage(string.Format("{0} disconnected", accountId));
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
    }
}