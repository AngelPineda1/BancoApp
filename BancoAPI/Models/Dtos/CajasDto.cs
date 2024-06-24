namespace BancoAPI.Models.Dtos
{
    public class CajasDto
    {
        public string Nombre { get; set; } = null!;

        public string Contrasena { get; set; } = null!;

        public int? Estado { get; set; }

        public string Username { get; set; } = null!;
    }

    public class CajasUpDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;

        public string Username { get; set; } = null!;
    }

    public class CajasDto2
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;

        public int? Estado { get; set; }

        public string NumeroActual { get; set; } = string.Empty;

    }

}
