namespace Api_Eden.Services;

using Api_Eden.Data;
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

    private static AnimalDTO MapToDto(Animale a) => new AnimalDTO
    {
        Id = a.Id,
        Nombre = a.Nombre,
        Especie = a.Especie?.Nombre,
        EspecieId = a.EspecieId,
        Raza = a.Raza,
        Edad = a.Edad,
        FechaIngreso = a.FechaIngreso.ToString("yyyy-MM-dd"),
        Sexo = a.Sexo,
        ZonaActual = a.ZonaActual?.Nombre,
        ZonaActualId = a.ZonaActualId,
        Color = a.Color,
        FotografiaUrl = a.FotografiaUrl,
        Observaciones = a.Observaciones,
        EstadoSalud = a.EstadoSalud,
        EstadoGeneral = a.EstadoGeneral,
    };

    public async Task<IEnumerable<AnimalDTO>> GetAllAsync()
    {
        var animales = await _context.Animales
            .Include(a => a.Especie)
            .Include(a => a.ZonaActual)
            .ToListAsync();

        return animales.Select(MapToDto);
    }

    public async Task<AnimalDTO?> GetByIdAsync(int id)
    {
        var animal = await _context.Animales
            .Include(a => a.Especie)
            .Include(a => a.ZonaActual)
            .FirstOrDefaultAsync(a => a.Id == id);

        return animal == null ? null : MapToDto(animal);
    }

    public async Task<AnimalDTO> CreateAsync(CrearAnimalDto dto)
    {
        if (dto.ZonaActualId.HasValue)
        {
            var zonaExiste = await _context.Zonas.AnyAsync(z => z.Id == dto.ZonaActualId);
            if (!zonaExiste) throw new ArgumentException("La zona especificada no existe");
        }

        var especieExiste = await _context.Especies.AnyAsync(e => e.Id == dto.EspecieId);
        if (!especieExiste) throw new ArgumentException("La especie especificada no existe");

        var nuevoAnimal = new Animale
        {
            Nombre = dto.Nombre,
            EspecieId = dto.EspecieId,
            Raza = dto.Raza,
            Edad = dto.Edad,
            Sexo = dto.Sexo,
            Color = dto.Color,
            FotografiaUrl = dto.FotografiaUrl,
            Observaciones = dto.Observaciones,
            UsuarioRegistroId = dto.UsuarioRegistroId,
            FechaIngreso = dto.FechaIngreso ?? DateOnly.FromDateTime(DateTime.Now),
            ZonaActualId = dto.ZonaActualId,
            FechaCreacion = DateTime.Now,
            FechaUltimaModificacion = DateTime.Now
        };

        _context.Animales.Add(nuevoAnimal);
        await _context.SaveChangesAsync();

        return (await GetByIdAsync(nuevoAnimal.Id))!;
    }

    public async Task<bool> UpdateAsync(int id, CrearAnimalDto dto)
    {
        var animal = await _context.Animales.FindAsync(id);
        if (animal == null) return false;

        if (dto.ZonaActualId.HasValue)
        {
            var zonaExiste = await _context.Zonas.AnyAsync(z => z.Id == dto.ZonaActualId);
            if (!zonaExiste) throw new ArgumentException("La zona especificada no existe");
        }

        animal.Nombre = dto.Nombre;
        animal.EspecieId = dto.EspecieId;
        animal.Raza = dto.Raza;
        animal.Edad = dto.Edad;
        animal.Sexo = dto.Sexo;
        animal.Color = dto.Color;
        animal.FotografiaUrl = dto.FotografiaUrl;
        animal.Observaciones = dto.Observaciones;
        animal.ZonaActualId = dto.ZonaActualId;
        animal.FechaUltimaModificacion = DateTime.Now;

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

    // ── Nuevo: actualiza EstadoGeneral y/o EstadoSalud ───────────────────────
    public async Task<bool> ActualizarEstadoAsync(
        int id, string? estadoGeneral, string? estadoSalud)
    {
        var animal = await _context.Animales.FindAsync(id);
        if (animal == null) return false;

        if (!string.IsNullOrEmpty(estadoGeneral))
            animal.EstadoGeneral = estadoGeneral;

        if (!string.IsNullOrEmpty(estadoSalud))
            animal.EstadoSalud = estadoSalud;

        animal.FechaUltimaModificacion = DateTime.Now;
        await _context.SaveChangesAsync();
        return true;
    }
}