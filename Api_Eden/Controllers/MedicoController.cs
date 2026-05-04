using Api_Eden.Data;
using Api_Eden.DTOs.MedicoDto;
using Api_Eden.Models;
using Api_Eden.Services.TratamientoService.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Api_Eden.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MedicoController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly ITratamientoService _tratamientoService;

        public MedicoController(AppDbContext db, ITratamientoService tratamientoService)
        {
            _db = db;
            _tratamientoService = tratamientoService;
        }

        // ── GET: Lista todos los tratamientos para la vista principal ─────────
        [Authorize]
        [HttpGet("tratamientos")]
        public async Task<IActionResult> GetTratamientos()
        {
            try
            {
                var data = await _db.Tratamientos
                    .Include(t => t.Medicamento)
                    .Include(t => t.HistorialMedico)
                        .ThenInclude(h => h.Animal)
                            .ThenInclude(a => a.Especie)
                    .OrderByDescending(t => t.FechaInicio)
                    .Select(t => new
                    {
                        t.Id,
                        AnimalId = t.HistorialMedico.AnimalId,
                        Animal = t.HistorialMedico.Animal.Nombre,
                        FotografiaUrl = t.HistorialMedico.Animal.FotografiaUrl,
                        Especie = t.HistorialMedico.Animal.Especie != null
                                             ? t.HistorialMedico.Animal.Especie.Nombre : null,
                        Diagnostico = t.HistorialMedico.Diagnostico,
                        Medicamento = t.Medicamento.Nombre,
                        t.Dosis,
                        t.Frecuencia,
                        t.ViaAdministracion,
                        t.Estado,
                        FechaInicio = t.FechaInicio.ToString("yyyy-MM-dd"),
                        FechaFin = t.FechaFin != null
                                             ? t.FechaFin.Value.ToString("yyyy-MM-dd") : null,
                        HistorialMedicoId = t.HistorialMedicoId,
                    })
                    .ToListAsync();

                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al obtener tratamientos", error = ex.Message });
            }
        }

        // ── GET: Historial médico de un animal ────────────────────────────────
        [Authorize]
        [HttpGet("historial/{animalId}")]
        public async Task<IActionResult> GetHistorial(int animalId)
        {
            try
            {
                var animal = await _db.Animales.FindAsync(animalId);
                if (animal is null)
                    return NotFound(new { mensaje = "Animal no encontrado." });

                var historial = await _db.Historialmedicos
                    .Where(h => h.AnimalId == animalId)
                    .Include(h => h.Tratamientos)
                        .ThenInclude(t => t.Medicamento)
                    .OrderByDescending(h => h.Fecha)
                    .Select(h => new
                    {
                        h.Id,
                        h.AnimalId,
                        h.Fecha,
                        h.Diagnostico,
                        h.Sintomas,
                        h.Peso,
                        h.Temperatura,
                        h.Observaciones,
                        Tratamientos = h.Tratamientos.Select(t => new
                        {
                            t.Id,
                            Medicamento = t.Medicamento.Nombre,
                            t.Dosis,
                            t.Frecuencia,
                            t.ViaAdministracion,
                            t.Estado,
                            t.FechaInicio,
                            t.FechaFin
                        })
                    })
                    .ToListAsync();

                if (historial == null || !historial.Any())
                    return NotFound(new { mensaje = "El animal no tiene historial médico." });

                return Ok(historial);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al obtener historial", error = ex.Message });
            }
        }

        // ── POST: Registrar consulta/historial ────────────────────────────────
        // FIX: acepta Administrador además de Veterinario
        [Authorize(Roles = "Administrador,Veterinario")]
        [HttpPost("historial")]
        public async Task<IActionResult> RegistrarHistorial([FromBody] RegistrarHistorialDto dto)
        {
            try
            {
                var animal = await _db.Animales.FindAsync(dto.AnimalId);
                if (animal is null)
                    return NotFound(new { mensaje = "Animal no encontrado." });

                // FIX: acepta Administrador o Veterinario como responsable
                var responsable = await _db.Usuarios.FindAsync(dto.VeterinarioId);
                if (responsable is null ||
                    (responsable.Rol != "Veterinario" && responsable.Rol != "Administrador"))
                    return BadRequest(new { mensaje = "El usuario responsable no existe o no tiene permisos." });

                var historial = new Historialmedico
                {
                    AnimalId = dto.AnimalId,
                    Diagnostico = dto.Diagnostico,
                    Sintomas = dto.Sintomas,
                    Peso = dto.Peso,
                    Temperatura = dto.Temperatura,
                    VeterinarioId = dto.VeterinarioId,
                    Observaciones = dto.Observaciones,
                    Fecha = DateTime.UtcNow,
                    FechaCreacion = DateTime.UtcNow
                };

                _db.Historialmedicos.Add(historial);
                await _db.SaveChangesAsync();

                return Ok(new { mensaje = "Historial registrado correctamente.", id = historial.Id });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al registrar historial", error = ex.Message });
            }
        }

        // ── POST: Registrar tratamiento ───────────────────────────────────────
        [Authorize(Roles = "Administrador,Veterinario")]
        [HttpPost("tratamiento")]
        public async Task<IActionResult> RegistrarTratamiento([FromBody] RegistrarTratamientoDto dto)
        {
            var (ok, mensaje, id) = await _tratamientoService.RegistrarTratamiento(dto);
            if (!ok) return BadRequest(new { mensaje });
            return Ok(new { mensaje, id });
        }

        // ── PUT: Actualizar estado de tratamiento ─────────────────────────────
        [Authorize(Roles = "Administrador,Veterinario")]
        [HttpPut("tratamiento/{id}/estado")]
        public async Task<IActionResult> ActualizarEstadoTratamiento(
            int id, [FromBody] ActualizarEstadoTratamientoDto dto)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId))
                return Unauthorized(new { mensaje = "No se pudo identificar al usuario." });

            var (ok, mensaje) = await _tratamientoService
                .ActualizarEstadoTratamiento(id, dto.Estado, userId);

            if (!ok) return BadRequest(new { mensaje });
            return Ok(new { mensaje });
        }

        // ── GET: Catálogo de medicamentos ─────────────────────────────────────
        [Authorize]
        [HttpGet("medicamentos")]
        public async Task<IActionResult> GetMedicamentos()
        {
            try
            {
                var data = await _db.Medicamentos
                    .Where(m => m.Activo == true)
                    .Select(m => new
                    {
                        m.Id,
                        m.Nombre,
                        m.PrincipioActivo,
                        m.Presentacion,
                        m.Concentracion
                    })
                    .OrderBy(m => m.Nombre)
                    .ToListAsync();

                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al obtener medicamentos", error = ex.Message });
            }
        }

        // ── POST: Crear medicamento nuevo desde el formulario ─────────────────
        [Authorize(Roles = "Administrador,Veterinario")]
        [HttpPost("medicamentos")]
        public async Task<IActionResult> CrearMedicamento([FromBody] CrearMedicamentoDto dto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dto.Nombre))
                    return BadRequest(new { mensaje = "El nombre del medicamento es obligatorio." });

                // Si ya existe, devolver el existente sin duplicar
                var existente = await _db.Medicamentos
                    .FirstOrDefaultAsync(m => m.Nombre.ToLower() == dto.Nombre.ToLower().Trim());

                if (existente != null)
                    return Ok(new { id = existente.Id, nombre = existente.Nombre });

                var nuevo = new Medicamento
                {
                    Nombre = dto.Nombre.Trim(),
                    Activo = true,
                    FechaCreacion = DateTime.UtcNow,
                };

                _db.Medicamentos.Add(nuevo);
                await _db.SaveChangesAsync();

                return Ok(new { id = nuevo.Id, nombre = nuevo.Nombre });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al crear medicamento", error = ex.Message });
            }
        }

        // ── GET: Tipos de vacuna ──────────────────────────────────────────────
        [Authorize]
        [HttpGet("tipos-vacuna")]
        public async Task<IActionResult> GetTiposVacuna()
        {
            try
            {
                var data = await _db.Tiposvacunas
                    .Where(t => t.Activa == true)
                    .Select(t => new
                    {
                        t.Id,
                        t.Nombre,
                        t.EspecieId,
                        t.Descripcion,
                        t.DuracionMeses,
                        t.Obligatoria
                    })
                    .OrderBy(t => t.Nombre)
                    .ToListAsync();

                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al obtener tipos de vacuna", error = ex.Message });
            }
        }

        // ── GET: Veterinarios y administradores activos ───────────────────────
        [Authorize]
        [HttpGet("veterinarios")]
        public async Task<IActionResult> GetVeterinarios()
        {
            try
            {
                var data = await _db.Usuarios
                    .Where(u => (u.Rol == "Veterinario" || u.Rol == "Administrador")
                                && u.Activo == true)
                    .Select(u => new { u.Id, u.Nombre, u.Apellido, u.Rol })
                    .OrderBy(u => u.Nombre)
                    .ToListAsync();

                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al obtener veterinarios", error = ex.Message });
            }
        }



        [Authorize]
        [HttpGet("tratamientosall")]
        public async Task<IActionResult> GetAllTratamientos()
        {
            try
            {
                var data = await _db.Tratamientos
                    .Include(t => t.Medicamento)
                    .Include(t => t.HistorialMedico)
                        .ThenInclude(h => h.Animal)
                            .ThenInclude(a => a.Especie)
                    .OrderByDescending(t => t.FechaInicio)
                    .Select(t => new
                    {
                        t.Id,
                        AnimalId = t.HistorialMedico.AnimalId,
                        Animal = t.HistorialMedico.Animal.Nombre,
                        FotografiaUrl = t.HistorialMedico.Animal.FotografiaUrl,
                        Especie = t.HistorialMedico.Animal.Especie != null
                                              ? t.HistorialMedico.Animal.Especie.Nombre : null,
                        Diagnostico = t.HistorialMedico.Diagnostico,
                        Medicamento = t.Medicamento.Nombre,
                        t.Dosis,
                        t.Frecuencia,
                        t.ViaAdministracion,
                        t.Estado,
                        FechaInicio = t.FechaInicio.ToString("yyyy-MM-dd"),
                        FechaFin = t.FechaFin != null
                                              ? t.FechaFin.Value.ToString("yyyy-MM-dd") : null,
                        HistorialMedicoId = t.HistorialMedicoId,
                    })
                    .ToListAsync();

                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al obtener tratamientos", error = ex.Message });
            }
        }
    }
}