using BancoAPI.Repositories;
using Microsoft.AspNetCore.SignalR;

namespace BancoAPI.Hubs
{
    public class EstadisticasHub(TurnosRepository turnosRepository) : Hub
    {
        public TurnosRepository TurnosRepository { get; } = turnosRepository;


        public async Task Estadisticas()
        {
           
            var turnos=TurnosRepository.GetTurnosInfo();
            await Clients.All.SendAsync("EstadisticasActualizadas",turnos);

        }
    }
}
