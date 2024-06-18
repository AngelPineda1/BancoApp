using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BancoMexicoWeb.Areas.Admin.Controllers
{
    [Authorize(Roles ="Admin")]
    [Area("Admin")]
    public class CajerosController : Controller
    {

        readonly HttpClient _httpClient=new();
        public IActionResult Index()
        {
            _httpClient.BaseAddress=new Uri("https://bancomexicoapi.websitos256.com/");

            return View();
        }
    }
}
