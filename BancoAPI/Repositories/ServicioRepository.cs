using BancoAPI.Models.Entities;

namespace BancoAPI.Repositories
{
    public class ServicioRepository(WebsitosBancoMexicoContext ctx) : Repository<Servicio>(ctx)
    {
        private WebsitosBancoMexicoContext context = ctx;
    }
}
