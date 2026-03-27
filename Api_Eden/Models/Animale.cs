using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Api_Eden.Models;

[Table("animales")]
[Index("UsuarioRegistroId", Name = "UsuarioRegistroId")]
[Index("EspecieId", Name = "id_animales_especie")]
[Index("EstadoGeneral", Name = "idx_animales_estado")]
[Index("Nombre", Name = "idx_animales_nombre")]
[Index("ZonaActualId", Name = "idx_animales_zona")]
[MySqlCollation("utf8mb4_unicode_ci")]
public partial class Animale
{
    [Key]
    public int Id { get; set; }

    [Column("nombre")]
    [StringLength(100)]
    public string Nombre { get; set; } = null!;

    [Column("especie_id")]
    public int EspecieId { get; set; }

    [Column("raza")]
    [StringLength(100)]
    public string? Raza { get; set; }

    [Column("edad")]
    public int? Edad { get; set; }

    [Column("sexo", TypeName = "enum('Macho','Hembra','Desconocido')")]
    public string? Sexo { get; set; }

    [Column("color")]
    [StringLength(50)]
    public string? Color { get; set; }

    [Column("fecha_ingreso")]
    public DateOnly FechaIngreso { get; set; }

    [Column("zona_actual_id")]
    public int? ZonaActualId { get; set; }

    [Column("estado_salud", TypeName = "enum('Saludable','EnTratamiento','Critico','Recuperado')")]
    public string? EstadoSalud { get; set; }

    [Column("estado_general", TypeName = "enum('Activo','Adoptado','Fallecido','Transferido')")]
    public string? EstadoGeneral { get; set; }

    [Column("fotografia_url")]
    [StringLength(255)]
    public string? FotografiaUrl { get; set; }

    [Column("observaciones", TypeName = "text")]
    public string? Observaciones { get; set; }

    [Column("usuario_registro_id")]
    public int? UsuarioRegistroId { get; set; }

    [Column("fecha_creacion", TypeName = "datetime")]
    public DateTime? FechaCreacion { get; set; }

    [Column("fecha_ultima_modificacion", TypeName = "datetime")]
    public DateTime? FechaUltimaModificacion { get; set; }

    [Column("fecha_fallecimiento")]
    public DateOnly? FechaFallecimiento { get; set; }

    [Column("fecha_adopcion")]
    public DateOnly? FechaAdopcion { get; set; }

    [Column("fecha_transferencia")]
    public DateOnly? FechaTransferencia { get; set; }

    [Column("lugar_transferencia")]
    [StringLength(200)]
    public string? LugarTransferencia { get; set; }

    [InverseProperty("Animal")]
    public virtual ICollection<Adopcione> Adopciones { get; set; } = new List<Adopcione>();

    [ForeignKey("EspecieId")]
    [InverseProperty("Animales")]
    public virtual Especy Especie { get; set; } = null!;

    [InverseProperty("Animal")]
    public virtual ICollection<Estadosgenerale> Estadosgenerales { get; set; } = new List<Estadosgenerale>();

    [InverseProperty("Animal")]
    public virtual ICollection<Fallecimiento> Fallecimientos { get; set; } = new List<Fallecimiento>();

    [InverseProperty("Animal")]
    public virtual ICollection<Historialmedico> Historialmedicos { get; set; } = new List<Historialmedico>();

    [InverseProperty("Animal")]
    public virtual ICollection<Historialmovimiento> Historialmovimientos { get; set; } = new List<Historialmovimiento>();

    [InverseProperty("Animal")]
    public virtual ICollection<Transferencia> Transferencia { get; set; } = new List<Transferencia>();

    [InverseProperty("Animal")]
    public virtual ICollection<Tratamiento> Tratamientos { get; set; } = new List<Tratamiento>();

    [ForeignKey("UsuarioRegistroId")]
    [InverseProperty("Animales")]
    public virtual Usuario? UsuarioRegistro { get; set; }

    [InverseProperty("Animal")]
    public virtual ICollection<Vacuna> Vacunas { get; set; } = new List<Vacuna>();

    [ForeignKey("ZonaActualId")]
    [InverseProperty("Animales")]
    public virtual Zona? ZonaActual { get; set; }
}
