using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using NVT.lib.JSONObjects;
using NVT.lib.Managers;
using NVT.lib.Objects;

using NLog;

namespace NVT.lib.Connections.Base
{
    public abstract class BaseConnections
    {
        protected static readonly Logger Log = LogManager.GetCurrentClassLogger();

        protected static readonly HttpClient HttpClient = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(1)
        };

        public abstract string ConnectionType { get; }

        public async Task<List<NetworkConnectionItem>> GetConnectionsAsync(List<NetworkConnectionItem> connectionQuery)
        {
            var unknownConnections = new List<NetworkConnectionItem>();
            var knownConnections = new List<NetworkConnectionItem>();

            foreach (var connection in connectionQuery)
            {
                try
                {
                    var networkConnectionItem = connection;
                    
                    if (!DB.CheckDB(ref networkConnectionItem) && DIContainer.GetDIService<SettingsManager>().SettingsObject.EnableIPLookup)
                    {
                        unknownConnections.Add(networkConnectionItem);
                    }
                    else
                    {
                        knownConnections.Add(networkConnectionItem);
                    }
                }
                catch (Exception ex)
                {
                    Log.Error($"Error occurred on populating the network items: {ex}");
                }
            }

            var lookupResults = await GetReverseLookupAsync(unknownConnections);

            foreach (var result in lookupResults)
            {
                var item = unknownConnections.FirstOrDefault(a => a.IPAddress == result.query);

                if (item == null)
                {
                    continue;
                }

                item.City = result.city;
                item.Country = result.country;
                item.ISP = result.isp;
                item.Longitude = result.lon;
                item.Latitude = result.lat;
                item.Id = 0;

                DB.AddToDB(item);

                knownConnections.Add(item);
            }

            knownConnections.AddRange(unknownConnections);

            return knownConnections;
        }

        protected static async Task<List<IPAPIJsonObject>> GetReverseLookupAsync(List<NetworkConnectionItem> items)
        {
            try
            {
                var response =
                    await HttpClient.PostAsync(
                        new Uri(
                            DIContainer.GetDIService<SettingsManager>().SettingsObject.IPLookupURL),
                        new StringContent(
                            JsonSerializer.Serialize(items.Select(a => a.IPAddress).ToArray()), Encoding.UTF8, "application/json"));

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();

                    if (string.IsNullOrEmpty(json) || json == "{}")
                    {
                        return new List<IPAPIJsonObject>();
                    }

                    return JsonSerializer.Deserialize<List<IPAPIJsonObject>>(json);
                }

                return new List<IPAPIJsonObject>();
            }
            catch (Exception ex)
            {
                Log.Error($"Error when retrieving the reverse lookup: {ex}");

                return new List<IPAPIJsonObject>();
            }
        }
    }
}