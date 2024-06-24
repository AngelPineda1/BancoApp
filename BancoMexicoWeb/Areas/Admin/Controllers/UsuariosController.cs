using BancoMexicoWeb.Areas.Admin.Models;
using BancoMexicoWeb.Areas.Admin.Models.Validators;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace BancoMexicoWeb.Areas.Admin.Controllers
{
    [Authorize(Roles ="Admin")]
    [Area("Admin")]
    public class UsuariosController : Controller
    {
        public UsuariosController(UsuariosValidator validationRules)
        {
            ValidationRules = validationRules;
        }

        HttpClient _httpClient = new();

        public UsuariosValidator ValidationRules { get; }

        public async Task<IActionResult> IndexAsync()
        {
            _httpClient.BaseAddress = new Uri("https://bancomexicoapi.websitos256.com/");
            UsuariosViewModel model = new();
            var response = await _httpClient.GetAsync("/api/Usuarios");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var usuarios = JsonSerializer.Deserialize<IEnumerable<Usuarios>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (usuarios != null)
                {
                    model.Usuarios = usuarios;
                    return View(model);
                }
            }

            return View();
        }


        public IActionResult Agregar()
        {
            AgregarUsuarioViewModel vm = new();
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> AgregarAsync(AgregarUsuarioViewModel model)
        {
            _httpClient.BaseAddress = new Uri("https://bancomexicoapi.websitos256.com/");
            if (model != null)
            {
                var results = ValidationRules.Validate(model);
                if (results.IsValid)
                {
                    Usuarios usuarios = new()
                    {
                        Nombre = model.Nombre,
                        Username = model.Username,
                        Contrasena = model.Contrasena,
                    };
                   
                    var json = System.Text.Json.JsonSerializer.Serialize(usuarios);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    var response = await
                        _httpClient.PostAsync("/api/Usuarios", content);
                   
                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Index");
                    }
                    return View(model);


                }
                foreach (var item in results.Errors)
                {

                    ModelState.AddModelError("", item.ErrorMessage);
                }
                return View(model);
            }
            return View();
        }



        [HttpGet("admin/usuarios/editar/{id}")]
        public async Task<IActionResult> Editar(int id)
        {

            var viewmodel = new ActualizarUsuarioViewModel();
            _httpClient.BaseAddress = new Uri("https://bancomexicoapi.websitos256.com/");
            var response = await _httpClient.GetAsync($"api/Usuarios/{id} ");
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
        public async Task<IActionResult> Editar(ActualizarUsuarioViewModel vm)
        {
            _httpClient.BaseAddress = new Uri("https://bancomexicoapi.websitos256.com/");
            if (vm != null)
            {
                var results = ValidationRules.Validate(vm);
                if (results.IsValid)
                {
                    
                    //var response = await _httpClient.GetAsync($"api/Cajas/{vm.Id} ");
                    //if (response.IsSuccessStatusCode)
                    //{
                    //    var content = await response.Content.ReadAsStringAsync();
                    //    var caja = JsonSerializer.Deserialize<Cajas>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    //    if (caja == null)
                    //    {
                    //        return View();

                    //    }

                    //    return View(vm);
                    //}

                    Usuarios usuarios = new Usuarios()
                    {
                        Id = vm.Id,
                        Username = vm.Username,
                        Nombre = vm.Nombre,
                    };
                   

                    var json = System.Text.Json.JsonSerializer.Serialize(usuarios);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    var response2 = await
                        _httpClient.PutAsync("/api/Usuarios", content);
                    if (response2.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Index");
                    }
                    return View(vm);

                }

                var response = await _httpClient.GetAsync($"api/Usuarios/{vm.Id} ");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var caja = JsonSerializer.Deserialize<Usuarios>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
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



        [HttpGet("admin/usuarios/eliminar/{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            _httpClient.BaseAddress = new Uri("https://bancomexicoapi.websitos256.com/");
            var response = await _httpClient.DeleteAsync($"api/Usuarios/{id} ");
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
