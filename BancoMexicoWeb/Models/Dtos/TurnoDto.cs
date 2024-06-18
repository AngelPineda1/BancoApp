namespace BancoAPI.Models.Dtos
{
    public class TurnoDto
    {
        public int Id { get; set; }

        public int Numero { get; set; }

        public string Estado { get; set; } = null!;

        public DateTime? FechaCreacion { get; set; }
        public DateTime? FechaAtencion { get; set; }
        public bool Proximo { get; set; } = false;

        public int? IdCaja { get; set; }

        public string CajaNombre { get; set; } = string.Empty;

    }
}
