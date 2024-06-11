using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace BancoAPI.Models.Entities;

public partial class WebsitosBancoMexicoContext : DbContext
{
    public WebsitosBancoMexicoContext()
    {
    }

    public WebsitosBancoMexicoContext(DbContextOptions<WebsitosBancoMexicoContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Cajas> Cajas { get; set; }

    public virtual DbSet<Servicio> Servicio { get; set; }

    public virtual DbSet<Turno> Turno { get; set; }

    public virtual DbSet<Usuarios> Usuarios { get; set; }

   
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb3_general_ci")
            .HasCharSet("utf8mb3");

        modelBuilder.Entity<Cajas>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("cajas");

            entity.HasIndex(e => e.Username, "Username_UNIQUE").IsUnique();

            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.Contrasena).HasMaxLength(300);
            entity.Property(e => e.Nombre).HasMaxLength(100);
            entity.Property(e => e.Username).HasMaxLength(50);
        });

        modelBuilder.Entity<Servicio>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("servicio");

            entity.HasIndex(e => e.IdCaja, "servicio_cajas_FK");

            entity.HasIndex(e => e.IdTurno, "servicio_turno_FK");

            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.IdCaja).HasColumnType("int(11)");
            entity.Property(e => e.IdTurno).HasColumnType("int(11)");
            entity.Property(e => e.Ocupada).HasColumnType("smallint(1)");

            entity.HasOne(d => d.IdCajaNavigation).WithMany(p => p.Servicio)
                .HasForeignKey(d => d.IdCaja)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("servicio_cajas_FK");

            entity.HasOne(d => d.IdTurnoNavigation).WithMany(p => p.Servicio)
                .HasForeignKey(d => d.IdTurno)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("servicio_turno_FK");
        });

        modelBuilder.Entity<Turno>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("turno");

            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.Codigo)
                .HasMaxLength(3)
                .IsFixedLength();
        });

        modelBuilder.Entity<Usuarios>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("usuarios");

            entity.HasIndex(e => e.Username, "Username_UNIQUE").IsUnique();

            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.Contrasena).HasMaxLength(300);
            entity.Property(e => e.Nombre).HasMaxLength(100);
            entity.Property(e => e.Username).HasMaxLength(45);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
