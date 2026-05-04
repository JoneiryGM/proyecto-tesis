using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Api_Eden.Models;

[Table("gastos")]
[Index("AlimentoId", Name = "AlimentoId")]
[Index("MedicamentoId", Name = "MedicamentoId")]
[Index("CategoriaGastoId", Name = "idx_gastos_categoria")]
[Index("FechaGasto", Name = "idx_gastos_fecha")]
[Index("UsuarioRegistroId", Name = "idx_gastos_usuario")]
public partial class Gasto
{
    [Key]
    public int Id { get; set; }

    public int CategoriaGastoId { get; set; }

    [StringLength(200)]
    public string Concepto { get; set; } = null!;

    [Precision(10, 2)]
    public decimal Monto { get; set; }

    public DateOnly FechaGasto { get; set; }

    [Column(TypeName = "enum('Efectivo','Transferencia','Tarjeta','Cheque')")]
    public string FormaPago { get; set; } = null!;

    [StringLength(100)]
    public string? NumeroFactura { get; set; }

    [StringLength(100)]
    public string? NumeroTransaccion { get; set; }

    [StringLength(200)]
    public string? NombreProveedor { get; set; }

    [StringLength(20)]
    public string? TelefonoProveedor { get; set; }

    public int? AlimentoId { get; set; }

    public int? MedicamentoId { get; set; }

    [StringLength(255)]
    public string? DocumentoAdjunto { get; set; }

    public int UsuarioRegistroId { get; set; }

    [Column(TypeName = "text")]
    public string? Observaciones { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaCreacion { get; set; }

    [ForeignKey("AlimentoId")]
    [InverseProperty("Gastos")]
    public virtual Alimento? Alimento { get; set; }

    [ForeignKey("CategoriaGastoId")]
    [InverseProperty("Gastos")]
    public virtual Categoriasgasto CategoriaGasto { get; set; } = null!;

    [ForeignKey("MedicamentoId")]
    [InverseProperty("Gastos")]
    public virtual Medicamento? Medicamento { get; set; }

    [ForeignKey("UsuarioRegistroId")]
    [InverseProperty("Gastos")]
    public virtual Usuario UsuarioRegistro { get; set; } = null!;
}
