using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Api_Eden.Models;

[Table("categoriasgasto")]
[Index("Nombre", Name = "Nombre", IsUnique = true)]
public partial class Categoriasgasto
{
    [Key]
    public int Id { get; set; }

    [StringLength(100)]
    public string Nombre { get; set; } = null!;

    [Column(TypeName = "text")]
    public string? Descripcion { get; set; }

    public bool? Activa { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaCreacion { get; set; }

    [InverseProperty("CategoriaGasto")]
    public virtual ICollection<Gasto> Gastos { get; set; } = new List<Gasto>();

    [InverseProperty("CategoriaGasto")]
    public virtual ICollection<Presupuestomensual> Presupuestomensuals { get; set; } = new List<Presupuestomensual>();
}
