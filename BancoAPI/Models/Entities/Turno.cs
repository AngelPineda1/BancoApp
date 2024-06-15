using System;
using System.Collections.Generic;

namespace BancoAPI.Models.Entities;

public partial class Turno
{
    public int Id { get; set; }

    public int Numero { get; set; }

    public string Estado { get; set; } = null!;

    public DateTime? FechaCreacion { get; set; }

    public int? IdCaja { get; set; }

    public virtual Cajas? IdCajaNavigation { get; set; }

    public virtual ICollection<Servicio> Servicio { get; set; } = new List<Servicio>();
}
