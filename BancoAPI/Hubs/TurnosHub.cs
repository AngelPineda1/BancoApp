using BancoAPI.Models.Entities;
using BancoAPI.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;
using BancoAPI.Models.Dtos;
using BancoAPI.Models.Enum;
using System.Text.Json;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using BancoAPI.Helpers;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BancoAPI.Hubs
{
    public class TurnosHub : Hub
    {
        private readonly TurnosRepository _turnoRepository;
        private readonly CajasRepository _cajasRepository;
        private readonly IHubContext<CajasHub> _cajasHub;
        private readonly IHubContext<EstadisticasHub> _estadisticasHub;
        public TurnosHub(TurnosRepository turnoRepository, CajasRepository cajasRepository, IHubContext<CajasHub> cajasHub, IHubContext<EstadisticasHub> estadisticasHub)
        {
            _turnoRepository = turnoRepository;
            _cajasRepository = cajasRepository;
            _cajasHub = cajasHub;
            _estadisticasHub = estadisticasHub;
        }
        public async Task GenerarTurno()
        {
            var connectionId = Context.ConnectionId;

            var turnos = _turnoRepository.GetAll();

            int ultimoTurno = 0;

            if (turnos.Any())
                ultimoTurno = turnos.Max(x => x.Numero);


            var numeroProximo = turnos
                .Where(x => x.Estado == EstadoTurno.Atendiendo.ToString())
                .OrderByDescending(x => x.Numero)
                .FirstOrDefault()?.Numero;


            var turno = new TurnoDto()
            {
                Numero = ultimoTurno + 1,
            };


            if (turno.Numero == (numeroProximo + 1))
                turno.Proximo = true;


            turno.Estado = EstadoTurno.Pendiente.ToString();


            var turnoInsertar = new Turno()
            {
                ConnectionId = connectionId,
                Numero = turno.Numero,
                Estado = turno.Estado,
                IdCaja = turno.IdCaja,
                FechaAtendido = turno.FechaAtencion
            };

            _turnoRepository.Insert(turnoInsertar);

            turno.Id = turnoInsertar.Id;

            var cajas = ObtenerCajas();

            int cajasDisponibles = _cajasRepository.GetAll().Where(x => x.Estado == (int)EstadoCaja.Activa).Count();
            string estadoActual = cajasDisponibles == 0 ? "Por el momento no hay cajas disponibles, por favor espere un momento" : "Espere un momento";


            var cajasActivas = cajas.Where(x => x.Estado == (int)EstadoCaja.Activa);

            if(cajasActivas.Count() > 0)
                await _cajasHub.Clients.All.SendAsync("HayClientesEsperando");

            int numeroProximo2 = ObtenerNumeroProximo();

            await Clients.Clients(connectionId).SendAsync("TurnoGenerado", turno, cajas, estadoActual, numeroProximo2);

            //Estadisticas
            var estadiscticas = CalcularEstadisticas();
            await _estadisticasHub.Clients.All.SendAsync("ActualizarEstadisticas", estadiscticas, cajas);
        }

 

        public async Task CancelarTurno(int id)
        {
            var turno = _turnoRepository.Get(id);

            if(turno == null)
                return;

            turno.Estado = EstadoTurno.Cancelado.ToString();
            turno.FechaTermino = DateTime.Now;

            _turnoRepository.Update(turno);


            int numeroProximo = 0;
            var turnoProximo = _turnoRepository.GetAll().Where(x => x.Estado == EstadoTurno.Pendiente.ToString());

            if(turnoProximo.Any())
                numeroProximo = turnoProximo.Min(x => x.Numero);

            await Clients.All.SendAsync("TurnoCancelado", numeroProximo);

            var estadiscticas = CalcularEstadisticas();
            var cajas = ObtenerCajas();
            await _estadisticasHub.Clients.All.SendAsync("ActualizarEstadisticas", estadiscticas, cajas);
        }




        public override async Task OnDisconnectedAsync(Exception? exception)
        {

            var turnoCancelado = _turnoRepository.GetAll()
                .FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);

            if (turnoCancelado != null)
            {
                if (turnoCancelado.Estado == EstadoTurno.Pendiente.ToString())
                {
                    turnoCancelado.Estado = EstadoTurno.Cancelado.ToString();
                    turnoCancelado.FechaTermino = DateTime.Now;
                    _turnoRepository.Update(turnoCancelado);
                }
                  
            }
            var cajas = ObtenerCajas();
            
            await Clients.All.SendAsync("CajaDesconectada", cajas);
            var estadiscticas = CalcularEstadisticas();
            await _estadisticasHub.Clients.All.SendAsync("ActualizarEstadisticas", estadiscticas, cajas);

            await base.OnDisconnectedAsync(exception);
        }


        public List<CajasDto2> ObtenerCajas()
        {
            var cajas = _cajasRepository.GetAll().Select(x => new CajasDto2
            {
                Id = x.Id,
                Estado = x.Estado,
                Nombre = x.Nombre,
                NumeroActual = x.Estado == (int)EstadoCaja.Inactiva ? "Cerrada"
                 : x.Estado == (int)EstadoCaja.Activa ? "Activa"
                 : x.Turno.FirstOrDefault(y => y.Estado == EstadoTurno.Atendiendo.ToString())?.Numero.ToString() ?? "Cerrada",
            }).ToList();


            return cajas;
        }


        public EstadisticasDto CalcularEstadisticas()
        {
            var turnosAtendidosLista = _turnoRepository.GetAll().Where(x => x.Estado == "Atendido").ToList();

            int turnosPendientes = _turnoRepository.GetAll().Where(x => x.Estado == "Pendiente" && x.FechaCreacion.Value.Date == DateTime.Now.Date).Count();
            int turnosAtendidos = _turnoRepository.GetAll().Where(x => x.Estado == "Atendido" && x.FechaAtendido.Value.Date == DateTime.Now.Date).Count();
            int turnosCancelados = _turnoRepository.GetAll().Where(x => x.Estado == "Cancelado" && x.FechaCreacion.Value.Date == DateTime.Now.Date).Count();

            double totalMinutos = 0;
            foreach (var turno in turnosAtendidosLista)
            {
                var diferencia = turno.FechaAtendido - turno.FechaCreacion;
                totalMinutos += diferencia == null ? 0 : diferencia.Value.TotalMinutes;
            }


            var estadisticas = new EstadisticasDto
            {
                TurnosAtendidos = turnosAtendidos,
                TurnosCancelados = turnosCancelados,
                TurnosPendientes = turnosPendientes,
                TiempoPromedio = turnosAtendidos == 0 ? 0 : Math.Abs((int)(totalMinutos / turnosAtendidos))
            };

            return estadisticas;
        }

        public int ObtenerNumeroProximo()
        {

            int numeroProximo = 0;
            var turnoProximo = _turnoRepository.GetAll().Where(x => x.Estado == EstadoTurno.Pendiente.ToString());

            if (turnoProximo.Any())
                numeroProximo = turnoProximo.Min(x => x.Numero);

            return numeroProximo;
        }

    }
}
