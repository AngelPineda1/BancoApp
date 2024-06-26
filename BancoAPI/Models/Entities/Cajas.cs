﻿using System;
using System.Collections.Generic;

namespace BancoAPI.Models.Entities;

public partial class Cajas
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string Contrasena { get; set; } = null!;

    public int? Estado { get; set; }

    public string Username { get; set; } = null!;

    public string? ConnectionId { get; set; }

    public virtual ICollection<Turno> Turno { get; set; } = new List<Turno>();
}
