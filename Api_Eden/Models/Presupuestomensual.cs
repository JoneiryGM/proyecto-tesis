using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Api_Eden.Models;

[Table("presupuestomensual")]
[Index("CategoriaGastoId", Name = "idx_presupuesto_categoria")]
[Index("Año", "Mes", Name = "idx_presupuesto_periodo")]
[Index("Año", "Mes", "CategoriaGastoId", Name = "unique_presupuesto", IsUnique = true)]
public partial class Presupuestomensual
{
    [Key]
    public int Id { get; set; }

    public int Año { get; set; }

    public int Mes { get; set; }

    public int CategoriaGastoId { get; set; }

    [Precision(10, 2)]
    public decimal MontoPresupuestado { get; set; }

    [Precision(10, 2)]
    public decimal? MontoEjecutado { get; set; }

    [Column(TypeName = "text")]
    public string? Observaciones { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaCreacion { get; set; }

    [ForeignKey("CategoriaGastoId")]
    [InverseProperty("Presupuestomensuals")]
    public virtual Categoriasgasto CategoriaGasto { get; set; } = null!;
}
