using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Api_Eden.Models;

[Table("usuarios")]
[Index("Email", Name = "Email", IsUnique = true)]
[Index("Rol", Name = "idx_usuarios_rol")]
[MySqlCollation("utf8mb4_unicode_ci")]
public partial class Usuario
{
    [Key]
    public int Id { get; set; }

    [Column("nombre")]
    [StringLength(100)]
    public string Nombre { get; set; } = null!;

    [Column("apellido")]
    [StringLength(100)]
    public string Apellido { get; set; } = null!;

    [Column("email")]
    [StringLength(150)]
    public string Email { get; set; } = null!;

    [Column("password_hash")]
    [StringLength(255)]
    public string PasswordHash { get; set; } = null!;

    [Column("rol", TypeName = "enum('Administrador','Veterinario','Trabajador')")]
    public string Rol { get; set; } = null!;

    [Column("activo")]
    public bool? Activo { get; set; }

    [Column("fecha_creacion", TypeName = "datetime")]
    public DateTime? FechaCreacion { get; set; }

    [Column("fecha_ultima_modificacion", TypeName = "datetime")]
    public DateTime? FechaUltimaModificacion { get; set; }

    [InverseProperty("UsuarioResponsable")]
    public virtual ICollection<Adopcione> Adopciones { get; set; } = new List<Adopcione>();

    [InverseProperty("UsuarioRegistro")]
    public virtual ICollection<Animale> Animales { get; set; } = new List<Animale>();

    [InverseProperty("UsuarioCierre")]
    public virtual ICollection<Cierresmensuale> Cierresmensuales { get; set; } = new List<Cierresmensuale>();

    [InverseProperty("UsuarioRegistro")]
    public virtual ICollection<Donacione> Donaciones { get; set; } = new List<Donacione>();

    [InverseProperty("UsuarioResponsable")]
    public virtual ICollection<Estadosgenerale> Estadosgenerales { get; set; } = new List<Estadosgenerale>();

    [InverseProperty("UsuarioRegistro")]
    public virtual ICollection<Fallecimiento> FallecimientoUsuarioRegistros { get; set; } = new List<Fallecimiento>();

    [InverseProperty("Veterinario")]
    public virtual ICollection<Fallecimiento> FallecimientoVeterinarios { get; set; } = new List<Fallecimiento>();

    [InverseProperty("UsuarioRegistro")]
    public virtual ICollection<Gasto> Gastos { get; set; } = new List<Gasto>();

    [InverseProperty("Veterinario")]
    public virtual ICollection<Historialmedico> Historialmedicos { get; set; } = new List<Historialmedico>();

    [InverseProperty("UsuarioResponsable")]
    public virtual ICollection<Historialmovimiento> Historialmovimientos { get; set; } = new List<Historialmovimiento>();

    [InverseProperty("UsuarioResponsable")]
    public virtual ICollection<Movimientosinventario> Movimientosinventarios { get; set; } = new List<Movimientosinventario>();

    [InverseProperty("UsuarioCreo")]
    public virtual ICollection<Objetivo> Objetivos { get; set; } = new List<Objetivo>();

    [InverseProperty("UsuarioResponsable")]
    public virtual ICollection<Transferencia> Transferencia { get; set; } = new List<Transferencia>();

    [InverseProperty("Veterinario")]
    public virtual ICollection<Tratamiento> Tratamientos { get; set; } = new List<Tratamiento>();

    [InverseProperty("Veterinario")]
    public virtual ICollection<Vacuna> Vacunas { get; set; } = new List<Vacuna>();
}
