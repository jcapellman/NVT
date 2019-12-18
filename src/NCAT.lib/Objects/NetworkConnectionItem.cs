using System;

namespace NCAT.lib.Objects
{
    public class NetworkConnectionItem
    {
        public string IPAddress { get; set; }

        public int Port { get; set; }

        public string Country { get; set; }

        public string ISP { get; set; }

        public DateTime DetectedTime { get; set; }
    }
}