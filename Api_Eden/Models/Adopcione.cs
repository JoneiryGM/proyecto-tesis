using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Api_Eden.Models;

[Table("adopciones")]
[Index("UsuarioResponsableId", Name = "UsuarioResponsableId")]
[Index("NombreAdoptante", Name = "idx_adopciones_adoptante")]
[Index("AnimalId", Name = "idx_adopciones_animal")]
[Index("EstadoAdopcion", Name = "idx_adopciones_estado")]
[Index("FechaAdopcion", Name = "idx_adopciones_fecha")]
[MySqlCollation("utf8mb4_unicode_ci")]
public partial class Adopcione
{
    [Key]
    public int Id { get; set; }

    public int AnimalId { get; set; }

    [Column("nombre_adoptante")]
    [StringLength(200)]
    public string NombreAdoptante { get; set; } = null!;

    [Column("telefono_adoptante")]
    [StringLength(20)]
    public string? TelefonoAdoptante { get; set; }

    [Column("email_adoptante")]
    [StringLength(150)]
    public string? EmailAdoptante { get; set; }

    [Column("direccion_adoptante", TypeName = "text")]
    public string? DireccionAdoptante { get; set; }

    [Column("documento_identidad")]
    [StringLength(50)]
    public string? DocumentoIdentidad { get; set; }

    [Column("fecha_adopcion")]
    public DateOnly FechaAdopcion { get; set; }

    [Column("fecha_seguimiento")]
    public DateOnly? FechaSeguimiento { get; set; }

    [Column("estado_adopcion", TypeName = "enum('Pendiente','Aprobada','Rechazada','Devuelto')")]
    public string? EstadoAdopcion { get; set; }

    [Column("usuario_responsable_id")]
    public int UsuarioResponsableId { get; set; }

    [Column("observaciones", TypeName = "text")]
    public string? Observaciones { get; set; }

    [Column("fecha_creacion", TypeName = "datetime")]
    public DateTime? FechaCreacion { get; set; }

    [ForeignKey("AnimalId")]
    [InverseProperty("Adopciones")]
    public virtual Animale Animal { get; set; } = null!;

    [ForeignKey("UsuarioResponsableId")]
    [InverseProperty("Adopciones")]
    public virtual Usuario UsuarioResponsable { get; set; } = null!;
}
