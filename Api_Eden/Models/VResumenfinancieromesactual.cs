using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Api_Eden.Models;

[Keyless]
public partial class VResumenfinancieromesactual
{
    [StringLength(10)]
    public string Periodo { get; set; } = null!;

    [Precision(32, 2)]
    public decimal? TotalIngresos { get; set; }

    [Precision(32, 2)]
    public decimal? TotalEgresos { get; set; }

    [Precision(33, 2)]
    public decimal? Balance { get; set; }
}
