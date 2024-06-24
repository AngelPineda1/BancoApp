using BancoAPI.Helpers;
using BancoAPI.Hubs;
using BancoAPI.Models.Dtos;
using BancoAPI.Models.Entities;
using BancoAPI.Models.Enum;
using BancoAPI.Models.Validators;
using BancoAPI.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Runtime.Intrinsics.Arm;

namespace BancoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CajasController(CajasRepository cajasRepository, CajasValidator validations, IHubContext<TurnosHub> turnoHub,CajasValidator2 rules) : ControllerBase
    {
        private CajasRepository _cajasRepository = cajasRepository;
        private CajasValidator _cajasValidator = validations;
        private readonly IHubContext<TurnosHub> _turnosHub = turnoHub;
        private readonly CajasValidator2 rules = rules;

        [HttpGet]
        public IActionResult Get()
        {
            var datos = _cajasRepository.GetAll().Select(x => new Cajas()
            {
                Estado = x.Estado,
                Nombre = x.Nombre,
                Username = x.Username,
                ConnectionId = x.ConnectionId,
                Contrasena = x.Contrasena,
                Id = x.Id
            });

            return Ok(datos);
        }


        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var datos = _cajasRepository.Get(id);
            return Ok(datos);
        }

        [HttpPost]
        public async Task<IActionResult> Post(CajasDto dto)
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
                        Estado = (int)EstadoCaja.Activa
                    };
                    cajas.Username = cajas.Username.ToUpper();
                    cajas.Estado = 0;
                    _cajasRepository.Insert(cajas);

                    var cajasDto = _cajasRepository.GetAll().Select(x => new CajasDto2
                    {
                        Id = x.Id,
                        Estado = x.Estado,
                        Nombre = x.Nombre,
                        NumeroActual = x.Estado == (int)EstadoCaja.Inactiva ? "Cerrada"
                             : x.Estado == (int)EstadoCaja.Activa ? "Activa"
                             : x.Turno.FirstOrDefault(y => y.Estado == EstadoTurno.Atendiendo.ToString())?.Numero.ToString() ?? "Cerrada",
                    }).ToList();

                    await _turnosHub.Clients.All.SendAsync("ActualizarCajas", cajasDto, 0);

                    return Created();
                }
                return BadRequest(results.Errors.Select(x => x.ErrorMessage));
            }
            return BadRequest();
        }

        [HttpPut]
        public IActionResult Put(CajasUpDto dto)
        {
            if (dto != null)
            {
                        
                var caja = _cajasRepository.Get(dto.Id);
                if (caja != null)
                {
                    var encrypter = new Encrypter();


                    var results = rules.Validate(dto);
                    if (results.IsValid)
                    {
                        //if (encrypter.IsPasswordChanged(caja.Contrasena, dto.Contrasena))
                        //{
                        //    dto.Contrasena = Encrypter.HashPassword(dto.Contrasena);

                        //}
                       caja.Username=dto.Username;
                        caja.Nombre=dto.Nombre;
                        _cajasRepository.Update(caja);
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
            var caja = _cajasRepository.Get(id);
            if (caja != null && caja.Estado != (int)EstadoCaja.Inactiva)
            {
                Cajas _cajas = new Cajas()
                {
                    Id = caja.Id,
                    Username = caja.Username,
                    Contrasena = caja.Contrasena,
                    Estado = (int)EstadoCaja.Inactiva,
                    Nombre = caja.Nombre,
                };
                _cajasRepository.Update(_cajas);
                return Ok();
            }
            return NotFound();
        }

    }
}
