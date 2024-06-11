using BancoAPI.Models.Entities;

namespace BancoAPI.Repositories
{
    public class CajasRepository:Repository<Cajas>
    {
        public CajasRepository(WebsitosBancoMexicoContext ctx):base(ctx) 
        {
           Context=ctx;
        }

        public WebsitosBancoMexicoContext Context { get; }

        public Cajas? Get(string usernam)
        {
            return Ctx.Cajas.Where(x => x.Username.Equals(usernam)).FirstOrDefault();
        }
    }
}
