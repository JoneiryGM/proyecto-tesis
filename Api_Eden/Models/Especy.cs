using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Api_Eden.Models;

[Table("especies")]
[Index("Nombre", Name = "idx_especies_nombre", IsUnique = true)]
[MySqlCollation("utf8mb4_unicode_ci")]
public partial class Especy
{
    [Key]
    public int Id { get; set; }

    [Column("nombre")]
    [StringLength(50)]
    public string Nombre { get; set; } = null!;

    [Column("descripcion", TypeName = "text")]
    public string? Descripcion { get; set; }

    [Column("cuidados_especiales", TypeName = "text")]
    public string? CuidadosEspeciales { get; set; }

    [Column("activa")]
    public bool? Activa { get; set; }

    [Column("fecha_creacion", TypeName = "datetime")]
    public DateTime? FechaCreacion { get; set; }

    [InverseProperty("Especie")]
    public virtual ICollection<Animale> Animales { get; set; } = new List<Animale>();

    [InverseProperty("Especie")]
    public virtual ICollection<Tiposvacuna> Tiposvacunas { get; set; } = new List<Tiposvacuna>();
}
