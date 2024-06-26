﻿using BancoAPI.Models.Dtos;
using BancoAPI.Models.Entities;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BancoAPI.Repositories
{
    public class TurnosRepository(WebsitosBancoMexicoContext ctx) : Repository<Turno>(ctx)
    {
        private WebsitosBancoMexicoContext context = ctx;


        //public int GetAtendidos()
        //{
        //    return context.Turno.Where(x=>x.Estado=="Atendido").Count();
        //}

        //public int GetCancelados()
        //{
        //    return context.Turno.Where(x => x.Estado == "Cancelado").Count();

        //}

        public TurnosEstats GetTurnosInfo()
        {
            var turnos=new TurnosEstats();
            turnos.Atendidos= context.Turno.Where(x => x.Estado == "Atendido").Count();
            turnos.Cancelados= context.Turno.Where(x => x.Estado == "Cancelado").Count();
            turnos.TurnosInf = context.Turno.Include(x => x.IdCajaNavigation).Select(x => new Turnos()
            {
                Id = x.Id,
                IdCaja = x.IdCaja,
                CajaNombre = x.IdCajaNavigation.Nombre,
                FechaAtencion = x.FechaAtendido,
                FechaCreacion = x.FechaCreacion,
                FechaTermino = x.FechaTermino,
                Estado = x.Estado,
                Numero = x.Numero,
                Tiempo = new Duracion()
                {
                    Minutos = EF.Functions.DateDiffMinute(x.FechaAtendido, x.FechaTermino),
                    Segundos = EF.Functions.DateDiffSecond(x.FechaAtendido, x.FechaTermino) % 60

                }
            });
           
            return turnos;
        }
    }
}
