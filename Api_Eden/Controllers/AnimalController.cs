using Api_Eden.DTOs;
using Api_Eden.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api_Eden.Controllers
{
    [ApiController] // 🔥 IMPORTANTE
    [Route("api/[controller]")] // 🔥 IMPORTANTE
    public class AnimalController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AnimalController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/animal
        [HttpGet]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AnimalDTO>>> GetAnimales()
        {
            try
            {
                var animales = await _context.Animales
                    .Select(a => new AnimalDTO
                    {
                        Id = a.Id,
                        Nombre = a.Nombre,
                        EstadoSalud = a.EstadoSalud,
                        Raza = a.Raza,
                        EstadoGeneral = a.EstadoGeneral,
                        Zona = a.ZonaActual != null ? a.ZonaActual.Nombre : null
                    })
                    .ToListAsync();

                return Ok(animales);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = ex.Message });
            }
        }
    }
}
