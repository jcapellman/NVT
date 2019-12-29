using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using NVT.lib.Connections.Base;
using NVT.lib.Objects;
using NVT.lib.PlatformAbstractions;

namespace NVT.lib.Managers
{
    public class ConnectionManager
    {
        private readonly List<BaseConnections> _connections = new List<BaseConnections>();

        private readonly BaseNetworkConnectionQuery _networkConnectionQuery;

        public ConnectionManager(BaseNetworkConnectionQuery networkConnectionQuery)
        {
            _networkConnectionQuery = networkConnectionQuery;

            var implementations = Assembly.GetAssembly(typeof(BaseConnections)).GetTypes().Where(a => a.BaseType == typeof(BaseConnections)).ToList();

            foreach (var implementation in implementations)
            {
                _connections.Add((BaseConnections)Activator.CreateInstance(implementation));
            }
        }

        public string[] SupportedConnectionTypes => _connections.Select(a => a.ConnectionType).ToArray();

        public async Task<List<NetworkConnectionItem>> GetConnectionsAsync()
        {
            var networkConnections = new List<NetworkConnectionItem>();

            var activeConnections = _networkConnectionQuery.GetActiveConnections();

            foreach (var connection in _connections.Where(a => DIContainer.GetDIService<SettingsManager>().SettingsObject.EnabledConnectionTypes.Contains(a.ConnectionType)))
            {
                var connections = await connection.GetConnectionsAsync(activeConnections.Where(a => a.ConnectionType == connection.ConnectionType).ToList());

                networkConnections.AddRange(connections);
            }

            return networkConnections;
        }
    }
}