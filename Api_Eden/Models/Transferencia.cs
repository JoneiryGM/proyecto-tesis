using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Api_Eden.Models;

[Table("transferencias")]
[Index("UsuarioResponsableId", Name = "UsuarioResponsableId")]
[Index("AnimalId", Name = "idx_transferencias_animal")]
[Index("FechaTransferencia", Name = "idx_transferencias_fecha")]
[MySqlCollation("utf8mb4_unicode_ci")]
public partial class Transferencia
{
    [Key]
    public int Id { get; set; }

    [Column("animal_id")]
    public int AnimalId { get; set; }

    [Column("fecha_transferencia")]
    public DateOnly FechaTransferencia { get; set; }

    [Column("lugar_destino")]
    [StringLength(200)]
    public string LugarDestino { get; set; } = null!;

    [Column("direccion_destino", TypeName = "text")]
    public string? DireccionDestino { get; set; }

    [Column("contacto_destino")]
    [StringLength(200)]
    public string? ContactoDestino { get; set; }

    [Column("telefono_destino")]
    [StringLength(20)]
    public string? TelefonoDestino { get; set; }

    [Column("email_destino")]
    [StringLength(150)]
    public string? EmailDestino { get; set; }

    [Column("motivo_transferencia", TypeName = "text")]
    public string MotivoTransferencia { get; set; } = null!;

    [Column("documento_transferencia")]
    [StringLength(255)]
    public string? DocumentoTransferencia { get; set; }

    [Column("usuario_responsable_id")]
    public int UsuarioResponsableId { get; set; }

    [Column("observaciones", TypeName = "text")]
    public string? Observaciones { get; set; }

    [Column("fecha_creacion", TypeName = "datetime")]
    public DateTime? FechaCreacion { get; set; }

    [ForeignKey("AnimalId")]
    [InverseProperty("Transferencia")]
    public virtual Animale Animal { get; set; } = null!;

    [ForeignKey("UsuarioResponsableId")]
    [InverseProperty("Transferencia")]
    public virtual Usuario UsuarioResponsable { get; set; } = null!;
}
