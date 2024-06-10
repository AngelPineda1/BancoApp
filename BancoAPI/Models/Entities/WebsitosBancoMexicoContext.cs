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

    public virtual DbSet<Turno> Turno { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySql("database=websitos_BancoMexico;user=websitos_AdminBancoMexico;server=websitos256.com;password=Banco2024", Microsoft.EntityFrameworkCore.ServerVersion.Parse("10.11.7-mariadb"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb3_general_ci")
            .HasCharSet("utf8mb3");

        modelBuilder.Entity<Turno>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("turno");

            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.Codigo)
                .HasMaxLength(3)
                .IsFixedLength();
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
