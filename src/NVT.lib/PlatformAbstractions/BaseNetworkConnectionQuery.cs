using System.Collections.Generic;

using NVT.lib.Objects;

namespace NVT.lib.PlatformAbstractions
{
    public abstract class BaseNetworkConnectionQuery
    {
        protected string[] EMPTY_HOST = { "0.0.0.0:0", "[::]:0" };

        public abstract List<NetworkConnectionItem> GetActiveConnections();
    }
}