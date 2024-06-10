using Microsoft.AspNetCore.Mvc;

namespace BancoMexicoWeb.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
