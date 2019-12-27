using System;

namespace NVT.lib.Objects
{
    public class NetworkConnectionItem
    {
        public int Id { get; set; }

        public string ConnectionType { get; set; }

        public string IPAddress { get; set; }

        public int Port { get; set; }

        public string Country { get; set; }

        public string City { get; set; }

        public string ISP { get; set; }

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }

        public DateTime DetectedTime { get; set; }

        public string ProcessName { get; set; }

        public int ProcessId { get; set; }

        public string ProcessFileName { get; set; }
    }
}