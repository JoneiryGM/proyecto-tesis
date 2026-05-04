using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Api_Eden.Models;

[Table("cierresmensuales")]
[Index("UsuarioCierreId", Name = "UsuarioCierreId")]
[Index("Año", "Mes", Name = "idx_cierres_periodo", IsUnique = true)]
public partial class Cierresmensuale
{
    [Key]
    public int Id { get; set; }

    public int Año { get; set; }

    public int Mes { get; set; }

    [Precision(10, 2)]
    public decimal TotalIngresos { get; set; }

    [Precision(10, 2)]
    public decimal TotalEgresos { get; set; }

    [Precision(10, 2)]
    public decimal Balance { get; set; }

    [Precision(10, 2)]
    public decimal? BalanceAnterior { get; set; }

    [Precision(10, 2)]
    public decimal? BalanceFinal { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime FechaCierre { get; set; }

    public int UsuarioCierreId { get; set; }

    [Column(TypeName = "text")]
    public string? Observaciones { get; set; }

    [ForeignKey("UsuarioCierreId")]
    [InverseProperty("Cierresmensuales")]
    public virtual Usuario UsuarioCierre { get; set; } = null!;
}
