using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Api_Eden.Models;

[Table("fallecimientos")]
[Index("UsuarioRegistroId", Name = "UsuarioRegistroId")]
[Index("VeterinarioId", Name = "VeterinarioId")]
[Index("AnimalId", Name = "idx_fallecimientos_animal")]
[Index("Fecha", Name = "idx_fallecimientos_fecha")]
[MySqlCollation("utf8mb4_unicode_ci")]
public partial class Fallecimiento
{
    [Key]
    public int Id { get; set; }

    [Column("animal_id")]
    public int AnimalId { get; set; }

    [Column("fecha")]
    public DateOnly Fecha { get; set; }

    [Column("hora", TypeName = "time")]
    public TimeOnly? Hora { get; set; }

    [Column("causa", TypeName = "text")]
    public string Causa { get; set; } = null!;

    [Column("lugar")]
    [StringLength(200)]
    public string? Lugar { get; set; }

    [Column("veterinario_id")]
    public int? VeterinarioId { get; set; }

    [Column("observaciones", TypeName = "text")]
    public string? Observaciones { get; set; }

    [Column("documento_adjunto")]
    [StringLength(255)]
    public string? DocumentoAdjunto { get; set; }

    [Column("usuario_registro_id")]
    public int UsuarioRegistroId { get; set; }

    [Column("fecha_creacion", TypeName = "datetime")]
    public DateTime? FechaCreacion { get; set; }

    [ForeignKey("AnimalId")]
    [InverseProperty("Fallecimientos")]
    public virtual Animale Animal { get; set; } = null!;

    [ForeignKey("UsuarioRegistroId")]
    [InverseProperty("FallecimientoUsuarioRegistros")]
    public virtual Usuario UsuarioRegistro { get; set; } = null!;

    [ForeignKey("VeterinarioId")]
    [InverseProperty("FallecimientoVeterinarios")]
    public virtual Usuario? Veterinario { get; set; }
}
