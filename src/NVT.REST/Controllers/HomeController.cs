using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using NVT.REST.Objects;

namespace NVT.REST.Controllers
{
    public class HomeController : Controller
    {
        private async Task<GitHubLatestResponseItem> GetLatestRelease()
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("User-Agent", "NVTWebApp");
                
                var response =
                    await httpClient.GetAsync("https://api.github.com/repos/jcapellman/NVT/releases/latest");

                if (response.IsSuccessStatusCode)
                {
                    return JsonSerializer.Deserialize<GitHubLatestResponseItem>(await response.Content.ReadAsStringAsync());
                }

                return null;
            }
        }

        public async Task<IActionResult> Index()
        {
            var gitHubResponseItem = await GetLatestRelease();

            return View(gitHubResponseItem);
        }
    }
}