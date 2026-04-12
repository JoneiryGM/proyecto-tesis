using Api_Eden.DTOs.MedicoDto;
using Api_Eden.Services.TratamientoService.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api_Eden.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
   
    public class VacunaController : ControllerBase
    {
        private readonly IVacunaService _vacunaService;

      
        public VacunaController(IVacunaService vacunaService)
        {
            _vacunaService = vacunaService;
        }
        [HttpGet("animal/{animalId}")]
        public async Task<IActionResult> GetVacunasPorAnimal(int animalId)
        {
            var (ok, mensaje, data) = await _vacunaService.GetVacunasPorAnimal(animalId);

            if (!ok)
                return NotFound(new { mensaje });

            return Ok(data);
        }

        [HttpPost]
        public async Task<IActionResult> RegistrarVacuna([FromBody] RegistrarVacunaDto dto)
        {
            var (ok, mensaje, id) = await _vacunaService.RegistrarVacuna(dto);

            if (!ok)
                return BadRequest(new { mensaje });

            return Ok(new { mensaje, id });
        }
    }
}