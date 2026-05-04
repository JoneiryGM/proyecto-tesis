using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Api_Eden.Models;

[Table("zonas")]
[Index("Activa", Name = "idx_zonas_activa")]
[MySqlCollation("utf8mb4_unicode_ci")]
public partial class Zona
{
    [Key]
    public int Id { get; set; }

    [Column("nombre")]
    [StringLength(100)]
    public string Nombre { get; set; } = null!;

    [Column("descripcion", TypeName = "text")]
    public string? Descripcion { get; set; }

    [Column("capacidad_maxima")]
    public int CapacidadMaxima { get; set; }

    [Column("cantidad_actual")]
    public int? CantidadActual { get; set; }

    [Column("activa")]
    public bool? Activa { get; set; }

    [Column("fecha_creacion", TypeName = "datetime")]
    public DateTime? FechaCreacion { get; set; }

    [InverseProperty("ZonaActual")]
    public virtual ICollection<Animale> Animales { get; set; } = new List<Animale>();

    [InverseProperty("ZonaDestino")]
    public virtual ICollection<Historialmovimiento> HistorialmovimientoZonaDestinos { get; set; } = new List<Historialmovimiento>();

    [InverseProperty("ZonaOrigen")]
    public virtual ICollection<Historialmovimiento> HistorialmovimientoZonaOrigens { get; set; } = new List<Historialmovimiento>();
}
