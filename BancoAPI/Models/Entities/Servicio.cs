using System;
using System.Collections.Generic;

namespace BancoAPI.Models.Entities;

public partial class Servicio
{
    public int Id { get; set; }

    public int IdTurno { get; set; }

    public int IdCaja { get; set; }

    public short Ocupada { get; set; }

    public virtual Cajas IdCajaNavigation { get; set; } = null!;

    public virtual Turno IdTurnoNavigation { get; set; } = null!;
}
