using Microsoft.AspNetCore.Mvc;

namespace NVT.REST.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index() => View();
    }
}