using BancoAPI.Helpers;
using BancoAPI.Models.Dtos;
using BancoAPI.Models.Entities;
using BancoAPI.Models.Validators;
using BancoAPI.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BancoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController(UsuariosRepository usuariosRepository, UsuariosValidator validations) : ControllerBase
    {
        public UsuariosRepository UsuariosRepository { get; } = usuariosRepository;
        public UsuariosValidator Validations { get; } = validations;

        [HttpGet]
        public IActionResult Get()
        {
            var datos= UsuariosRepository.GetAll();
            return Ok(datos);
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var usuario= UsuariosRepository.Get(id);
            if(usuario == null)
            {
                return NotFound();
            }
            return Ok(usuario);
        }
        [HttpPost] 
        public IActionResult Post(UsuarioDto dto)
        {
            if (dto != null)
            {
                var results = Validations.Validate(dto);
                if (results.IsValid)
                {
                    dto.Contrasena=Encrypter.HashPassword(dto.Contrasena);
                    Usuarios  usuarios = new Usuarios()
                    {
                        Id = 0,
                        Nombre = dto.Nombre,
                        Username = dto.Username,
                        Contrasena = dto.Contrasena,
                       
                    };
                    UsuariosRepository.Insert(usuarios);
                    return Created();
                }
                return BadRequest(results.Errors.Select(x => x.ErrorMessage));
            }
            return BadRequest();
        }

        [HttpPut]
        public IActionResult Put(UsuarioUpDto dto)
        {
            if(dto != null)
            {
                var user=UsuariosRepository.Get(dto.Id);
                if(user == null)
                {
                    return NotFound();
                }
                var encrypter = new Encrypter();
                var results=Validations.Validate(dto);
                if (results.IsValid) {
                    if (encrypter.IsPasswordChanged(user.Contrasena, dto.Contrasena))
                    {
                        dto.Contrasena = Encrypter.HashPassword(dto.Contrasena);
                        user.Contrasena=dto.Contrasena;

                    }

                   user.Nombre = dto.Nombre;
                    user.Username = dto.Username;
                    UsuariosRepository.Update(user);
                    return Ok();
                }
                else
                {
                    return BadRequest(results.Errors.Select(x => x.ErrorMessage));
                }
            }
            return BadRequest();
        }


        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var user = UsuariosRepository.Get(id);
            if(user == null)
            {
                return NotFound();
            }
            UsuariosRepository.Delete(user);
            return Ok();
        }
    }
}
