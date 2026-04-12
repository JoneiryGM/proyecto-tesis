using Api_Eden.Data;
using Api_Eden.DTOs.MedicoDto;
using Api_Eden.Services.TratamientoService.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api_Eden.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FallecimientoController : ControllerBase
    {
        private readonly IFallecimientoService _service;

        public FallecimientoController(IFallecimientoService service)
        {
            _service = service;
        }
        public static class Roles
        {
            public const string Admin = "Administrador";
            public const string Veterinario = "Veterinario";

        }
        [Authorize(Roles = Roles.Admin)]
        [HttpGet]
        public async Task<IActionResult> GetFallecimientos([FromServices] AppDbContext _db)
        {
            var data = await _db.Fallecimientos
                .Include(f => f.Animal)
                .Include(f => f.Veterinario)
                .Include(f => f.UsuarioRegistro)
                .OrderByDescending(f => f.Fecha)
                .Select(f => new
                {
                    f.Id,
                    Animal = f.Animal.Nombre,
                    f.Fecha,
                    f.Causa,
                    Veterinario = $"{f.Veterinario.Nombre} {f.Veterinario.Apellido}",
                    RegistradoPor = $"{f.UsuarioRegistro.Nombre} {f.UsuarioRegistro.Apellido}",
                    f.Observaciones,
                    f.FechaCreacion
                })
                .ToListAsync();

            return Ok(data);
        }

        [Authorize(Roles = "Administrador,Veterinario")]
        [HttpPost]
        public async Task<IActionResult> RegistrarFallecimiento([FromBody] RegistrarFallecimientoDto dto)
        {
            var (ok, mensaje) = await _service.RegistrarFallecimiento(dto);

            if (!ok)
                return BadRequest(new { mensaje });

            return Ok(new { mensaje });
        }
    }
}
