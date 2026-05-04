using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Api_Eden.Models;

[Table("donantes")]
[Index("Nombre", Name = "idx_donantes_nombre")]
[Index("EsRecurrente", Name = "idx_donantes_recurrente")]
[Index("TipoDonante", Name = "idx_donantes_tipo")]
public partial class Donante
{
    [Key]
    public int Id { get; set; }

    [Column(TypeName = "enum('Persona','Empresa')")]
    public string TipoDonante { get; set; } = null!;

    [StringLength(200)]
    public string Nombre { get; set; } = null!;

    [StringLength(50)]
    public string? DocumentoIdentidad { get; set; }

    [Column("RNC")]
    [StringLength(50)]
    public string? Rnc { get; set; }

    [StringLength(20)]
    public string? Telefono { get; set; }

    [StringLength(150)]
    public string? Email { get; set; }

    [Column(TypeName = "text")]
    public string? Direccion { get; set; }

    public bool? EsRecurrente { get; set; }

    [StringLength(50)]
    public string? FrecuenciaDonacion { get; set; }

    public DateOnly? FechaRegistro { get; set; }

    [Column(TypeName = "text")]
    public string? Observaciones { get; set; }

    public bool? Activo { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaCreacion { get; set; }

    [InverseProperty("Donante")]
    public virtual ICollection<Donacione> Donaciones { get; set; } = new List<Donacione>();
}
