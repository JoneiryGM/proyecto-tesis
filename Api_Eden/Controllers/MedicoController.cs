
using Api_Eden.DTOs.MedicoDto;
using Api_Eden.Services.TratamientoService.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Api_Eden.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MedicoController : ControllerBase
    {
        private readonly IMedicoService _medicoService;
        private readonly ITratamientoService _tratamientoService;

        public MedicoController(
            IMedicoService medicoService,
            ITratamientoService tratamientoService)
        {
            _medicoService = medicoService;
            _tratamientoService = tratamientoService;
        }

        [Authorize]
        [HttpGet("tratamientos")]
        public async Task<IActionResult> GetTratamientos()
        {
            try { return Ok(await _medicoService.GetTratamientosAsync()); }
            catch (Exception ex) { return StatusCode(500, new { mensaje = "Error al obtener tratamientos", error = ex.Message }); }
        }

        [Authorize]
        [HttpGet("tratamientosall")]
        public async Task<IActionResult> GetAllTratamientos()
        {
            try { return Ok(await _medicoService.GetAllTratamientosAsync()); }
            catch (Exception ex) { return StatusCode(500, new { mensaje = "Error al obtener tratamientos", error = ex.Message }); }
        }

        // FIX: devuelve [] en vez de 404 cuando no hay historial
        [Authorize]
        [HttpGet("historial/{animalId}")]
        public async Task<IActionResult> GetHistorial(int animalId)
        {
            try { return Ok(await _medicoService.GetHistorialAsync(animalId)); }
            catch (Exception ex) { return StatusCode(500, new { mensaje = "Error al obtener historial", error = ex.Message }); }
        }

        [Authorize(Roles = "Administrador,Veterinario")]
        [HttpPost("historial")]
        public async Task<IActionResult> RegistrarHistorial([FromBody] RegistrarHistorialDto dto)
        {
            try
            {
                var (ok, mensaje, id) = await _medicoService.RegistrarHistorialAsync(dto);
                if (!ok) return BadRequest(new { mensaje });
                return Ok(new { mensaje, id });
            }
            catch (Exception ex) { return StatusCode(500, new { mensaje = "Error al registrar historial", error = ex.Message }); }
        }

        [Authorize(Roles = "Administrador,Veterinario")]
        [HttpPost("tratamiento")]
        public async Task<IActionResult> RegistrarTratamiento([FromBody] RegistrarTratamientoDto dto)
        {
            var (ok, mensaje, id) = await _tratamientoService.RegistrarTratamiento(dto);
            if (!ok) return BadRequest(new { mensaje });
            return Ok(new { mensaje, id });
        }

        [Authorize(Roles = "Administrador,Veterinario")]
        [HttpPut("tratamiento/{id}/estado")]
        public async Task<IActionResult> ActualizarEstadoTratamiento(
            int id, [FromBody] ActualizarEstadoTratamientoDto dto)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId))
                return Unauthorized(new { mensaje = "No se pudo identificar al usuario." });

            var (ok, mensaje) = await _tratamientoService.ActualizarEstadoTratamiento(id, dto.Estado, userId);
            if (!ok) return BadRequest(new { mensaje });
            return Ok(new { mensaje });
        }

        [Authorize]
        [HttpGet("medicamentos")]
        public async Task<IActionResult> GetMedicamentos()
        {
            try { return Ok(await _medicoService.GetMedicamentosAsync()); }
            catch (Exception ex) { return StatusCode(500, new { mensaje = "Error al obtener medicamentos", error = ex.Message }); }
        }

        [Authorize(Roles = "Administrador,Veterinario")]
        [HttpPost("medicamentos")]
        public async Task<IActionResult> CrearMedicamento([FromBody] CrearMedicamentoDto dto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dto.Nombre))
                    return BadRequest(new { mensaje = "El nombre del medicamento es obligatorio." });

                var (ok, data) = await _medicoService.CrearMedicamentoAsync(dto.Nombre);
                if (!ok) return BadRequest(new { mensaje = "Nombre inválido." });
                return Ok(data);
            }
            catch (Exception ex) { return StatusCode(500, new { mensaje = "Error al crear medicamento", error = ex.Message }); }
        }

        [Authorize]
        [HttpGet("tipos-vacuna")]
        public async Task<IActionResult> GetTiposVacuna()
        {
            try { return Ok(await _medicoService.GetTiposVacunaAsync()); }
            catch (Exception ex) { return StatusCode(500, new { mensaje = "Error al obtener tipos de vacuna", error = ex.Message }); }
        }

        [Authorize]
        [HttpGet("veterinarios")]
        public async Task<IActionResult> GetVeterinarios()
        {
            try { return Ok(await _medicoService.GetVeterinariosAsync()); }
            catch (Exception ex) { return StatusCode(500, new { mensaje = "Error al obtener veterinarios", error = ex.Message }); }
        }
    }
}