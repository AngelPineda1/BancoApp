namespace BancoAPI.Models.Dtos
{
    public class UsuarioDto
    {
        public string Nombre { get; set; } = null!;

        public string Contrasena { get; set; }=null!;

        public string Username { get; set; } = null!;
    }


    public class UsuarioUpDto:UsuarioDto
    {
        public int Id { get; set; }
}
