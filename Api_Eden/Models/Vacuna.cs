using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Api_Eden.Models;

[Table("vacunas")]
[Index("TipoVacunaId", Name = "TipoVacunaId")]
[Index("VeterinarioId", Name = "VeterinarioId")]
[Index("AnimalId", Name = "idx_vacunas_animal")]
[Index("FechaAplicacion", Name = "idx_vacunas_fecha")]
[Index("ProximaDosis", Name = "idx_vacunas_proxima")]
[MySqlCollation("utf8mb4_unicode_ci")]
public partial class Vacuna
{
    [Key]
    public int Id { get; set; }

    [Column("animal_id")]
    public int AnimalId { get; set; }

    [Column("tipo_vacuna_id")]
    public int TipoVacunaId { get; set; }

    [Column("fecha_aplicacion")]
    public DateOnly FechaAplicacion { get; set; }

    [Column("proxima_dosis")]
    public DateOnly? ProximaDosis { get; set; }

    [Column("lote")]
    [StringLength(50)]
    public string? Lote { get; set; }

    [Column("veterinario_id")]
    public int VeterinarioId { get; set; }

    [Column("observaciones", TypeName = "text")]
    public string? Observaciones { get; set; }

    [Column("fecha_creacion", TypeName = "datetime")]
    public DateTime? FechaCreacion { get; set; }

    [ForeignKey("AnimalId")]
    [InverseProperty("Vacunas")]
    public virtual Animale Animal { get; set; } = null!;

    [ForeignKey("TipoVacunaId")]
    [InverseProperty("Vacunas")]
    public virtual Tiposvacuna TipoVacuna { get; set; } = null!;

    [ForeignKey("VeterinarioId")]
    [InverseProperty("Vacunas")]
    public virtual Usuario Veterinario { get; set; } = null!;
}
