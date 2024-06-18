using BancoMexicoWeb.Areas.Admin.Models;
using BancoMexicoWeb.Areas.Admin.Models.Validators;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;

namespace BancoMexicoWeb.Areas.Admin.Controllers
{
    [Authorize(Roles ="Admin")]
    [Area("Admin")]
    public class CajerosController : Controller
    {

        public CajerosController(CajasValidator validationRules)
        {
            ValidationRules = validationRules;
        }

        readonly HttpClient _httpClient=new();

        public CajasValidator ValidationRules { get; }

        public async Task<IActionResult> Index()
        {
            _httpClient.BaseAddress=new Uri("https://bancomexicoapi.websitos256.com/");
            CajasViewModel model = new CajasViewModel();
            var response = await _httpClient.GetAsync("/api/Cajas");

            if (response.IsSuccessStatusCode)
            {
                var content= await response.Content.ReadAsStringAsync();
                var cajas = JsonSerializer.Deserialize<IEnumerable<Cajas>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if(cajas != null)
                {
                    model.Cajas = cajas;
                    return View(model);
                }
            }

            return View();
        }
        [HttpGet]
        public async Task<IActionResult> Agregar()
        {
            AgregarCajaViewModel cajaViewModel = new AgregarCajaViewModel();
            return View(cajaViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Agregar(AgregarCajaViewModel viewModel)
        {
            _httpClient.BaseAddress = new Uri("https://bancomexicoapi.websitos256.com/");
            if (viewModel != null)
            {
                var results =ValidationRules.Validate(viewModel);
                if (results.IsValid)
                {
                    
                    Cajas cajas = new Cajas()
                    {
                       
                        Nombre = viewModel.Nombre,
                        Username = viewModel.Username,
                        Contrasena = viewModel.Contrasena,
                        Estado=1
                    };
                    var json=System.Text.Json.JsonSerializer.Serialize(cajas);
                    var content=new StringContent(json,Encoding.UTF8,"application/json");
                    var response = await
                        _httpClient.PostAsync("/api/Caja", content);
                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Index");
                    }
                    return View(viewModel);
                    
                    
                }
                return View(results.Errors.Select(x => x.ErrorMessage));
            }
            return View();
        }
    }
}
