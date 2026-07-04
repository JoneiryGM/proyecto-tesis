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

        // ── Tratamientos globales (tab "Tratamientos" en Gestión Médica) ──────
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

        // ── Historial por animal (solo consultas + tratamientos embebidos) ─────
        // GET api/medico/historial/{animalId}
        [Authorize]
        [HttpGet("historial/{animalId}")]
        public async Task<IActionResult> GetHistorial(int animalId)
        {
            try { return Ok(await _medicoService.GetHistorialAsync(animalId)); }
            catch (Exception ex) { return StatusCode(500, new { mensaje = "Error al obtener historial", error = ex.Message }); }
        }

        // ── NUEVO: Timeline unificada por animal ──────────────────────────────
        // Devuelve una lista plana ordenada por fecha con tipo = consulta | vacuna | fallecimiento
        // El frontend renderiza cada item según su "Tipo" para mostrar todo en un solo historial.
        //
        // Ejemplo de respuesta:
        // [
        //   { "tipo": "consulta",      "fecha": "2026-06-28", "titulo": "gripe", "tratamientos": [...] },
        //   { "tipo": "vacuna",        "fecha": "2026-06-27", "titulo": "Moquillo", "vencida": false },
        //   { "tipo": "consulta",      "fecha": "2026-06-25", "titulo": "dsasd",  "tratamientos": [...] },
        //   { "tipo": "fallecimiento", "fecha": "2026-05-02", "titulo": "Paro cardíaco" }
        // ]
        //
        // GET api/medico/historial/{animalId}/timeline
        [Authorize]
        [HttpGet("historial/{animalId}/timeline")]
        public async Task<IActionResult> GetTimeline(int animalId)
        {
            try
            {
                var resultado = await _medicoService.GetTimelineAnimalAsync(animalId);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al obtener timeline", error = ex.Message });
            }
        }

        // ── Historial completo estructurado (consultas + vacunas + fallecimiento separados) ─
        // GET api/medico/historial/{animalId}/completo
        [Authorize]
        [HttpGet("historial/{animalId}/completo")]
        public async Task<IActionResult> GetHistorialCompleto(int animalId)
        {
            try
            {
                var resultado = await _medicoService.GetHistorialCompletoAnimalAsync(animalId);
                if (resultado is null)
                    return NotFound(new { mensaje = "Animal no encontrado." });
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al obtener historial completo", error = ex.Message });
            }
        }

        // ── Registros ─────────────────────────────────────────────────────────
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
            try
            {
                var (ok, mensaje, id) = await _tratamientoService.RegistrarTratamiento(dto);
                if (!ok) return BadRequest(new { mensaje });
                return Ok(new { mensaje, id });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al registrar tratamiento", error = ex.Message });
            }
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

        // ── Catálogos ─────────────────────────────────────────────────────────
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