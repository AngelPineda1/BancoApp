using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BancoMexicoWeb.Areas.Cajero.Controllers
{
    [Authorize(Roles = "Cajero")]
    [Area("Cajero")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
