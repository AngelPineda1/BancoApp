using BancoAPI.Models.Entities;
using BancoAPI.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;
using BancoAPI.Models.Dtos;
using BancoAPI.Models.Enum;
using System.Text.Json;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace BancoAPI.Hubs
{
    public class TurnosHub(TurnosRepository turnosRepository, CajasRepository cajasRepository) : Hub
    {
        private readonly TurnosRepository _turnoRepository = turnosRepository;
        private readonly CajasRepository _cajasRepository = cajasRepository;

        public async Task GenerarTurno()
        {
            var connectionId = Context.ConnectionId;

            string cajaConnectionId = "";

            var turnos = _turnoRepository.GetAll();

            int ultimoTurno = 0;

            if (turnos.Any())
                ultimoTurno = turnos.Max(x => x.Numero);

            var turno = new TurnoDto()
            {
                Numero = ultimoTurno + 1,
            };

            var cajasDisponibles = _cajasRepository.GetAll().Where(x => x.Estado == (int)EstadoCaja.Activa);


            if (!cajasDisponibles.Any())
                turno.Estado = EstadoTurno.Pendiente.ToString();
            else
            {
                var caja = cajasDisponibles.First();

                turno.IdCaja = caja.Id;


                turno.Estado = EstadoTurno.Atendiendo.ToString();
                turno.CajaNombre = caja.Nombre.ToUpper();
                turno.FechaAtencion = DateTime.Now;
                caja.Estado = (int)EstadoCaja.Ocupada;
                
                if (caja.ConnectionId != null)
                    cajaConnectionId =  caja.ConnectionId;
                
                _cajasRepository.Update(caja);
            }

            _turnoRepository.Insert(new()
            {
                ConnectionId = connectionId,
                Numero = turno.Numero,
                Estado = turno.Estado,
                IdCaja = turno.IdCaja,
                FechaAtendido = turno.FechaAtencion,
            });


            var cajas = _cajasRepository.GetAll().Select(x => new CajasDto2
            {
                Estado = x.Estado,
                Nombre = x.Nombre,
                NumeroActual = x.Turno.FirstOrDefault(y => y.Estado == EstadoTurno.Atendiendo.ToString())?.Numero.ToString() ?? "Vacía",
            }).ToList();

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };

            var jsonCajas = JsonSerializer.Serialize(cajas, jsonOptions);


            await Clients.Clients(connectionId,cajaConnectionId).SendAsync("TurnoGenerado", turno, jsonCajas);
        }

        public async Task AtenderCliente(int idcaja)
        {

            var cajaExite = _cajasRepository.Get(idcaja);

            if (cajaExite == null)
                return;

            cajaExite.ConnectionId ??= Context.ConnectionId;

            var turnoSiguiente = _turnoRepository.GetAll()
                .Where(x => x.Estado == EstadoTurno.Pendiente.ToString())
                .OrderBy(x => x.Numero)
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


            var cajas = _cajasRepository.GetAll().Select(x => new CajasDto2
            {
                Estado = x.Estado,
                Nombre = x.Nombre,
                NumeroActual = x.Turno.FirstOrDefault(y => y.Estado == EstadoTurno.Atendiendo.ToString())?.Numero.ToString() ?? "Vacía",
            }).ToList();


            var turnoDto = new TurnoDto()
            {
                Numero = turnoSiguiente?.Numero ?? 0,
                Estado = turnoSiguiente?.Estado ?? EstadoTurno.Pendiente.ToString(),
                IdCaja = turnoSiguiente?.IdCaja ?? 0,
                CajaNombre = cajaExite.Nombre.ToUpper(),
            };



            await Clients.All.SendAsync("TurnoAtendido", turnoDto, cajas, cajaExite.Id);

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

                var cajas = _cajasRepository.GetAll().Select(x => new CajasDto2
                {
                    Estado = x.Estado,
                    Nombre = x.Nombre,
                    NumeroActual = x.Turno.FirstOrDefault(y => y.Estado == EstadoTurno.Atendiendo.ToString())?.Numero.ToString() ?? "Vacía",
                }).ToList();

                await Clients.All.SendAsync("CajaDesconectada", cajas);
            }

            await base.OnDisconnectedAsync(exception);
        }






    }
}
