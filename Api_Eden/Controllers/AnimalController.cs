
using Api_Eden.DTOs.AnimalCreadoDto;
using Api_Eden.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

//Nota  : Este controlador se centra exclusivamente en la gestión de animales, incluyendo operaciones CRUD (Crear, Leer, Actualizar, Eliminar). No incluye funcionalidades relacionadas con adopciones, alimentos o usuarios, que deberían manejarse en controladores separados para mantener una arquitectura limpia y modular.
//Nota2: Aunque cree las autenticaciones de seguridad,no las implementé en este controlador para facilitar las pruebas iniciales. Sin embargo, en un entorno de producción, se recomienda proteger estos endpoints con autenticación y autorización adecuadas para garantizar la seguridad de los datos y las operaciones.

namespace Api_Eden.Controllers
{


    [ApiController]
    [Route("api/[controller]")]
    public class AnimalController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AnimalController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/animal
        [HttpGet]
        [Authorize(Roles = "Administrador,Veterinario")]
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
                return StatusCode(500, new { mensaje = "Error interno del servidor", ex.Message });
            }
        }

        // GET: api/animal  **OBTENER UN ANIMAL POR ID**
        [HttpGet("{id}")]
        [Authorize(Roles = "Administrador,Veterinario,Trabajador")]
        public async Task<ActionResult<AnimalDTO>> GetAnimal(int id)
        {
            try
            {
                var animal = await _context.Animales
                    .Where(a => a.Id == id)
                    .Select(a => new AnimalDTO
                    {
                        Id = a.Id,
                        Nombre = a.Nombre,
                        EstadoSalud = a.EstadoSalud,
                        Raza = a.Raza,
                        EstadoGeneral = a.EstadoGeneral,
                        Zona = a.ZonaActual != null ? a.ZonaActual.Nombre : null
                    })
                    .FirstOrDefaultAsync();
                if (animal == null)
                {
                    return NotFound(new { mensaje = "Animal no encontrado" });
                }
                return Ok(animal);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error interno del servidor", ex.Message });
            }
        }

        //Post : api/animal  **CREAR UN NUEVO ANIMAL**
        [Authorize(Roles = "Administrador")]
        [HttpPost]
        public async Task<ActionResult<AnimalDTO>> PostAnimal([FromBody] CrearAnimalDto dto
            )
        {
            try
            {
                
                if (dto.ZonaActualId.HasValue)
                {
                    var zonaExiste = await _context.Zonas.AnyAsync(z => z.Id == dto.ZonaActualId);
                    if (!zonaExiste)
                    {
                        return BadRequest(new { mensaje = "La zona especificada no existe" });
                    }
                }

             
                var nuevoAnimal = new Animale
                {
                    Nombre = dto.Nombre,
                    EspecieId = dto.EspecieId,
                    Raza = dto.Raza,
                    Edad = dto.Edad,
                    Sexo = dto.Sexo,
                    FechaIngreso = dto.FechaIngreso ?? DateOnly.FromDateTime(DateTime.Now),
                    ZonaActualId = dto.ZonaActualId,
                    FechaCreacion = DateTime.Now,
                    FechaUltimaModificacion = DateTime.Now
                };

                _context.Animales.Add(nuevoAnimal);
                await _context.SaveChangesAsync();

                
                var animalDto = new AnimalDTO
                {
                    Id = nuevoAnimal.Id,
                    Nombre = nuevoAnimal.Nombre,
                    EstadoSalud = nuevoAnimal.EstadoSalud,
                    Raza = nuevoAnimal.Raza,
                    EstadoGeneral = nuevoAnimal.EstadoGeneral,
                    Zona = nuevoAnimal.ZonaActual != null ? nuevoAnimal.ZonaActual.Nombre : null
                };

                return CreatedAtAction(nameof(GetAnimal), new { id = nuevoAnimal.Id }, animalDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al crear el animal", error = ex.Message });
            }
        }

        //put : api/animal/{id}  **ACTUALIZAR UN ANIMAL EXISTENTE**

        [Authorize(Roles = "Administrador")]
        [HttpPut("{id}")]

        public async Task<ActionResult> PutAnimal(int id, [FromBody] CrearAnimalDto dto)
        {
            try
            {
                var animalExistente = await _context.Animales.FindAsync(id);
                if (animalExistente == null)
                {
                    return NotFound(new { mensaje = "Animal no encontrado" });
                }
               
                if (dto.ZonaActualId.HasValue)
                {
                    var zonaExiste = await _context.Zonas.AnyAsync(z => z.Id == dto.ZonaActualId);
                    if (!zonaExiste)
                    {
                        return BadRequest(new { mensaje = "La zona especificada no existe" });
                    }
                }
                
                animalExistente.Nombre = dto.Nombre;
                animalExistente.EspecieId = dto.EspecieId;
                animalExistente.Raza = dto.Raza;
                animalExistente.Edad = dto.Edad;
                animalExistente.Sexo = dto.Sexo;
                animalExistente.ZonaActualId = dto.ZonaActualId;

                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al actualizar el animal", error = ex.Message });
            }

        }

        //Delete : api/animal/{id}  **ELIMINAR UN ANIMAL**
        [Authorize(Roles = "Administrador")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAnimal(int id)
        {
            try
            {
                var animal = await _context.Animales.FindAsync(id);
                if (animal == null)
                {
                    return NotFound(new { mensaje = "Animal no encontrado" });
                }

                _context.Animales.Remove(animal);
                await _context.SaveChangesAsync();

                return Ok(new { mensaje = "Animal eliminado correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al eliminar el animal", error = ex.Message });
            }
        }

        // Método auxiliar
        private bool AnimalExists(int id)
        {
            return _context.Animales.Any(e => e.Id == id);
        }
    } 
}