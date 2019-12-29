using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

using NVT.lib.Managers;
using NVT.lib.Objects;
using NVT.lib.PlatformAbstractions;

namespace NVT.lib.Windows.PlatformImplementations
{
    public class WindowsNetworkConnectionQuery : BaseNetworkConnectionQuery
    {
        public override List<NetworkConnectionItem> GetActiveConnections()
        {
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

            var lines = Regex.Split(output, "\r\n").Skip(4);

            var processes = ProcessManager.GetCurrentProcesses();

            foreach (var line in lines)
            {
                try
                {
                    var parts = line.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);

                    var len = parts.Length;

                    if (len <= 2 || EMPTY_HOST.Contains(parts[2]) || parts[2].Contains(lib.Common.Constants.LOCALHOST) || parts[1].Count(a => a == ':') > 1)
                    {
                        continue;
                    }

                    var networkItem = new NetworkConnectionItem
                    {
                        IPAddress = parts[1].Split(':')[0],
                        Port = Convert.ToInt32(parts[1].Split(':')[1]),
                        ProcessId = int.Parse(parts[len - 1]),
                        ConnectionType =  parts[0]
                    };

                    if (processes.ContainsKey(networkItem.ProcessId))
                    {
                        var processItem = processes[networkItem.ProcessId];

                        networkItem.ProcessName = processItem.Name;

                        try
                        {
                            networkItem.ProcessFileName = processItem.FileName;
                        }
                        catch (Exception ex)
                        {
                            // TODO
                        }
                    }

                    activeConnections.Add(networkItem);
                }
                catch (Exception ex)
                {
                    //
                }
            }

            return activeConnections;
        }
    }
}