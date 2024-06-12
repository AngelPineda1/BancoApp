using BancoAPI.Models.Entities;

namespace BancoAPI.Repositories
{
    public class CajasRepository(WebsitosBancoMexicoContext ctx) : Repository<Cajas>(ctx)
    {
        public WebsitosBancoMexicoContext Context { get; } = ctx;

        public Cajas? Get(string usernam)
        {
            return Ctx.Cajas.Where(x => x.Username.Equals(usernam)).FirstOrDefault();
        }
    }
}
