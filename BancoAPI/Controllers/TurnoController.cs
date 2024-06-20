using BancoAPI.Models.Dtos;
using BancoAPI.Models.Entities;
using BancoAPI.Models.Enum;
using BancoAPI.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BancoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TurnoController : ControllerBase
    {

        private readonly TurnosRepository _turnosRepositry;
        private readonly CajasRepository _cajasRepository;
        public TurnoController(TurnosRepository turnosRepositry, CajasRepository cajasRepository)
        {
            _turnosRepositry = turnosRepositry;
            _cajasRepository = cajasRepository;
        }


        [HttpGet]
        public IActionResult GetEstats()
        {
            var datos = new TurnosEstats()
            {
                Atendidos = _turnosRepositry.GetAtendidos(),
                Cancelados = _turnosRepositry.GetCancelados(),
                TurnosInf=_turnosRepositry.GetTurnosInfo()
            };
            return Ok(datos);
        }
        //[HttpPost]
        //public IActionResult Post()
        //{

           // var turnos = _turnosRepositry.GetAll();

           // int ultimoturno = 0;
           // if (turnos.Any())
           //     ultimoturno = turnos.Max(x => x.Numero);


           // var turno = new Turno()
           // {
           //     Numero = ultimoturno + 1,
           // };

           //var cajasDisponibles =  _cajasRepository.GetAll().Where(x => x.Activa == (int)EstadoCaja.Activa);
           
           // if (!cajasDisponibles.Any())
           //     turno.Estado = EstadoTurno.Pendiente.ToString();
           // else
           // {
           //     var caja = cajasDisponibles.FirstOrDefault();

           //     if(caja == null)
           //         return BadRequest("No hay cajas disponibles");

           //     turno.IdCaja = caja.Id;
           //     turno.Estado = EstadoTurno.Atendido.ToString();
           //     caja.Activa = (int)EstadoCaja.Ocupada;
           //     _cajasRepository.Update(caja);
           // }

           // _turnosRepositry.Insert(turno);


           // return Ok(turno);
       // }
    }
}
