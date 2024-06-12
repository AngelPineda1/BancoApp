namespace BancoAPI.Models.Dtos
{
    public class CajasDto
    {
        public string Nombre { get; set; } = null!;

        public string Contrasena { get; set; } = null!;

        public bool? Activa { get; set; }

        public string Username { get; set; } = null!;
    }
}
