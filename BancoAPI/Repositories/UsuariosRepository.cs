using BancoAPI.Models.Entities;

namespace BancoAPI.Repositories
{
    public class UsuariosRepository:Repository<Usuarios>
    {
        private readonly WebsitosBancoMexicoContext context;
        public UsuariosRepository(WebsitosBancoMexicoContext ctx):base(ctx) 
        {
           context=ctx;
        }
        public Usuarios? Get(string usernam)
        {
            return Ctx.Usuarios.Where(x=>x.Username.Equals(usernam)).FirstOrDefault();
        }
    }
}
