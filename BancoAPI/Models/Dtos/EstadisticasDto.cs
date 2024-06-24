namespace BancoAPI.Models.Dtos
{
    public class EstadisticasDto
    {
        public int TurnosAtendidos { get; set; }
        public int TurnosCancelados { get; set; }
        public int TurnosPendientes { get; set; }
        public int TiempoPromedio { get; set; }
    }
}
