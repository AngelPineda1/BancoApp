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

            if (!string.IsNullOrWhiteSpace(token))
            {

                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadToken(token) as JwtSecurityToken;

                var claims = jsonToken?.Claims.Select(x => new Claim(x.Type, x.Value)).ToList();

                if (claims != null)
                {
                    var roleClaim = claims.FirstOrDefault(c => c.Type == "role")?.Value;
                    var nameClaim = claims.FirstOrDefault(c => c.Type == "unique_name")?.Value;
                    var nameid = claims.FirstOrDefault(c => c.Type == "nameid")?.Value;
                    //var email = claims.FirstOrDefault(c => c.Type == "email")?.Value;

                    var claimsLista = new List<Claim>(){
                        new Claim(ClaimTypes.Name, nameClaim),
                        new Claim(ClaimTypes.Role, roleClaim),
                        new Claim(ClaimTypes.NameIdentifier, nameid),
                        //new Claim(ClaimTypes.Email, email)
                    };


                    var claimsIdentity = new ClaimsIdentity(claimsLista, CookieAuthenticationDefaults.AuthenticationScheme);

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity)
                      , new AuthenticationProperties()
                      {
                          IsPersistent = true,
                          AllowRefresh = true
                      });

                    if (!HttpContext.User.Identity.IsAuthenticated)
                    {
                        ModelState.AddModelError("", "Error al crear la cookie de autenticación");
                        return View(dto);
                    }

                    var a = HttpContext.User.Identities.Select(x => x.Claims.Where(y => y.Type == ClaimTypes.Name)).ToList();

                    if (roleClaim == "Admin")
                        return RedirectToAction("Index", "Home", new { area = "Admin" });
                    else if (roleClaim == "Cajero")
                        return RedirectToAction("Index", "Home", new { area = "Cajero" });
                }
            }

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
