using System;
using System.Collections.Generic;

namespace BancoAPI.Models.Entities;

public partial class Servicio
{
    public int Id { get; set; }

    public int IdTurno { get; set; }

    public int IdCaja { get; set; }

    public DateTime? FechaIncio { get; set; }

    public DateTime FechaTermino { get; set; }

    public string EstadoServicio { get; set; } = null!;

    public virtual Cajas IdCajaNavigation { get; set; } = null!;

    public virtual Turno IdTurnoNavigation { get; set; } = null!;
}
