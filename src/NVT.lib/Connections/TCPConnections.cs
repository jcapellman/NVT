using NVT.lib.Connections.Base;

namespace NVT.lib.Connections
{
    public class TCPConnections : BaseConnections
    {
        public override string ConnectionType => "TCP";
    }
}