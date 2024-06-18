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

    public virtual DbSet<Turno> Turno { get; set; }

    public virtual DbSet<Usuarios> Usuarios { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySql("database=websitos_BancoMexico;user=websitos_AdminBancoMexico;server=websitos256.com;password=Banco2024", Microsoft.EntityFrameworkCore.ServerVersion.Parse("10.11.7-mariadb"));

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
            entity.Property(e => e.ConnectionId).HasMaxLength(100);
            entity.Property(e => e.Contrasena).HasMaxLength(300);
            entity.Property(e => e.Estado).HasColumnType("int(11)");
            entity.Property(e => e.Nombre).HasMaxLength(100);
            entity.Property(e => e.Username).HasMaxLength(50);
        });

        modelBuilder.Entity<Turno>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("turno");

            entity.HasIndex(e => e.IdCaja, "turno_cajas_FK");

            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.ConnectionId).HasMaxLength(100);
            entity.Property(e => e.Estado).HasMaxLength(100);
            entity.Property(e => e.FechaAtendido).HasColumnType("datetime");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("datetime");
            entity.Property(e => e.FechaTermino).HasColumnType("datetime");
            entity.Property(e => e.IdCaja).HasColumnType("int(11)");
            entity.Property(e => e.Numero).HasColumnType("int(11)");

            entity.HasOne(d => d.IdCajaNavigation).WithMany(p => p.Turno)
                .HasForeignKey(d => d.IdCaja)
                .HasConstraintName("turno_cajas_FK");
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
