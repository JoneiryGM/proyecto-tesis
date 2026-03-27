using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Api_Eden.Models;

[Table("historialmovimientos")]
[Index("ZonaDestinoId", Name = "ZonaDestinoId")]
[Index("ZonaOrigenId", Name = "ZonaOrigenId")]
[Index("AnimalId", Name = "idx_movimientos_animal")]
[Index("Fecha", Name = "idx_movimientos_fecha")]
[Index("UsuarioResponsableId", Name = "idx_movimientos_usuario")]
[MySqlCollation("utf8mb4_unicode_ci")]
public partial class Historialmovimiento
{
    [Key]
    public int Id { get; set; }

    [Column("animal_id")]
    public int AnimalId { get; set; }

    [Column("zona_origen_id")]
    public int? ZonaOrigenId { get; set; }

    [Column("zona_destino_id")]
    public int? ZonaDestinoId { get; set; }

    [Column("motivo", TypeName = "text")]
    public string? Motivo { get; set; }

    [Column("fecha", TypeName = "datetime")]
    public DateTime? Fecha { get; set; }

    [Column("usuario_responsable_id")]
    public int UsuarioResponsableId { get; set; }

    [Column("observaciones", TypeName = "text")]
    public string? Observaciones { get; set; }

    [ForeignKey("AnimalId")]
    [InverseProperty("Historialmovimientos")]
    public virtual Animale Animal { get; set; } = null!;

    [ForeignKey("UsuarioResponsableId")]
    [InverseProperty("Historialmovimientos")]
    public virtual Usuario UsuarioResponsable { get; set; } = null!;

    [ForeignKey("ZonaDestinoId")]
    [InverseProperty("HistorialmovimientoZonaDestinos")]
    public virtual Zona? ZonaDestino { get; set; }

    [ForeignKey("ZonaOrigenId")]
    [InverseProperty("HistorialmovimientoZonaOrigens")]
    public virtual Zona? ZonaOrigen { get; set; }
}
