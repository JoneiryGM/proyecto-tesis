using System;
using System.Collections.Generic;
using Api_Eden.Models;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace Api_Eden.Data;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Adopcione> Adopciones { get; set; }

    public virtual DbSet<Alimento> Alimentos { get; set; }

    public virtual DbSet<Animale> Animales { get; set; }

    public virtual DbSet<Categoriasgasto> Categoriasgastos { get; set; }

    public virtual DbSet<Cierresmensuale> Cierresmensuales { get; set; }

    public virtual DbSet<Donacione> Donaciones { get; set; }

    public virtual DbSet<Donante> Donantes { get; set; }

    public virtual DbSet<Especy> Especies { get; set; }

    public virtual DbSet<Estadosgenerale> Estadosgenerales { get; set; }

    public virtual DbSet<Fallecimiento> Fallecimientos { get; set; }

    public virtual DbSet<Gasto> Gastos { get; set; }

    public virtual DbSet<Historialmedico> Historialmedicos { get; set; }

    public virtual DbSet<Historialmovimiento> Historialmovimientos { get; set; }

    public virtual DbSet<Medicamento> Medicamentos { get; set; }

    public virtual DbSet<Movimientosinventario> Movimientosinventarios { get; set; }

    public virtual DbSet<Objetivo> Objetivos { get; set; }

    public virtual DbSet<Presupuestomensual> Presupuestomensuals { get; set; }

    public virtual DbSet<Tiposdonacion> Tiposdonacions { get; set; }

    public virtual DbSet<Tiposvacuna> Tiposvacunas { get; set; }

    public virtual DbSet<Transferencia> Transferencias { get; set; }

    public virtual DbSet<Tratamiento> Tratamientos { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    public virtual DbSet<VResumenfinancieromesactual> VResumenfinancieromesactuals { get; set; }

    public virtual DbSet<Vacuna> Vacunas { get; set; }

    public virtual DbSet<Zona> Zonas { get; set; }

 
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Adopcione>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.EstadoAdopcion).HasDefaultValueSql("'Pendiente'");
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.Animal).WithMany(p => p.Adopciones)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("adopciones_ibfk_1");

            entity.HasOne(d => d.UsuarioResponsable).WithMany(p => p.Adopciones)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("adopciones_ibfk_2");
        });

        modelBuilder.Entity<Alimento>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.Activo).HasDefaultValueSql("'1'");
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        modelBuilder.Entity<Animale>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.EstadoGeneral).HasDefaultValueSql("'Activo'");
            entity.Property(e => e.EstadoSalud).HasDefaultValueSql("'Saludable'");
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.FechaUltimaModificacion)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.Especie).WithMany(p => p.Animales)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_animales_especie");

            entity.HasOne(d => d.UsuarioRegistro).WithMany(p => p.Animales)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("animales_ibfk_2");

            entity.HasOne(d => d.ZonaActual).WithMany(p => p.Animales)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("animales_ibfk_1");
        });

        modelBuilder.Entity<Categoriasgasto>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.Activa).HasDefaultValueSql("'1'");
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        modelBuilder.Entity<Cierresmensuale>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasOne(d => d.UsuarioCierre).WithMany(p => p.Cierresmensuales)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("cierresmensuales_ibfk_1");
        });

        modelBuilder.Entity<Donacione>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.CertificadoGenerado).HasDefaultValueSql("'0'");
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.RequiereCertificado).HasDefaultValueSql("'0'");

            entity.HasOne(d => d.Alimento).WithMany(p => p.Donaciones)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("donaciones_ibfk_3");

            entity.HasOne(d => d.Donante).WithMany(p => p.Donaciones)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("donaciones_ibfk_1");

            entity.HasOne(d => d.Medicamento).WithMany(p => p.Donaciones)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("donaciones_ibfk_4");

            entity.HasOne(d => d.Objetivo).WithMany(p => p.Donaciones)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_donacion_objetivo");

            entity.HasOne(d => d.TipoDonacion).WithMany(p => p.Donaciones)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("donaciones_ibfk_2");

            entity.HasOne(d => d.UsuarioRegistro).WithMany(p => p.Donaciones)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("donaciones_ibfk_5");
        });

        modelBuilder.Entity<Donante>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.Activo).HasDefaultValueSql("'1'");
            entity.Property(e => e.EsRecurrente).HasDefaultValueSql("'0'");
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.FechaRegistro).HasDefaultValueSql("curdate()");
        });

        modelBuilder.Entity<Especy>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.Activa).HasDefaultValueSql("'1'");
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        modelBuilder.Entity<Estadosgenerale>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.FechaCambio).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.Animal).WithMany(p => p.Estadosgenerales).HasConstraintName("estadosgenerales_ibfk_1");

            entity.HasOne(d => d.UsuarioResponsable).WithMany(p => p.Estadosgenerales)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("estadosgenerales_ibfk_2");
        });

        modelBuilder.Entity<Fallecimiento>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.Animal).WithMany(p => p.Fallecimientos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fallecimientos_ibfk_1");

            entity.HasOne(d => d.UsuarioRegistro).WithMany(p => p.FallecimientoUsuarioRegistros)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fallecimientos_ibfk_3");

            entity.HasOne(d => d.Veterinario).WithMany(p => p.FallecimientoVeterinarios)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fallecimientos_ibfk_2");
        });

        modelBuilder.Entity<Gasto>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.Alimento).WithMany(p => p.Gastos)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("gastos_ibfk_2");

            entity.HasOne(d => d.CategoriaGasto).WithMany(p => p.Gastos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("gastos_ibfk_1");

            entity.HasOne(d => d.Medicamento).WithMany(p => p.Gastos)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("gastos_ibfk_3");

            entity.HasOne(d => d.UsuarioRegistro).WithMany(p => p.Gastos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("gastos_ibfk_4");
        });

        modelBuilder.Entity<Historialmedico>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.Fecha).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.Animal).WithMany(p => p.Historialmedicos).HasConstraintName("historialmedico_ibfk_1");

            entity.HasOne(d => d.Veterinario).WithMany(p => p.Historialmedicos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("historialmedico_ibfk_2");
        });

        modelBuilder.Entity<Historialmovimiento>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.Fecha).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.Animal).WithMany(p => p.Historialmovimientos).HasConstraintName("historialmovimientos_ibfk_1");

            entity.HasOne(d => d.UsuarioResponsable).WithMany(p => p.Historialmovimientos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("historialmovimientos_ibfk_4");

            entity.HasOne(d => d.ZonaDestino).WithMany(p => p.HistorialmovimientoZonaDestinos)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("historialmovimientos_ibfk_3");

            entity.HasOne(d => d.ZonaOrigen).WithMany(p => p.HistorialmovimientoZonaOrigens)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("historialmovimientos_ibfk_2");
        });

        modelBuilder.Entity<Medicamento>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.Activo).HasDefaultValueSql("'1'");
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.RequiereReceta).HasDefaultValueSql("'0'");
        });

        modelBuilder.Entity<Movimientosinventario>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.FechaMovimiento).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.Alimento).WithMany(p => p.Movimientosinventarios)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("movimientosinventario_ibfk_1");

            entity.HasOne(d => d.UsuarioResponsable).WithMany(p => p.Movimientosinventarios)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("movimientosinventario_ibfk_2");
        });

        modelBuilder.Entity<Objetivo>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.Estado).HasDefaultValueSql("'Activo'");
            entity.Property(e => e.FechaActualizacion)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.UsuarioCreo).WithMany(p => p.Objetivos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("objetivos_ibfk_1");
        });

        modelBuilder.Entity<Presupuestomensual>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.MontoEjecutado).HasDefaultValueSql("'0.00'");

            entity.HasOne(d => d.CategoriaGasto).WithMany(p => p.Presupuestomensuals)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("presupuestomensual_ibfk_1");
        });

        modelBuilder.Entity<Tiposdonacion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.Activa).HasDefaultValueSql("'1'");
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        modelBuilder.Entity<Tiposvacuna>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.Activa).HasDefaultValueSql("'1'");
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Obligatoria).HasDefaultValueSql("'0'");

            entity.HasOne(d => d.Especie).WithMany(p => p.Tiposvacunas)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("tiposvacunas_ibfk_1");
        });

        modelBuilder.Entity<Transferencia>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.Animal).WithMany(p => p.Transferencia)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("transferencias_ibfk_1");

            entity.HasOne(d => d.UsuarioResponsable).WithMany(p => p.Transferencia)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("transferencias_ibfk_2");
        });

        modelBuilder.Entity<Tratamiento>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.Estado).HasDefaultValueSql("'Activo'");
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.HistorialMedico).WithMany(p => p.Tratamientos).HasConstraintName("tratamientos_ibfk_1");

            entity.HasOne(d => d.Medicamento).WithMany(p => p.Tratamientos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("tratamientos_ibfk_4");

            entity.HasOne(d => d.Veterinario).WithMany(p => p.Tratamientos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("tratamientos_ibfk_3");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.Activo).HasDefaultValueSql("'1'");
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.FechaUltimaModificacion)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        modelBuilder.Entity<VResumenfinancieromesactual>(entity =>
        {
            entity.ToView("v_resumenfinancieromesactual");

            entity.Property(e => e.Periodo).HasDefaultValueSql("''");
        });

        modelBuilder.Entity<Vacuna>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.Animal).WithMany(p => p.Vacunas).HasConstraintName("vacunas_ibfk_1");

            entity.HasOne(d => d.TipoVacuna).WithMany(p => p.Vacunas)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("vacunas_ibfk_3");

            entity.HasOne(d => d.Veterinario).WithMany(p => p.Vacunas)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("vacunas_ibfk_2");
        });

        modelBuilder.Entity<Zona>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.Activa).HasDefaultValueSql("'1'");
            entity.Property(e => e.CantidadActual).HasDefaultValueSql("'0'");
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
