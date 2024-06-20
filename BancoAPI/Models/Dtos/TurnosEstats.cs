namespace BancoAPI.Models.Dtos
{
    public class TurnosEstats
    {
        public int Atendidos {  get; set; }
        public int Cancelados { get; set; }

        public IEnumerable<Turnos> TurnosInf { get; set; } = [];
    }

    public class Turnos
    {
        public int Id { get; set; }

        public int Numero { get; set; }

        public string Estado { get; set; } = null!;

        public DateTime? FechaCreacion { get; set; }
        public DateTime? FechaAtencion { get; set; }
        public DateTime? FechaTermino { get; set; }

        public int? IdCaja { get; set; }


        public string CajaNombre { get; set; } = string.Empty;
        public Duracion? Tiempo { get; set; }
    }
    public class Duracion
    {
        public int? Minutos { get; set; }
        public int? Segundos { get; set; }
    }
}
