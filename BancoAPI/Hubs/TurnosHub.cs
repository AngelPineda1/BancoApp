using BancoAPI.Models.Entities;
using BancoAPI.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;
using BancoAPI.Models.Dtos;

namespace BancoAPI.Hubs
{
    public class TurnosHub(TurnosRepository turnosRepository, ServicioRepository servicioRepository):Hub
    {
        private TurnosRepository _repository = turnosRepository;
        private readonly ServicioRepository servicioRepository = servicioRepository;

        public async Task<Turno> CrearNuevoTurnoAsync()
        {
            // Recupera el último turno creado
            var Turnos = _repository.GetAll();
            Turnos.AsEnumerable();
            Turnos.OrderByDescending(x => x.Id);
           var ultimoTurno= Turnos.OrderByDescending(x => x.Id).FirstOrDefault();
            string nuevoCodigo = GenerarSiguienteCodigo(ultimoTurno?.Codigo);

            // Crea y guarda el nuevo turno
            var nuevoTurno = new Turno()
            {
                Id = 0,
                Codigo = nuevoCodigo,
            };
            _repository.Insert(nuevoTurno);

            await Clients.All.SendAsync("TurnoCreado", nuevoTurno);

            return nuevoTurno;
        }

        private string GenerarSiguienteCodigo(string? codigo)
        {
            if (string.IsNullOrEmpty(codigo))
            {
                return "000";
            }

            char[] chars = codigo.ToCharArray();
            for (int i = chars.Length - 1; i >= 0; i--)
            {
                if (chars[i] == '9')
                {
                    chars[i] = 'A';
                    break;
                }
                else if (chars[i] == 'Z')
                {
                    chars[i] = '0';
                }
                else if (chars[i] == '9' || (chars[i] >= 'A' && chars[i] < 'Z'))
                {
                    chars[i]++;
                    break;
                }
                else
                {
                    chars[i]++;
                    break;
                }
            }

            return new string(chars);
        }



    }
}
