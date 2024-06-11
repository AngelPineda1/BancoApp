using BancoAPI.Helpers;
using BancoAPI.Models.Dtos;
using BancoAPI.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices;

namespace BancoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController(UsuariosRepository usuariosRepository, CajasRepository cajasRepository, IConfiguration configuration) : ControllerBase
    {
        readonly IConfiguration Configuration = configuration;
        readonly UsuariosRepository UsuariosRepository = usuariosRepository;
        readonly CajasRepository CajasRepository = cajasRepository;
        [HttpPost]
        public IActionResult Login(LoginDto login)
        {
            var user = UsuariosRepository.Get(login.UserName);
            var cajas = CajasRepository.Get(login.UserName);
            if (user != null)
            {
                var succes = Verifier.VerifyPassword(login.Password, user.Contrasena);
                if (!succes)
                {
                    return BadRequest("Credenciales Incorrectas");
                }
                string role = "Admin";
                var jwttoken = new JwtTokenGenerator(configuration);
                var token = jwttoken.GetTokenUser(user, role);
                return Ok(token);


            }
            else if (cajas != null)
            {
                var succes = Verifier.VerifyPassword(login.Password, cajas.Contrasena);
                if (!succes)
                {
                    return BadRequest("Credenciales Incorrectas");
                }
                string role = "Cajero";
                var jwttoken = new JwtTokenGenerator(configuration);
                var token = jwttoken.GetTokenCajas(cajas, role);
                return Ok(token);
            }
            else { return BadRequest(); }
        }
    }
}
