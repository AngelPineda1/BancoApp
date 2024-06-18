using System;
using System.Collections.Generic;

namespace BancoAPI.Models.Entities;

public partial class Usuarios
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Contrasena { get; set; }

    public string Username { get; set; } = null!;
}
