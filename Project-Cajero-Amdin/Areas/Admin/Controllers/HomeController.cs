using Microsoft.AspNetCore.Mvc;

namespace Project_Cajero_Amdin.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
