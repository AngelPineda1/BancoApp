using BancoMexicoWeb.Areas.Admin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json;

namespace BancoMexicoWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class HomeController : Controller
    {
        readonly HttpClient _httpClient = new();

        public async Task<IActionResult> IndexAsync()
        {
            _httpClient.BaseAddress = new Uri("https://bancomexicoapi.websitos256.com/");

            var turnosinfo = new TurnosEstatsViewModel();
            var response = await _httpClient.GetAsync("/api/Turno");

            if(response.IsSuccessStatusCode) {
                var content = await response.Content.ReadAsStringAsync();
                var turnos= JsonSerializer.Deserialize<TurnosEstatsViewModel>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                turnosinfo=turnos;
                return View(turnosinfo);
            }
            return View();
        }
    }
}
