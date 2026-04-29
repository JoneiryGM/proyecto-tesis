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
        try
        {
            return Ok(await _zoneService.GetAllAsync());
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { mensaje = "Error al obtener zonas", error = ex.Message });
        }
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<ActionResult<ZoneResponseDto>> GetById(int id)
    {
        try
        {
            return Ok(await _zoneService.GetByIdAsync(id));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { mensaje = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { mensaje = "Error al obtener zona", error = ex.Message });
        }
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
        catch (ArgumentException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { mensaje = "Error al crear zona", error = ex.Message });
        }
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
        catch (ArgumentException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { mensaje = "Error al actualizar zona", error = ex.Message });
        }
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
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { mensaje = "Error al eliminar zona", error = ex.Message });
        }
    }
}

// TODO: agregar validaciones y agregue el try catch a cada método, para manejar errores de forma centralizada y evitar que el servidor devuelva errores 500 sin control.


