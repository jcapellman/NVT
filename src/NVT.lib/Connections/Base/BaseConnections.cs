using System;
using System.Collections.Generic;
using System.Net.Http;
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
            for (var x = 0; x < connectionQuery.Count; x++) {
                try
                {
                    var item = connectionQuery[x];

                    if (!DB.CheckDB(ref item) && DIContainer.GetDIService<SettingsManager>().SettingsObject.EnableIPLookup)
                    {
                        item = await GetReverseLookupAsync(item);
                    }

                    connectionQuery[x] = item;
                }
                catch (Exception ex)
                {
                    Log.Error($"Error occurred on populating the network items: {ex}");
                }
            }

            return connectionQuery;
        }

        protected static async Task<NetworkConnectionItem> GetReverseLookupAsync(NetworkConnectionItem item)
        {
            if (item.IPAddress != Common.Constants.LOCALHOST)
            {
                try
                {
                    var response =
                        await HttpClient.GetAsync(new Uri($"http://ip-api.com/json/{item.IPAddress}?fields=country,city,lat,lon,isp"));

                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();

                        if (string.IsNullOrEmpty(json) || json == "{}")
                        {
                            return item;
                        }

                        var ipObject = JsonSerializer.Deserialize<IPAPIJsonObject>(json);

                        item.Latitude = ipObject.lat;
                        item.Longitude = ipObject.lon;
                        item.Country = ipObject.country;
                        item.ISP = ipObject.isp;
                        item.City = ipObject.city;

                        DB.AddToDB(item);

                        return item;
                    }
                }
                catch (Exception ex)
                {
                    Log.Error($"Error when retrieving the reverse lookup: {ex}");
                }
            }

            return item;
        }
    }
}