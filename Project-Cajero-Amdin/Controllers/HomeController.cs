using Microsoft.AspNetCore.Mvc;

namespace Project_Cajero_Amdin.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
