using BancoAPI.Models.Entities;

namespace BancoAPI.Repositories
{
    public class TurnosRepository(WebsitosBancoMexicoContext ctx) : Repository<Turno>(ctx)
    {
        private WebsitosBancoMexicoContext context = ctx;


    }
}
