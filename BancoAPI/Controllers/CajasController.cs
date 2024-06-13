using BancoAPI.Helpers;
using BancoAPI.Models.Dtos;
using BancoAPI.Models.Entities;
using BancoAPI.Models.Validators;
using BancoAPI.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.Intrinsics.Arm;

namespace BancoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CajasController(CajasRepository cajasRepository, CajasValidator validations) : ControllerBase
    {
        private CajasRepository _cajasRepository = cajasRepository;
        private CajasValidator _cajasValidator = validations;
        [HttpGet]
        public IActionResult Get()
        {
            var datos = _cajasRepository.GetAll();
            return Ok(datos);
        }


        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var datos = _cajasRepository.Get(id);
            return Ok(datos);
        }

        [HttpPost]
        public IActionResult Post(CajasDto dto)
        {
            if (dto != null)
            {
                var results = _cajasValidator.Validate(dto);
                if (results.IsValid)
                {
                    dto.Contrasena = Encrypter.HashPassword(dto.Contrasena);
                    Cajas cajas = new Cajas()
                    {
                        Id = 0,
                        Nombre = dto.Nombre,
                        Username = dto.Username,
                        Contrasena = dto.Contrasena,
                        Activa = true
                    };
                    _cajasRepository.Insert(cajas);
                    return Created();
                }
                return BadRequest(results.Errors.Select(x => x.ErrorMessage));
            }
            return BadRequest();
        }

        [HttpPut]
        public IActionResult Pu(CajasUpDto dto)
        {
            if (dto != null)
            {
                var caja = _cajasRepository.Get(dto.Id);
                if (caja != null)
                {
                    var encrypter = new Encrypter();


                    var results = _cajasValidator.Validate(dto);
                    if (results.IsValid)
                    {
                        if (encrypter.IsPasswordChanged(caja.Contrasena, dto.Contrasena))
                        {
                            dto.Contrasena = Encrypter.HashPassword(dto.Contrasena);
                            
                        }
                        Cajas cajas = new Cajas()
                        {

                            Nombre = dto.Nombre,
                            Username = dto.Username,
                            Contrasena = dto.Contrasena,
                            Activa = dto.Activa
                        };
                        _cajasRepository.Update(cajas);
                        return Ok();
                    }
                    return BadRequest(results.Errors.Select(x => x.ErrorMessage));
                }
                return NotFound();
            }
            return BadRequest();
        }


        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var caja= _cajasRepository.Get(id);
            if (caja != null && caja.Activa!=false)
            {
                Cajas _cajas = new Cajas()
                {
                    Id = caja.Id,
                    Username = caja.Username,
                    Contrasena = caja.Contrasena,
                    Activa = false,
                    Nombre = caja.Nombre,
                };
                _cajasRepository.Update(_cajas);
               return Ok();
            }
            return NotFound();
        }

    }
}
