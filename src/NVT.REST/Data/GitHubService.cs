using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

using NVT.REST.Objects;

namespace NVT.REST.Data
{
    public class GitHubService
    {
        public async Task<GitHubLatestResponseItem> GetLatestRelease()
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("User-Agent", "NVTWebApp");

                var response =
                    await httpClient.GetAsync("https://api.github.com/repos/jcapellman/NVT/releases/latest");

                return response.IsSuccessStatusCode ? JsonSerializer.Deserialize<GitHubLatestResponseItem>(await response.Content.ReadAsStringAsync()) : null;
            }
        }
    }
}