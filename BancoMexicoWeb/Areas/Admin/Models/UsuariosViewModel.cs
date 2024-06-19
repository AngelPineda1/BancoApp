namespace BancoMexicoWeb.Areas.Admin.Models
{
    public class UsuariosViewModel
    {
        public IEnumerable<Usuarios> Usuarios { get; set; } = [];
    }

    public class Usuarios()
    {
        public int Id { get; set; }

        public string Nombre { get; set; } = null!;

        public string? Contrasena { get; set; }

        public string Username { get; set; } = null!;
    }
}
