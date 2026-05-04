using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Api_Eden.Models;

[Table("objetivos")]
[Index("UsuarioCreoId", Name = "UsuarioCreoId")]
[MySqlCollation("utf8mb4_unicode_ci")]
public partial class Objetivo
{
    [Key]
    public int Id { get; set; }

    [StringLength(200)]
    public string Nombre { get; set; } = null!;

    [Column(TypeName = "text")]
    public string? Descripcion { get; set; }

    [Precision(12, 2)]
    public decimal MontoObjetivo { get; set; }

    [Precision(12, 2)]
    public decimal MontoRecaudado { get; set; }

    [Column(TypeName = "enum('Activo','Completado','Pausado')")]
    public string Estado { get; set; } = null!;

    public DateOnly FechaInicio { get; set; }

    public DateOnly? FechaLimite { get; set; }

    public int UsuarioCreoId { get; set; }

    [Column(TypeName = "text")]
    public string? Observaciones { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaCreacion { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaActualizacion { get; set; }

    [InverseProperty("Objetivo")]
    public virtual ICollection<Donacione> Donaciones { get; set; } = new List<Donacione>();

    [ForeignKey("UsuarioCreoId")]
    [InverseProperty("Objetivos")]
    public virtual Usuario UsuarioCreo { get; set; } = null!;
}
