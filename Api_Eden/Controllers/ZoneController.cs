namespace Api_Eden.Controllers;

using global::Api_Eden.DTOs.Zone.Request;
using global::Api_Eden.DTOs.Zone.Response;
using global::Api_Eden.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ZoneController : ControllerBase
{
    private readonly ZoneService _zoneService;

    public ZoneController(ZoneService zoneService) => _zoneService = zoneService;

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ZoneResponseDto>>> GetAll()
    {
        try { return Ok(await _zoneService.GetAllAsync()); }
        catch (Exception ex) { return StatusCode(500, new { mensaje = "Error al obtener zonas", error = ex.Message }); }
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<ActionResult<ZoneResponseDto>> GetById(int id)
    {
        try { return Ok(await _zoneService.GetByIdAsync(id)); }
        catch (KeyNotFoundException ex) { return NotFound(new { mensaje = ex.Message }); }
        catch (ArgumentException ex) { return BadRequest(new { mensaje = ex.Message }); }
        catch (Exception ex) { return StatusCode(500, new { mensaje = "Error al obtener zona", error = ex.Message }); }
    }

    /// <summary>
    /// Devuelve los animales que están actualmente en una zona específica
    /// </summary>
    [Authorize]
    [HttpGet("{id}/animales")]
    public async Task<ActionResult> GetAnimalesByZona(int id)
    {
        try { return Ok(await _zoneService.GetAnimalesByZonaAsync(id)); }
        catch (KeyNotFoundException ex) { return NotFound(new { mensaje = ex.Message }); }
        catch (Exception ex) { return StatusCode(500, new { mensaje = "Error al obtener animales de la zona", error = ex.Message }); }
    }

    /// <summary>
    /// Devuelve el historial de movimientos de todas las zonas
    /// </summary>
    [Authorize]
    [HttpGet("movimientos")]
    public async Task<ActionResult> GetMovimientos()
    {
        try { return Ok(await _zoneService.GetMovimientosAsync()); }
        catch (Exception ex) { return StatusCode(500, new { mensaje = "Error al obtener movimientos", error = ex.Message }); }
    }

    /// <summary>
    /// Devuelve el historial de movimientos de un animal específico
    /// </summary>
    [Authorize]
    [HttpGet("movimientos/{animalId}")]
    public async Task<ActionResult> GetMovimientosByAnimal(int animalId)
    {
        try { return Ok(await _zoneService.GetMovimientosByAnimalAsync(animalId)); }
        catch (Exception ex) { return StatusCode(500, new { mensaje = "Error al obtener movimientos del animal", error = ex.Message }); }
    }

    /// <summary>
    /// Mueve un animal de su zona actual a una zona destino y registra el movimiento
    /// </summary>
    [Authorize(Roles = "Administrador,Veterinario")]
    [HttpPost("mover-animal")]
    public async Task<ActionResult> MoverAnimal([FromBody] MoverAnimalDto dto)
    {
        try
        {
            // Obtener el ID del usuario autenticado desde el token JWT
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int usuarioId))
                return Unauthorized(new { mensaje = "No se pudo identificar al usuario" });

            await _zoneService.MoverAnimalAsync(dto, usuarioId);
            return Ok(new { mensaje = "Animal movido correctamente" });
        }
        catch (KeyNotFoundException ex) { return NotFound(new { mensaje = ex.Message }); }
        catch (InvalidOperationException ex) { return BadRequest(new { mensaje = ex.Message }); }
        catch (ArgumentException ex) { return BadRequest(new { mensaje = ex.Message }); }
        catch (Exception ex) { return StatusCode(500, new { mensaje = "Error al mover el animal", error = ex.Message }); }
    }

    [Authorize(Roles = "Administrador")]
    [HttpPost]
    public async Task<ActionResult<ZoneResponseDto>> Create([FromBody] CreateZoneDto dto)
    {
        try
        {
            var zone = await _zoneService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = zone.Id }, zone);
        }
        catch (ArgumentException ex) { return BadRequest(new { mensaje = ex.Message }); }
        catch (Exception ex) { return StatusCode(500, new { mensaje = "Error al crear zona", error = ex.Message }); }
    }

    [Authorize(Roles = "Administrador")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] CreateZoneDto dto)
    {
        try
        {
            var updated = await _zoneService.UpdateAsync(id, dto);
            if (!updated) return NotFound(new { mensaje = $"No existe una zona con ID {id}." });
            return NoContent();
        }
        catch (ArgumentException ex) { return BadRequest(new { mensaje = ex.Message }); }
        catch (Exception ex) { return StatusCode(500, new { mensaje = "Error al actualizar zona", error = ex.Message }); }
    }

    [Authorize(Roles = "Administrador")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var deleted = await _zoneService.DeleteAsync(id);
            if (!deleted) return NotFound(new { mensaje = $"No existe una zona con ID {id}." });
            return NoContent();
        }
        catch (InvalidOperationException ex) { return BadRequest(new { mensaje = ex.Message }); }
        catch (ArgumentException ex) { return BadRequest(new { mensaje = ex.Message }); }
        catch (Exception ex) { return StatusCode(500, new { mensaje = "Error al eliminar zona", error = ex.Message }); }
    }
}