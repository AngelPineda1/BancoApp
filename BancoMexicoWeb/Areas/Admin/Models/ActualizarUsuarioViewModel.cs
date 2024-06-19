namespace BancoMexicoWeb.Areas.Admin.Models
{
    public class ActualizarUsuarioViewModel
    {
        public int Id { get; set; }

        public string Nombre { get; set; } = null!;

        public string? Contrasena { get; set; }
        public string Username { get; set; } = null!;
    }
}
