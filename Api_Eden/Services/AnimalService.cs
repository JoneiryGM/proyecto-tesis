namespace Api_Eden.Services;

using Api_Eden.DTOs.AnimalCreadoDto;
using Api_Eden.Models;
using Microsoft.EntityFrameworkCore;


public class AnimalService
{
    private readonly AppDbContext _context;

    public AnimalService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<AnimalDTO>> GetAllAsync()
    {
        return await _context.Animales
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
    }

    public async Task<AnimalDTO?> GetByIdAsync(int id)
    {
        return await _context.Animales
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
    }

    public async Task<AnimalDTO> CreateAsync(CrearAnimalDto dto)
    {
        if (dto.ZonaActualId.HasValue)
        {
            var zonaExiste = await _context.Zonas.AnyAsync(z => z.Id == dto.ZonaActualId);
            if (!zonaExiste) throw new ArgumentException("La zona especificada no existe");
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

        return new AnimalDTO
        {
            Id = nuevoAnimal.Id,
            Nombre = nuevoAnimal.Nombre,
            EstadoSalud = nuevoAnimal.EstadoSalud,
            Raza = nuevoAnimal.Raza,
            EstadoGeneral = nuevoAnimal.EstadoGeneral,
            Zona = nuevoAnimal.ZonaActual != null ? nuevoAnimal.ZonaActual.Nombre : null
        };
    }

    public async Task<bool> UpdateAsync(int id, CrearAnimalDto dto)
    {
        var animalExistente = await _context.Animales.FindAsync(id);
        if (animalExistente == null) return false;

        if (dto.ZonaActualId.HasValue)
        {
            var zonaExiste = await _context.Zonas.AnyAsync(z => z.Id == dto.ZonaActualId);
            if (!zonaExiste) throw new ArgumentException("La zona especificada no existe");
        }

        animalExistente.Nombre = dto.Nombre;
        animalExistente.EspecieId = dto.EspecieId;
        animalExistente.Raza = dto.Raza;
        animalExistente.Edad = dto.Edad;
        animalExistente.Sexo = dto.Sexo;
        animalExistente.ZonaActualId = dto.ZonaActualId;
        animalExistente.FechaUltimaModificacion = DateTime.Now;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var animal = await _context.Animales.FindAsync(id);
        if (animal == null) return false;

        _context.Animales.Remove(animal);
        await _context.SaveChangesAsync();
        return true;
    }
}