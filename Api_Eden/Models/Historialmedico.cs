using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Api_Eden.Models;

[Table("historialmedico")]
[Index("AnimalId", Name = "idx_medico_animal")]
[Index("Fecha", Name = "idx_medico_fecha")]
[Index("VeterinarioId", Name = "idx_medico_veterinario")]
[MySqlCollation("utf8mb4_unicode_ci")]
public partial class Historialmedico
{
    [Key]
    public int Id { get; set; }

    [Column("animal_id")]
    public int AnimalId { get; set; }

    [Column("fecha", TypeName = "datetime")]
    public DateTime? Fecha { get; set; }

    [Column("diagnostico", TypeName = "text")]
    public string Diagnostico { get; set; } = null!;

    [Column("sintomas", TypeName = "text")]
    public string? Sintomas { get; set; }

    [Column("peso")]
    [Precision(5, 2)]
    public decimal? Peso { get; set; }

    [Column("temperatura")]
    [Precision(4, 2)]
    public decimal? Temperatura { get; set; }

    [Column("veterinario_id")]
    public int VeterinarioId { get; set; }

    [Column("observaciones", TypeName = "text")]
    public string? Observaciones { get; set; }

    [Column("fecha_creacion", TypeName = "datetime")]
    public DateTime? FechaCreacion { get; set; }

    [ForeignKey("AnimalId")]
    [InverseProperty("Historialmedicos")]
    public virtual Animale Animal { get; set; } = null!;

    [InverseProperty("HistorialMedico")]
    public virtual ICollection<Tratamiento> Tratamientos { get; set; } = new List<Tratamiento>();

    [ForeignKey("VeterinarioId")]
    [InverseProperty("Historialmedicos")]
    public virtual Usuario Veterinario { get; set; } = null!;
}
