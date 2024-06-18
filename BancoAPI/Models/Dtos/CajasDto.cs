namespace BancoAPI.Models.Dtos
{
    public class CajasDto
    {
        public string Nombre { get; set; } = null!;

        public string Contrasena { get; set; } = null!;

        public int? Estado { get; set; }

        public string Username { get; set; } = null!;
    }

    public class CajasUpDto:CajasDto
    {
        public int Id { get; set; } 
    }

    public class CajasDto2
    {
        public string Nombre { get; set; } = null!;

        public int? Estado { get; set; }

        public string NumeroActual { get; set; } = string.Empty;

    }

}
