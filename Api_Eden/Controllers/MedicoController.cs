using Api_Eden.Data;
using Api_Eden.DTOs.MedicoDto;
using Api_Eden.Models;
using Api_Eden.Services.TratamientoService.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using System.Data;
using System.Security.Claims;

namespace Api_Eden.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MedicoController : ControllerBase
    {
        public static class Roles
{
    public const string Admin = "Administrador";
    public const string Veterinario = "Veterinario";
            
        }


        private readonly AppDbContext _db;
        private readonly ITratamientoService _tratamientoService;
        public MedicoController(AppDbContext db, ITratamientoService tratamientoService)
    {
        _db = db;
        _tratamientoService = tratamientoService;
    }


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
                {
                    return NotFound(new { mensaje = "El animal no tiene historial médico." });
                }

                return Ok(historial);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al obtener historial", error = ex.Message });
            }
        }

        // POST: api/medico/historial  **REGISTRAR UN NUEVO HISTORIAL MÉDICO PARA UN ANIMAL**
        [Authorize(Roles = "Administrador,Veterinario")]
        [HttpPost("historial")]
        public async Task<IActionResult> RegistrarHistorial([FromBody] RegistrarHistorialDto dto)
        {
            try
            {
                var animal = await _db.Animales.FindAsync(dto.AnimalId);
                if (animal is null)
                    return NotFound(new { mensaje = "Animal no encontrado." });

                var veterinario = await _db.Usuarios.FindAsync(dto.VeterinarioId);
                if (veterinario is null || veterinario.Rol != "Veterinario")
                    return BadRequest(new { mensaje = "El veterinario especificado no existe o no tiene el rol correcto." });

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

        [HttpPost("tratamiento")]
        public async Task<IActionResult> RegistrarTratamiento([FromBody] RegistrarTratamientoDto dto)
        {
            var (ok, mensaje, id) = await _tratamientoService.RegistrarTratamiento(dto);

            if (!ok)
                return BadRequest(new { mensaje });

            return Ok(new
            {
                mensaje,
                id
            });
        }
        [Authorize(Roles = "Administrador,Veterinario")]
[HttpPut("tratamiento/{id}/estado")]
        public async Task<IActionResult> ActualizarEstadoTratamiento(int id, [FromBody] ActualizarEstadoTratamientoDto dto)
        {
            var veterinarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var (ok, mensaje) = await _tratamientoService
                .ActualizarEstadoTratamiento(id, dto.Estado, veterinarioId);

            if (!ok)
                return BadRequest(new { mensaje });

            return Ok(new { mensaje });
        }



    }

}
