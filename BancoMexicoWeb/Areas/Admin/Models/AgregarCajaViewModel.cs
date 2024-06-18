namespace BancoMexicoWeb.Areas.Admin.Models
{
    public class AgregarCajaViewModel
    {
        public string Nombre { get; set; } = null!;

        public string Contrasena { get; set; } = null!;

        public int? Estado { get; set; }

        public string Username { get; set; } = null!;
    }
}
