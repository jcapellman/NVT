using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Text.Json;
using System.Threading.Tasks;

using NCAT.lib.JSONObjects;
using NCAT.lib.Objects;

namespace NCAT.lib
{
    public static class TCPConnections
    {
        private static readonly HttpClient HttpClient = new HttpClient();

        private const string UNKNOWN = "<UNKNOWN>";

        private static async Task<(string ISP, string Country)> GetReverseLookupAsync(string ipAddress)
        {
            if (ipAddress != "127.0.0.1")
            {
                try
                {
                    var response =
                        await HttpClient.GetAsync(new Uri($"http://ip-api.com/csv/{ipAddress}?fields=country,isp"));

                    if (response.IsSuccessStatusCode)
                    {
                        var csv = await response.Content.ReadAsStringAsync();

                        csv = csv.Replace("\n", "");

                        if (string.IsNullOrEmpty(csv))
                        {
                            return (UNKNOWN, UNKNOWN);
                        }

                        return (csv.Split(',')[1], csv.Split(',')[0]);
                    }
                }
                catch (HttpRequestException hre)
                {
                    var e = hre;
                }
            }

            return (UNKNOWN, UNKNOWN);
        }

        public static async Task<List<NetworkConnectionItem>> GetConnections()
        {
            var properties = IPGlobalProperties.GetIPGlobalProperties();

            var connections = properties.GetActiveTcpConnections();

            var activeConnections = new List<NetworkConnectionItem>();

            foreach (var connection in connections)
            {
                var item = new NetworkConnectionItem
                {
                    IPAddress = connection.RemoteEndPoint.Address.ToString(),
                    Port = connection.RemoteEndPoint.Port,
                    DetectedTime = DateTime.Now
                };

                (item.ISP, item.Country) = await GetReverseLookupAsync(item.IPAddress);

                activeConnections.Add(item);
            }

            return activeConnections;
        }
    }
}