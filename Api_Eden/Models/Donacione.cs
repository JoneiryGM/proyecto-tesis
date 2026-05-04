using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Api_Eden.Models;

[Table("donaciones")]
[Index("AlimentoId", Name = "AlimentoId")]
[Index("MedicamentoId", Name = "MedicamentoId")]
[Index("UsuarioRegistroId", Name = "UsuarioRegistroId")]
[Index("ObjetivoId", Name = "fk_donacion_objetivo")]
[Index("DonanteId", Name = "idx_donaciones_donante")]
[Index("FechaDonacion", Name = "idx_donaciones_fecha")]
[Index("TipoDonacionId", Name = "idx_donaciones_tipo")]
public partial class Donacione
{
    [Key]
    public int Id { get; set; }

    public int? DonanteId { get; set; }

    public int TipoDonacionId { get; set; }

    [Precision(10, 2)]
    public decimal? MontoDinero { get; set; }

    [Column(TypeName = "text")]
    public string? DescripcionDonacion { get; set; }

    public int? CantidadArticulos { get; set; }

    [Precision(10, 2)]
    public decimal? ValorEstimado { get; set; }

    public int? AlimentoId { get; set; }

    public int? MedicamentoId { get; set; }

    public DateOnly FechaDonacion { get; set; }

    [Column(TypeName = "enum('Efectivo','Transferencia','Tarjeta','Cheque','Especie')")]
    public string? FormaPago { get; set; }

    [StringLength(100)]
    public string? NumeroTransaccion { get; set; }

    public bool? RequiereCertificado { get; set; }

    public bool? CertificadoGenerado { get; set; }

    public DateOnly? FechaCertificado { get; set; }

    [StringLength(255)]
    public string? DocumentoAdjunto { get; set; }

    public int UsuarioRegistroId { get; set; }

    [Column(TypeName = "text")]
    public string? Observaciones { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaCreacion { get; set; }

    public int? ObjetivoId { get; set; }

    [ForeignKey("AlimentoId")]
    [InverseProperty("Donaciones")]
    public virtual Alimento? Alimento { get; set; }

    [ForeignKey("DonanteId")]
    [InverseProperty("Donaciones")]
    public virtual Donante? Donante { get; set; }

    [ForeignKey("MedicamentoId")]
    [InverseProperty("Donaciones")]
    public virtual Medicamento? Medicamento { get; set; }

    [ForeignKey("ObjetivoId")]
    [InverseProperty("Donaciones")]
    public virtual Objetivo? Objetivo { get; set; }

    [ForeignKey("TipoDonacionId")]
    [InverseProperty("Donaciones")]
    public virtual Tiposdonacion TipoDonacion { get; set; } = null!;

    [ForeignKey("UsuarioRegistroId")]
    [InverseProperty("Donaciones")]
    public virtual Usuario UsuarioRegistro { get; set; } = null!;
}
