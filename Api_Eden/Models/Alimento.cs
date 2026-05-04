using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Api_Eden.Models;

[Table("alimentos")]
[Index("CantidadDisponible", Name = "idx_alimentos_stock")]
[Index("TipoAnimal", Name = "idx_alimentos_tipo")]
[Index("FechaVencimiento", Name = "idx_alimentos_vencimiento")]
[MySqlCollation("utf8mb4_unicode_ci")]
public partial class Alimento
{
    [Key]
    public int Id { get; set; }

    [Column("nombre")]
    [StringLength(150)]
    public string Nombre { get; set; } = null!;

    [Column("tipo_animal", TypeName = "enum('Perro','Gato','Ave','Otro')")]
    public string TipoAnimal { get; set; } = null!;

    [Column("marca")]
    [StringLength(100)]
    public string? Marca { get; set; }

    [Column("unidad_medida", TypeName = "enum('Kg','Lb','Unidad','Bolsa')")]
    public string UnidadMedida { get; set; } = null!;

    [Column("cantidad_disponible")]
    [Precision(10, 2)]
    public decimal CantidadDisponible { get; set; }

    [Column("stock_minimo")]
    [Precision(10, 2)]
    public decimal StockMinimo { get; set; }

    [Column("fecha_vencimiento")]
    public DateOnly? FechaVencimiento { get; set; }

    [Column("activo")]
    public bool? Activo { get; set; }

    [Column("fecha_creacion", TypeName = "datetime")]
    public DateTime? FechaCreacion { get; set; }

    [InverseProperty("Alimento")]
    public virtual ICollection<Donacione> Donaciones { get; set; } = new List<Donacione>();

    [InverseProperty("Alimento")]
    public virtual ICollection<Gasto> Gastos { get; set; } = new List<Gasto>();

    [InverseProperty("Alimento")]
    public virtual ICollection<Movimientosinventario> Movimientosinventarios { get; set; } = new List<Movimientosinventario>();
}
