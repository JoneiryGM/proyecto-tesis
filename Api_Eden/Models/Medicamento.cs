using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Api_Eden.Models;

[Table("medicamentos")]
[Index("Nombre", Name = "idx_medicamentos_nombre", IsUnique = true)]
[MySqlCollation("utf8mb4_unicode_ci")]
public partial class Medicamento
{
    [Key]
    public int Id { get; set; }

    [Column("nombre")]
    [StringLength(200)]
    public string Nombre { get; set; } = null!;

    [Column("principio_activo")]
    [StringLength(200)]
    public string? PrincipioActivo { get; set; }

    [Column("presentacion")]
    [StringLength(100)]
    public string? Presentacion { get; set; }

    [Column("concentracion")]
    [StringLength(50)]
    public string? Concentracion { get; set; }

    [Column("fabricante")]
    [StringLength(100)]
    public string? Fabricante { get; set; }

    [Column("indicaciones", TypeName = "text")]
    public string? Indicaciones { get; set; }

    [Column("contraindicaciones", TypeName = "text")]
    public string? Contraindicaciones { get; set; }

    [Column("efectos_secundarios", TypeName = "text")]
    public string? EfectosSecundarios { get; set; }

    [Column("requiere_receta")]
    public bool? RequiereReceta { get; set; }

    [Column("activo")]
    public bool? Activo { get; set; }

    [Column("fecha_creacion", TypeName = "datetime")]
    public DateTime? FechaCreacion { get; set; }

    [InverseProperty("Medicamento")]
    public virtual ICollection<Tratamiento> Tratamientos { get; set; } = new List<Tratamiento>();
}
