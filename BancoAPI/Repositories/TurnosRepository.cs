using BancoAPI.Models.Dtos;
using BancoAPI.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace BancoAPI.Repositories
{
    public class TurnosRepository(WebsitosBancoMexicoContext ctx) : Repository<Turno>(ctx)
    {
        private WebsitosBancoMexicoContext context = ctx;


        public int GetAtendidos()
        {
            return context.Turno.Where(x=>x.Estado=="Atendido").Count();
        }

        public int GetCancelados()
        {
            return context.Turno.Where(x => x.Estado == "Cancelado").Count();

        }

        public IEnumerable<Turnos> GetTurnosInfo()
        {
            return context.Turno.Include(x => x.IdCajaNavigation).Select(x => new Turnos()
            {
                Id = x.Id,
                IdCaja = x.IdCaja,
                CajaNombre = x.IdCajaNavigation.Nombre,
                FechaAtencion = x.FechaAtendido,
                FechaCreacion = x.FechaCreacion,
                FechaTermino = x.FechaTermino,
                Estado = x.Estado,
                Numero = x.Numero,
                Tiempo = new Duracion()
                {
                    Minutos = EF.Functions.DateDiffMinute(x.FechaAtendido, x.FechaTermino),
                    Segundos = EF.Functions.DateDiffSecond(x.FechaAtendido, x.FechaTermino) % 60

                }
            });
        }
    }
}
