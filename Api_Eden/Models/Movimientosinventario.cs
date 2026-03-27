using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Api_Eden.Models;

[Table("movimientosinventario")]
[Index("AlimentoId", Name = "idx_inventario_alimento")]
[Index("FechaMovimiento", Name = "idx_inventario_fecha")]
[Index("TipoMovimiento", Name = "idx_inventario_tipo")]
[Index("UsuarioResponsableId", Name = "idx_inventario_usuario")]
[MySqlCollation("utf8mb4_unicode_ci")]
public partial class Movimientosinventario
{
    [Key]
    public int Id { get; set; }

    [Column("alimento_id")]
    public int AlimentoId { get; set; }

    [Column("tipo_movimiento", TypeName = "enum('Entrada','Salida')")]
    public string TipoMovimiento { get; set; } = null!;

    [Column("cantidad")]
    [Precision(10, 2)]
    public decimal Cantidad { get; set; }

    [Column("motivo")]
    [StringLength(200)]
    public string? Motivo { get; set; }

    [Column("fecha_movimiento", TypeName = "datetime")]
    public DateTime? FechaMovimiento { get; set; }

    [Column("usuario_responsable_id")]
    public int UsuarioResponsableId { get; set; }

    [Column("observaciones", TypeName = "text")]
    public string? Observaciones { get; set; }

    [Column("costo_unitario")]
    [Precision(10, 2)]
    public decimal? CostoUnitario { get; set; }

    [ForeignKey("AlimentoId")]
    [InverseProperty("Movimientosinventarios")]
    public virtual Alimento Alimento { get; set; } = null!;

    [ForeignKey("UsuarioResponsableId")]
    [InverseProperty("Movimientosinventarios")]
    public virtual Usuario UsuarioResponsable { get; set; } = null!;
}
