using BancoAPI.Helpers;
using BancoAPI.Models.Dtos;
using BancoAPI.Models.Entities;
using BancoAPI.Models.Enum;
using BancoAPI.Models.Validators;
using BancoAPI.Repositories;
using Microsoft.AspNetCore.SignalR;

namespace BancoAPI.Hubs
{
    public class CajasHub : Hub
    {
        private readonly TurnosRepository _turnoRepository;
        private readonly CajasRepository _cajasRepository;
        private readonly IHubContext<TurnosHub> _turnosHub;
        private readonly IHubContext<EstadisticasHub> _estadisticasHub;
        public CajasHub(TurnosRepository turnoRepository, CajasRepository cajasRepository, IHubContext<TurnosHub> turnosHub, IHubContext<EstadisticasHub> estadisticasHub)
        {
            _turnoRepository = turnoRepository;
            _cajasRepository = cajasRepository;
            _turnosHub = turnosHub;
            _estadisticasHub = estadisticasHub;
        }
        public override async Task OnConnectedAsync()
        {
            await Clients.Caller.SendAsync("OnConnected");

            await base.OnConnectedAsync();
        }

        public async Task ActivarCaja(int idCaja)
        {
            var caja = _cajasRepository.Get(idCaja);
            if (caja == null)
                return;

            caja.ConnectionId = Context.ConnectionId;
            caja.Estado = (int)EstadoCaja.Activa;

            _cajasRepository.Update(caja);

            var cajas = ObtenerCajas();

            await _turnosHub.Clients.All.SendAsync("ActualizarCajas", cajas);
            var estadiscticas = CalcularEstadisticas();
            await _estadisticasHub.Clients.All.SendAsync("ActualizarEstadisticas", estadiscticas, cajas);
        }

        public async Task AtenderCliente(int idcaja)
        {

            var cajaExite = _cajasRepository.Get(idcaja);

            if (cajaExite == null)
                return;

            cajaExite.ConnectionId = Context.ConnectionId;

            var turnoSiguiente = _turnoRepository.GetAll()
                .Where(x => x.Estado == EstadoTurno.Pendiente.ToString())
                .OrderBy(x => x.Numero)
                .FirstOrDefault();


            var turnoFuturo = _turnoRepository.GetAll()
               .Where(x => x.Estado == EstadoTurno.Pendiente.ToString())
               .OrderBy(x => x.Numero)
               .Skip(1)
               .FirstOrDefault();



            var turnoAnterior = _turnoRepository.GetAll()
                .FirstOrDefault(x => x.Estado == EstadoTurno.Atendiendo.ToString() && x.IdCaja == idcaja);

            if (turnoAnterior != null)
            {
                turnoAnterior.Estado = EstadoTurno.Atendido.ToString();
                turnoAnterior.FechaTermino = DateTime.Now;
                _turnoRepository.Update(turnoAnterior);
            }


            cajaExite.Estado = turnoSiguiente == null ? (int)EstadoCaja.Activa : (int)EstadoCaja.Ocupada;

            if (turnoSiguiente != null)
            {
                turnoSiguiente.FechaAtendido = DateTime.Now;
                turnoSiguiente.Estado = EstadoTurno.Atendiendo.ToString();
                turnoSiguiente.IdCaja = idcaja;
                _turnoRepository.Update(turnoSiguiente);
            }


            _cajasRepository.Update(cajaExite);


            var cajas = ObtenerCajas();


            var turnoDto = new TurnoDto()
            {
                Numero = turnoSiguiente?.Numero ?? 0,
                Estado = turnoSiguiente?.Estado ?? EstadoTurno.Pendiente.ToString(),
                IdCaja = turnoSiguiente?.IdCaja ?? 0,
                CajaNombre = cajaExite.Nombre.ToUpper(),
            };


           await Clients.All.SendAsync("TurnoAtendido", turnoDto, cajas, cajaExite.Id, turnoFuturo?.Numero ?? 0);
           await _turnosHub.Clients.All.SendAsync("EstadoTurnoActual", turnoDto, cajas, cajaExite.Id, turnoFuturo?.Numero ?? 0);
            var estadiscticas = CalcularEstadisticas();
            await _estadisticasHub.Clients.All.SendAsync("ActualizarEstadisticas", estadiscticas, cajas);
        }


        public async Task CancelarTurno(int idturno, int idcaja)
        {
            var cajaExite = _cajasRepository.Get(idcaja);

            if (cajaExite == null)
                return;

            var turnoCancelar = _turnoRepository.GetAll().FirstOrDefault(x => x.Numero == idturno);

            if (turnoCancelar == null)
                return;

            turnoCancelar.Estado = EstadoTurno.Cancelado.ToString();
            turnoCancelar.FechaTermino = DateTime.Now;

            var turnoCancelarDto = new TurnoDto()
            {
                Numero = turnoCancelar.Numero,
                Estado = turnoCancelar.Estado,
                IdCaja = turnoCancelar.IdCaja,
            };

            _turnoRepository.Update(turnoCancelar);


            int numeroProximo = 0;
            var turnoProximo = _turnoRepository.GetAll().Where(x => x.Estado == EstadoTurno.Pendiente.ToString());

            if (turnoProximo.Any())
                numeroProximo = turnoProximo.Min(x => x.Numero);


            await _turnosHub.Clients.All.SendAsync("TurnoCanceladoCaja", turnoCancelarDto, cajaExite.Id, turnoProximo);
            await Clients.All.SendAsync("TurnoCancelado", turnoCancelarDto, cajaExite.Id, turnoProximo);
            var estadiscticas = CalcularEstadisticas();
            var cajas = ObtenerCajas();
            await _estadisticasHub.Clients.All.SendAsync("ActualizarEstadisticas", estadiscticas, cajas);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var caja = _cajasRepository.GetAll()
                        .FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
           

            if (caja != null)
            {
                caja.Estado = (int)EstadoCaja.Inactiva;
                caja.ConnectionId = null;
                var turnoAtendiendo = caja.Turno.FirstOrDefault(x => x.Estado == EstadoTurno.Atendiendo.ToString());

                if (turnoAtendiendo != null)
                {
                    turnoAtendiendo.Estado = EstadoTurno.Atendido.ToString();
                    turnoAtendiendo.FechaTermino = DateTime.Now;
                    _turnoRepository.Update(turnoAtendiendo);
                }
                _cajasRepository.Update(caja);

                var cajas = ObtenerCajas();

                var estadiscticas = CalcularEstadisticas();
                await _estadisticasHub.Clients.All.SendAsync("ActualizarEstadisticas", estadiscticas, cajas);
                await _turnosHub.Clients.All.SendAsync("ActualizarCajas", cajas);

            }
            await base.OnDisconnectedAsync(exception);
          
        }
        public List<CajasDto2> ObtenerCajas()
        {
            var cajas = _cajasRepository.GetAll().Select(x => new CajasDto2
            {
                Estado = x.Estado,
                Nombre = x.Nombre,
                NumeroActual = x.Estado == (int)EstadoCaja.Inactiva ? "Cerrada" 
                : x.Estado == (int)EstadoCaja.Activa ? "Activa" 
                : x.Turno.FirstOrDefault(y => y.Estado == EstadoTurno.Atendiendo.ToString())?.Numero.ToString() ?? "Cerrada" ,
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



    }
}
