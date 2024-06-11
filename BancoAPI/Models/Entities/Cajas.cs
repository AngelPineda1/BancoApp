﻿using System;
using System.Collections.Generic;

namespace BancoAPI.Models.Entities;

public partial class Cajas
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string Contrasena { get; set; } = null!;

    public bool? Activa { get; set; }

    public string Username { get; set; } = null!;

    public virtual ICollection<Servicio> Servicio { get; set; } = new List<Servicio>();
}
