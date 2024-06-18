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

    public string ConnectionId { get; set; } = null!;

    public DateTime? FechaAtendido { get; set; }

    public DateTime? FechaTermino { get; set; }

    public virtual Cajas? IdCajaNavigation { get; set; }
}
