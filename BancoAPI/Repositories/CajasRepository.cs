using BancoAPI.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace BancoAPI.Repositories
{
    public class CajasRepository(WebsitosBancoMexicoContext ctx) : Repository<Cajas>(ctx)
    {
        public WebsitosBancoMexicoContext Context { get; } = ctx;

        public Cajas? Get(string usernam)
        {
            return Ctx.Cajas.Where(x => x.Username.Equals(usernam)).FirstOrDefault();
        }

        public override IEnumerable<Cajas> GetAll()
        {
            return Ctx.Cajas.Include(x => x.Turno).OrderBy(x => x.Nombre);
        }

    }
}
