using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using NVT.lib.Connections.Base;
using NVT.lib.Objects;
using NVT.lib.JSONObjects;

namespace NVT.lib.Managers
{
    public class ConnectionManager
    {
        private readonly List<BaseConnections> _connections = new List<BaseConnections>();

        public ConnectionManager()
        {
            var implementations = Assembly.GetAssembly(typeof(BaseConnections)).GetTypes().Where(a => a.BaseType == typeof(BaseConnections)).ToList();

            foreach (var implementation in implementations)
            {
                _connections.Add((BaseConnections)Activator.CreateInstance(implementation));
            }
        }

        public string[] SupportedConnectionTypes => _connections.Select(a => a.ConnectionType).ToArray();

        public async Task<List<NetworkConnectionItem>> GetConnectionsAsync(SettingsObject settings)
        {
            var networkConnections = new List<NetworkConnectionItem>();

            foreach (var connection in _connections.Where(a => settings.EnabledConnectionTypes.Contains(a.ConnectionType)))
            {
                var connections = await connection.GetConnectionsAsync(settings);

                networkConnections.AddRange(connections);
            }

            return networkConnections;
        }
    }
}