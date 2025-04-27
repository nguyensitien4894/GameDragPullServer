using System;
using Microsoft.AspNet.SignalR;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using KeoNgua.Server.Hubs;

namespace KeoNgua.Server.Handlers
{
    public class ConnectionHandler
    {
        private static readonly Lazy<ConnectionHandler> _instance = new Lazy<ConnectionHandler>(
            () => new ConnectionHandler(GlobalHost.ConnectionManager.GetHubContext<KeoNguaHub>()));

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

        #region Player Connection
        public string PlayerConnect(long accountId, string connection)
        {
            if (accountId < 1 || string.IsNullOrEmpty(connection)) return string.Empty;

            _mapHubToAccount.TryAdd(connection, accountId);

            if (!_mapAccountToHub.ContainsKey(accountId))
            {
                List<string> lstConn = new List<string> { connection };
                _mapAccountToHub.TryAdd(accountId, lstConn);
            }
            else
            {
                List<string> lstConn = null;
                if (!_mapAccountToHub.TryGetValue(accountId, out lstConn)) return string.Empty;

                if (!Monitor.TryEnter(lstConn, 2000)) return string.Empty;

                if (lstConn.Count < 1) return string.Empty;

                try
                {
                    string first = lstConn.FirstOrDefault();
                    lstConn.Clear();
                    lstConn.Add(connection);
                    return first;
                }
                finally
                {
                    Monitor.Exit(lstConn);
                }
            }
            return string.Empty;
        }

        public long PlayerDisconnect(string connection)
        {
            if (string.IsNullOrEmpty(connection)) return -1;

            long accountId = 0;
            _mapHubToAccount.TryRemove(connection, out accountId);

            if (!_mapHubToAccount.TryGetValue(connection, out accountId)) return accountId;

            List<string> lstConn = null;
            _mapAccountToHub.TryGetValue(accountId, out lstConn);
            if (lstConn == null) return accountId;

            if (!Monitor.TryEnter(lstConn, 2000)) return accountId;

            try
            {
                if (lstConn.Contains(connection)) lstConn.Remove(connection);

                if (lstConn.Count == 0) _mapAccountToHub.TryRemove(accountId, out lstConn);
            }
            finally
            {
                Monitor.Exit(lstConn);
            }
            return accountId;
        }

        public IReadOnlyList<string> GetConnections(long accId)
        {
            IReadOnlyList<string> lstReturn = new List<string>().AsReadOnly();
            if (accId < 1 || !_mapAccountToHub.ContainsKey(accId))
            {
                return lstReturn;
            }

            List<string> lstConn = null;
            if (!_mapAccountToHub.TryGetValue(accId, out lstConn)) return lstReturn;
            return lstConn != null ? lstConn.AsReadOnly() : lstReturn;
        }
        #endregion


        public void PlayerLeave(long accountId, string msg)
        {
            IReadOnlyList<string> lstConn = ConnectionHandler.Instance.GetConnections(accountId);
            foreach (string conn in lstConn)
            {
                HubContext.Clients.Client(conn).playerLeave(accountId, msg);
            }
        }
    }
}