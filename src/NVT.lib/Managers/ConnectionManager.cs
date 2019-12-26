using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using NVT.lib.Connections.Base;
using NVT.lib.Objects;

namespace NVT.lib.Managers
{
    public class ConnectionManager
    {
        private List<BaseConnections> _connections = new List<BaseConnections>();

        public ConnectionManager()
        {
            var implementations = Assembly.GetAssembly(typeof(BaseConnections)).GetTypes().Where(a => a.BaseType == typeof(BaseConnections)).ToList();

            foreach (var implementation in implementations)
            {
                _connections.Add((BaseConnections)Activator.CreateInstance(implementation));
            }
        }

        public string[] SupportedConnectionTypes => _connections.Select(a => a.ConnectionType).ToArray();

        public async Task<List<NetworkConnectionItem>> GetConnectionsAsync(string[] supportedTypes)
        {
            var networkConnections = new List<NetworkConnectionItem>();

            foreach (var connection in _connections.Where(a => supportedTypes.Contains(a.ConnectionType)))
            {
                var connections = await connection.GetConnectionsAsync();

                networkConnections.AddRange(connections);
            }

            return networkConnections;
        }
    }
}