using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using NVT.lib;
using NVT.lib.JSONObjects;
using NVT.REST.Objects;

namespace NVT.REST.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LookupController : ControllerBase
    {
        private List<IPAPIJsonObject> CheckDB(string[] ipAddresses) => DB.CheckDBForIPs(ipAddresses);

        private async Task<List<IPAPIJsonObject>> CheckAPI(string[] ipAddresses)
        {
            using (var httpClient = new HttpClient())
            {
                var requestObject = ipAddresses.Distinct().Select(a => new LookupRequestItem(a)).ToArray();

                var response = await httpClient.PostAsync(
                    "http://ip-api.com/batch", new StringContent(
                        JsonSerializer.Serialize(requestObject), Encoding.UTF8, "application/json"));

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();

                    var results = JsonSerializer.Deserialize<List<IPAPIJsonObject>>(json);

                    return results.Where(a => a.status != "fail").ToList();
                }

                return new List<IPAPIJsonObject>();
            }
        }

        [HttpPost]
        public IEnumerable<IPAPIJsonObject> POST(string[] ipAddresses)
        {
            var dbMatches = CheckDB(ipAddresses);

            var unknownMatches = ipAddresses.Where(ipAddress => dbMatches.All(a => a.query != ipAddress)).ToArray();

            var apiHits = CheckAPI(unknownMatches).Result;

            DB.AddToDB(apiHits);

            dbMatches.AddRange(apiHits);

            return dbMatches;
        }
    }
}