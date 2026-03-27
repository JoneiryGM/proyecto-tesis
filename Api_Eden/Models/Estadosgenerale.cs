using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Api_Eden.Models;

[Table("estadosgenerales")]
[Index("UsuarioResponsableId", Name = "UsuarioResponsableId")]
[Index("AnimalId", Name = "idx_estados_animal")]
[Index("EstadoNuevo", Name = "idx_estados_estado")]
[Index("FechaCambio", Name = "idx_estados_fecha")]
[MySqlCollation("utf8mb4_unicode_ci")]
public partial class Estadosgenerale
{
    [Key]
    public int Id { get; set; }

    [Column("animal_id")]
    public int AnimalId { get; set; }

    [Column("estado_anterior", TypeName = "enum('Activo','Adoptado','Fallecido','Transferido')")]
    public string? EstadoAnterior { get; set; }

    [Column("estado_nuevo", TypeName = "enum('Activo','Adoptado','Fallecido','Transferido')")]
    public string EstadoNuevo { get; set; } = null!;

    [Column("fecha_cambio", TypeName = "datetime")]
    public DateTime? FechaCambio { get; set; }

    [Column("motivo", TypeName = "text")]
    public string? Motivo { get; set; }

    [Column("observaciones", TypeName = "text")]
    public string? Observaciones { get; set; }

    [Column("usuario_responsable_id")]
    public int UsuarioResponsableId { get; set; }

    [Column("lugar_transferencia")]
    [StringLength(200)]
    public string? LugarTransferencia { get; set; }

    [Column("causa_fallecimiento", TypeName = "text")]
    public string? CausaFallecimiento { get; set; }

    [ForeignKey("AnimalId")]
    [InverseProperty("Estadosgenerales")]
    public virtual Animale Animal { get; set; } = null!;

    [ForeignKey("UsuarioResponsableId")]
    [InverseProperty("Estadosgenerales")]
    public virtual Usuario UsuarioResponsable { get; set; } = null!;
}
