using NCAT.lib.Connections.Base;

namespace NCAT.lib.Connections
{
    public class TCPConnections : BaseConnections
    {
        public override string ConnectionType => "TCP";
    }
}