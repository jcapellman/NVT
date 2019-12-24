using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using NCAT.lib.JSONObjects;
using NCAT.lib.Objects;

using NLog;

namespace NCAT.lib.Connections.Base
{
    public abstract class BaseConnections
    {
        protected static readonly Logger Log = LogManager.GetCurrentClassLogger();

        protected static readonly HttpClient HttpClient = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(1)
        };

        public const string UNKNOWN = "<UNKNOWN>";

        protected string[] EMPTY_HOST = { "0.0.0.0:0", "[::]:0" };

        protected const string LOCALHOST = "127.0.0.1";

        public abstract string ConnectionType { get; }

        public async Task<List<NetworkConnectionItem>> GetConnectionsAsync()
        {
            var processes = Process.GetProcesses();

            var activeConnections = new List<NetworkConnectionItem>();

            var pStartInfo = new ProcessStartInfo();

            pStartInfo.FileName = "netstat.exe";
            pStartInfo.Arguments = "-a -n -o";
            pStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            pStartInfo.UseShellExecute = false;
            pStartInfo.RedirectStandardInput = true;
            pStartInfo.RedirectStandardOutput = true;
            pStartInfo.RedirectStandardError = true;
            pStartInfo.CreateNoWindow = true;

            var process = new Process()
            {
                StartInfo = pStartInfo
            };

            process.Start();

            var soStream = process.StandardOutput;

            var output = soStream.ReadToEnd();

            var lines = Regex.Split(output, "\r\n");

            foreach (var line in lines)
            {
                string[] parts = null;

                try
                {
                    if (!line.Trim().StartsWith(ConnectionType))
                    {
                        continue;
                    }

                    parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                    var len = parts.Length;

                    if (len <= 2 || EMPTY_HOST.Contains(parts[2]) || parts[2].Contains(LOCALHOST))
                    {
                        continue;
                    }

                    var ipAddress = parts[2].Split(':')[0];
                    var port = Convert.ToInt32(parts[2].Split(':')[1]);

                    var pid = int.Parse(parts[len - 1]);

                    var processName = UNKNOWN;
                    var processFileName = UNKNOWN;

                    var matchedProcess = processes.FirstOrDefault(a => a.Id == pid);

                    if (matchedProcess != null)
                    {
                        processName = matchedProcess.ProcessName;

                        try
                        {
                            processFileName = matchedProcess.MainModule?.FileName;
                        }
                        catch
                        {
                        }
                    }

                    var item = DB.CheckDB(ipAddress);

                    if (item == null)
                    {
                        item = new NetworkConnectionItem
                        {
                            IPAddress = ipAddress,
                            Port = port,
                            DetectedTime = DateTime.Now,
                            ProcessName = processName,
                            ProcessFileName = processFileName,
                            ISP = UNKNOWN,
                            Country = UNKNOWN,
                            City = UNKNOWN
                        };

                        item = await GetReverseLookupAsync(item);
                    }
                    else
                    {
                        item.DetectedTime = DateTime.Now;
                        item.ProcessName = processName;
                        item.ProcessFileName = processFileName;
                    }

                    activeConnections.Add(item);
                }
                catch (Exception ex)
                {
                    Log.Error($"Error occurred on populating the network items: {ex}");
                }
            }

            return activeConnections;
        }

        protected static async Task<NetworkConnectionItem> GetReverseLookupAsync(NetworkConnectionItem item)
        {
            if (item.IPAddress != LOCALHOST)
            {
                try
                {
                    var response =
                        await HttpClient.GetAsync(new Uri($"http://ip-api.com/json/{item.IPAddress}?fields=country,city,lat,lon,isp"));

                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();

                        if (string.IsNullOrEmpty(json))
                        {
                            item.ISP = UNKNOWN;
                            item.Country = UNKNOWN;

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