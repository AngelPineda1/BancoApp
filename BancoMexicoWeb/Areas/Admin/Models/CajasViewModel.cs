namespace BancoMexicoWeb.Areas.Admin.Models
{
    public class CajasViewModel
    {
        public IEnumerable<Cajas> Cajas { get; set; } = [];
    }
    public class Cajas
    {
        public int Id { get; set; }

        public string Nombre { get; set; } = null!;

        public string Contrasena { get; set; } = null!;

        public int? Estado { get; set; }

        public string Username { get; set; } = null!;

        public string? ConnectionId { get; set; }
    }
}
