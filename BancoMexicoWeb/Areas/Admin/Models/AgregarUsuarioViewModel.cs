namespace BancoMexicoWeb.Areas.Admin.Models
{
    public class AgregarUsuarioViewModel
    {
        public string Nombre { get; set; } = null!;

        public string? Contrasena { get; set; }

        public string Username { get; set; } = null!;
    }
}
