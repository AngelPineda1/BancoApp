using BancoAPI.Models.Dtos;
using BancoAPI.Models.Enum;
using BancoAPI.Repositories;
using Microsoft.AspNetCore.SignalR;

namespace BancoAPI.Hubs
{
    public class EstadisticasHub : Hub
    {
        private readonly TurnosRepository _turnoRepository;
        private readonly CajasRepository _cajasRepository;

        public EstadisticasHub(TurnosRepository turnoRepository, CajasRepository cajasRepository)
        {
            _turnoRepository = turnoRepository;
            _cajasRepository = cajasRepository;
        }

        public override Task OnConnectedAsync()
        {
            Clients.Caller.SendAsync("OnConnected");
            return base.OnConnectedAsync();
        }

        public async Task Actualizar()
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
                TiempoPromedio = turnosAtendidos == 0 ? 0 :  Math.Abs((int)(totalMinutos / turnosAtendidos))
            };

            var cajas = ObtenerCajas();

            await Clients.All.SendAsync("ActualizarEstadisticas", estadisticas, cajas);
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
    }
}
