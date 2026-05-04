using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Api_Eden.Models;

[Table("tratamientos")]
[Index("MedicamentoId", Name = "MedicamentoId")]
[Index("VeterinarioId", Name = "VeterinarioId")]
[Index("Estado", Name = "idx_tratamientos_estado")]
[Index("FechaInicio", "FechaFin", Name = "idx_tratamientos_fechas")]
[Index("HistorialMedicoId", Name = "idx_tratamientos_historial")]
[MySqlCollation("utf8mb4_unicode_ci")]
public partial class Tratamiento
{
    [Key]
    public int Id { get; set; }

    [Column("historial_medico_id")]
    public int HistorialMedicoId { get; set; }

    [Column("medicamento_id")]
    public int MedicamentoId { get; set; }

    [Column("dosis")]
    [StringLength(100)]
    public string Dosis { get; set; } = null!;

    [Column("frecuencia")]
    [StringLength(100)]
    public string Frecuencia { get; set; } = null!;

    [Column("via_administracion")]
    [StringLength(50)]
    public string? ViaAdministracion { get; set; }

    [Column("fecha_inicio")]
    public DateOnly FechaInicio { get; set; }

    [Column("fecha_fin")]
    public DateOnly? FechaFin { get; set; }

    [Column("estado", TypeName = "enum('Activo','Completado','Suspendido')")]
    public string? Estado { get; set; }

    [Column("veterinario_id")]
    public int VeterinarioId { get; set; }

    [Column("observaciones", TypeName = "text")]
    public string? Observaciones { get; set; }

    [Column("fecha_creacion", TypeName = "datetime")]
    public DateTime? FechaCreacion { get; set; }

    [ForeignKey("HistorialMedicoId")]
    [InverseProperty("Tratamientos")]
    public virtual Historialmedico HistorialMedico { get; set; } = null!;

    [ForeignKey("MedicamentoId")]
    [InverseProperty("Tratamientos")]
    public virtual Medicamento Medicamento { get; set; } = null!;

    [ForeignKey("VeterinarioId")]
    [InverseProperty("Tratamientos")]
    public virtual Usuario Veterinario { get; set; } = null!;
}
