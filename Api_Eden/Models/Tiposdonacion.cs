using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Api_Eden.Models;

[Table("tiposdonacion")]
[Index("Nombre", Name = "Nombre", IsUnique = true)]
public partial class Tiposdonacion
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

    [InverseProperty("TipoDonacion")]
    public virtual ICollection<Donacione> Donaciones { get; set; } = new List<Donacione>();
}
