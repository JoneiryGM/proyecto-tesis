using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Api_Eden.Models;

[Table("tiposvacunas")]
[Index("EspecieId", Name = "idx_tipos_vacunas_especie")]
[Index("Nombre", "EspecieId", Name = "unique_vacuna_especie", IsUnique = true)]
[MySqlCollation("utf8mb4_unicode_ci")]
public partial class Tiposvacuna
{
    [Key]
    public int Id { get; set; }

    [Column("nombre")]
    [StringLength(150)]
    public string Nombre { get; set; } = null!;

    [Column("especie_id")]
    public int EspecieId { get; set; }

    [Column("descripcion", TypeName = "text")]
    public string? Descripcion { get; set; }

    [Column("edad_minima")]
    public int? EdadMinima { get; set; }

    [Column("duracion_meses")]
    public int? DuracionMeses { get; set; }

    [Column("obligatoria")]
    public bool? Obligatoria { get; set; }

    [Column("activa")]
    public bool? Activa { get; set; }

    [Column("fecha_creacion", TypeName = "datetime")]
    public DateTime? FechaCreacion { get; set; }

    [ForeignKey("EspecieId")]
    [InverseProperty("Tiposvacunas")]
    public virtual Especy Especie { get; set; } = null!;

    [InverseProperty("TipoVacuna")]
    public virtual ICollection<Vacuna> Vacunas { get; set; } = new List<Vacuna>();
}
