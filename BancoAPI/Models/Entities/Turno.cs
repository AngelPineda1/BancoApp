using System;
using System.Collections.Generic;

namespace BancoAPI.Models.Entities;

public partial class Turno
{
    public int Id { get; set; }

    public string Codigo { get; set; } = null!;

    public virtual ICollection<Servicio> Servicio { get; set; } = new List<Servicio>();
}
