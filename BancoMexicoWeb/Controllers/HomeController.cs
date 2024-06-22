using BancoAPI.Models.Dtos;
using BancoMexicoWeb.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using NuGet.Protocol;

namespace BancoMexicoWeb.Controllers
{
    public class HomeController(TurnoService turnoService) : Controller
    {
        private readonly TurnoService turnoService = turnoService;

        public IActionResult Index()
        {
            return View();

        }



        [HttpGet]

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            if (dto == null)
                return View();

            if (string.IsNullOrWhiteSpace(dto.UserName))
                ModelState.AddModelError("", "El usuario es requerido");
            if (string.IsNullOrWhiteSpace(dto.Password))
                ModelState.AddModelError("", "La contraseña es requerida");


            if (!ModelState.IsValid)
                return View(dto);

            dto.UserName = dto.UserName.ToUpper().Trim();
            var token = await turnoService.Login(dto);

            if (string.IsNullOrWhiteSpace(token))
                return View(dto);

            var jsonToken = new JwtSecurityTokenHandler().ReadToken(token) as JwtSecurityToken;

            var roleClaim = jsonToken?.Claims.FirstOrDefault(c => c.Type == "role")?.Value;
            var nameClaim = jsonToken?.Claims.FirstOrDefault(c => c.Type == "unique_name")?.Value;
            var nameid = jsonToken?.Claims.FirstOrDefault(c => c.Type == "nameid")?.Value;
            var email = jsonToken?.Claims.FirstOrDefault(c => c.Type == "email")?.Value;

            var claimsLista = new List<Claim>(){
                        new (ClaimTypes.Name, nameClaim??""),
                        new (ClaimTypes.Role, roleClaim??""),
                        new (ClaimTypes.NameIdentifier, nameid??""),
                        new (ClaimTypes.Email, email??"")
                    };


            var claimsIdentity = new ClaimsIdentity(claimsLista, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity)
              , new AuthenticationProperties()
              {
                  IsPersistent = true,
                  AllowRefresh = true
              });

            if (roleClaim == "Admin")
                return RedirectToAction("Index", "Home", new { area = "Admin" });
            else if (roleClaim == "Cajero")
                return RedirectToAction("Index", "Home", new { area = "Cajero" });

            return View(dto);

        }



        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        // AccesoDenegado

        public IActionResult AccesoDenegado()
        {
            return View();
        }










    }
}
