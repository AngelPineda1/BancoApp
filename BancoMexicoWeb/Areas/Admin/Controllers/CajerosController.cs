using BancoMexicoWeb.Areas.Admin.Models;
using BancoMexicoWeb.Areas.Admin.Models.Validators;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;

namespace BancoMexicoWeb.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class CajerosController : Controller
    {

        public CajerosController(CajasValidator validationRules)
        {
            ValidationRules = validationRules;
        }

        readonly HttpClient _httpClient = new();

        public CajasValidator ValidationRules { get; }

        public async Task<IActionResult> Index()
        {
            _httpClient.BaseAddress = new Uri("https://bancomexicoapi.websitos256.com/");
            CajasViewModel model = new CajasViewModel();
            var response = await _httpClient.GetAsync("/api/Cajas");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var cajas = JsonSerializer.Deserialize<IEnumerable<Cajas>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (cajas != null)
                {
                    cajas.ToList();
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
        [Route("Agregar")]
        public async Task<IActionResult> Agregar(AgregarCajaViewModel viewModel)
        {
            _httpClient.BaseAddress = new Uri("https://bancomexicoapi.websitos256.com/");
            if (viewModel != null)
            {
                var results = ValidationRules.Validate(viewModel);
                if (results.IsValid)
                {

                    Cajas cajas = new Cajas()
                    {

                        Nombre = viewModel.Nombre,
                        Username = viewModel.Username,
                        Contrasena = viewModel.Contrasena,
                        Estado = 1
                    };
                    var json = System.Text.Json.JsonSerializer.Serialize(cajas);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    var response = await
                        _httpClient.PostAsync("/api/Cajas", content);

                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Index", "Home", new { area = "Admin" });
                    }
                    return View(viewModel);


                }
                foreach (var item in results.Errors)
                {

                    ModelState.AddModelError("", item.ErrorMessage);
                }
                return View(viewModel);
            }
            return View();
        }


        [HttpGet]
        [Route("/admin/cajeros/editar/{id}")]
        public async Task<IActionResult> Editar(int id)
        {
            var viewmodel = new ActualizarCajaViewModel();
            _httpClient.BaseAddress = new Uri("https://bancomexicoapi.websitos256.com/");
            var response = await _httpClient.GetAsync($"api/Cajas/{id} ");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var caja = JsonSerializer.Deserialize<Cajas>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (caja == null)
                {
                    return View();

                }
                viewmodel.Id = id;
                viewmodel.Username = caja.Username;
                viewmodel.Nombre = caja.Nombre;
                return View(viewmodel);
            }
            return View();

        }


        [HttpPost]
        public async Task<IActionResult> Editar(ActualizarCajaViewModel vm)
        {
            _httpClient.BaseAddress = new Uri("https://bancomexicoapi.websitos256.com/");
            if (vm != null)
            {
                var results = ValidationRules.Validate(vm);
                if (results.IsValid)
                {
                    _httpClient.BaseAddress = new Uri("https://bancomexicoapi.websitos256.com/");

                    Cajas cajas = new Cajas()
                    {
                        Id = vm.Id,
                        Nombre = vm.Nombre,
                        Username = vm.Username,

                    };

                    var json = System.Text.Json.JsonSerializer.Serialize(cajas);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    var response2 = await
                        _httpClient.PutAsync("/api/Cajas", content);
                    if (response2.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Index");
                    }
                    return View(vm);

                }

                var response = await _httpClient.GetAsync($"api/Cajas/{vm.Id} ");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var caja = JsonSerializer.Deserialize<Cajas>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    if (caja == null)
                    {
                        return View();

                    }


                    foreach (var item in results.Errors)
                    {

                        ModelState.AddModelError("", item.ErrorMessage);
                    }
                    return View(vm);
                }
            }
            return View();
        }



        [HttpGet("admin/cajeros/eliminar/{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            _httpClient.BaseAddress = new Uri("https://bancomexicoapi.websitos256.com/");
            var response = await _httpClient.DeleteAsync($"api/Cajas/{id} ");
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Error al eliminar la actividad");
                return View(); // Muestra la vista actual con el mensaje de error
            }

        }

    }
}
