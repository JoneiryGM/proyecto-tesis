namespace Api_Eden.Controllers;

using Api_Eden.DTOs.Zone.Request;
using Api_Eden.DTOs.Zone.Response;
using Api_Eden.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ZoneController : ControllerBase
{
    private readonly ZoneService _zoneService;

    public ZoneController(ZoneService zoneService)
    {
        _zoneService = zoneService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ZoneResponseDto>>> GetAll() => 
        Ok(await _zoneService.GetAllAsync());


    [HttpGet("{id}")]
    public async Task<ActionResult<ZoneResponseDto>> GetById(int id) =>
        await _zoneService.GetByIdAsync(id) is ZoneResponseDto zone ? Ok(zone) : NotFound();


    [HttpPost]
    public async Task<ActionResult<ZoneResponseDto>> Create([FromBody] CreateZoneDto dto) =>
        await _zoneService.CreateAsync(dto) is ZoneResponseDto zone 
            ? CreatedAtAction(nameof(GetById), new { id = zone.Id }, zone) 
            : BadRequest();


    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] CreateZoneDto dto) =>
        await _zoneService.UpdateAsync(id, dto) ? NoContent() : NotFound();


    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id) => 
        await _zoneService.DeleteAsync(id) ? NoContent() : NotFound();
}

// TODO: agregar validaciones con el token

// TODO: validar el tema del error handling, para que se
// maneje desde el servicio y no desde el controlador, y 
// así evitar repetir código en cada controlador.


// TODO: agregar los test unitarios para el servicio y el controlador.