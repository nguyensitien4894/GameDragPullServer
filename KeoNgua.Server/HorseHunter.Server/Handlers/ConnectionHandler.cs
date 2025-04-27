using System;
using Microsoft.AspNet.SignalR;
using HorseHunter.Server.Hubs;
using System.Collections.Concurrent;
using System.Collections.Generic;
using TraditionGame.Utilities;
using System.Threading;
using System.Linq;

namespace HorseHunter.Server.Handlers
{
    public class ConnectionHandler
    {
        private readonly static Lazy<ConnectionHandler> _instance = new Lazy<ConnectionHandler>(
            () => new ConnectionHandler(GlobalHost.ConnectionManager.GetHubContext<HorseHunterHub>()));

        private readonly ConcurrentDictionary<string, long> _mapHubToAccount = new ConcurrentDictionary<string, long>();
        private readonly ConcurrentDictionary<long, List<string>> _mapAccountToHub = new ConcurrentDictionary<long, List<string>>();

        public IHubContext HubContext { get; private set; }

        private ConnectionHandler(IHubContext hubContext)
        {
            HubContext = hubContext;
        }

        public static ConnectionHandler Instance
        {
            get { return _instance.Value; }
        }

        public void AddGroup(string connectionId, string groupName)
        {
            HubContext.Groups.Add(connectionId, groupName);
        }

        public void RemoveGroup(string connectionId, string groupName)
        {
            HubContext.Groups.Remove(connectionId, groupName);
        }

        public void RemoveGroup(string connectionId, ICollection<int> groups)
        {
            try
            {
                foreach (var group in groups)
                {
                    if (!string.IsNullOrEmpty(group.ToString()))
                    {
                        HubContext.Groups.Remove(connectionId, group.ToString());
                    }
                }
            }
            catch (Exception exception)
            {
                NLogManager.PublishException(exception);
            }
        }

        public string GetGroupName(int roomId = 0, string game = "")
        {
            object[] objArray = { roomId, "_", game };
            return string.Concat(objArray);
        }


        #region Player Connection
        public void PlayerConnect(long accountId, string connection)
        {
            if (accountId < 1 || string.IsNullOrEmpty(connection))
                return;

            _mapHubToAccount.TryAdd(connection, accountId);

            if (!_mapAccountToHub.ContainsKey(accountId))
            {
                List<string> lstConn = new List<string> { connection };
                _mapAccountToHub.TryAdd(accountId, lstConn);
            }
            else
            {
                List<string> lstConn = null;
                if (!_mapAccountToHub.TryGetValue(accountId, out lstConn))
                    return;

                if (!Monitor.TryEnter(lstConn, 2000))
                    return;

                if (lstConn.Count < 1)
                    return;

                try
                {
                    string firsr = lstConn.FirstOrDefault();
                    lstConn.Clear();
                    lstConn.Add(connection);
                }
                finally
                {
                    Monitor.Exit(lstConn);
                }
            }
            return;
        }

        public long PlayerDisconnect(string connection)
        {
            if (string.IsNullOrEmpty(connection))
                return -1;

            long accountId = 0;
            _mapHubToAccount.TryRemove(connection, out accountId);

            if (!_mapHubToAccount.TryGetValue(connection, out accountId))
                return accountId;

            List<string> lstConn = null;
            _mapAccountToHub.TryGetValue(accountId, out lstConn);
            if (lstConn == null)
                return accountId;

            if (!Monitor.TryEnter(lstConn, 2000))
                return accountId;

            try
            {
                if (lstConn.Contains(connection))
                    lstConn.Remove(connection);

                if (lstConn.Count == 0)
                    _mapAccountToHub.TryRemove(accountId, out lstConn);
            }
            finally
            {
                Monitor.Exit(lstConn);
            }
            return accountId;
        }

        public IReadOnlyList<string> GetConnections(long accountId)
        {
            IReadOnlyList<string> lstReturn = new List<string>().AsReadOnly();
            if (accountId < 1 || !_mapAccountToHub.ContainsKey(accountId))
                return lstReturn;

            List<string> lstConn = null;
            if (!_mapAccountToHub.TryGetValue(accountId, out lstConn)) return lstReturn;
            return lstConn != null ? lstConn.AsReadOnly() : lstReturn;
        }
        #endregion

        #region Gun message to client
        public void UpdateJackpot(string groupName, string lstJp)
        {
            HubContext.Clients.Group(groupName).updateJackpot(lstJp);
        }

        public void SendMessageToClient(string connectionId, string message)
        {
            HubContext.Clients.Client(connectionId).message(message);
        }

        public void SendMessageToClient(string connectionId, object message)
        {
            HubContext.Clients.Client(connectionId).message(message);
        }
        #endregion Gun message to client
    }
}