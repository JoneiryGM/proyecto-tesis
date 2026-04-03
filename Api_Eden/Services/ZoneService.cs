namespace Api_Eden.Services;

using Api_Eden.DTOs.Zone.Request;
using Api_Eden.DTOs.Zone.Response;
using Api_Eden.Models;
using Mapster;
using Microsoft.EntityFrameworkCore;

public class ZoneService
{
    private readonly AppDbContext _context;

    private readonly ILogger<ZoneService> _logger;
    public ZoneService(AppDbContext context, ILogger<ZoneService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<ZoneResponseDto>> GetAllAsync()
    {
        _logger.LogInformation("Obteniendo todas las zonas");
        return await _context.Zonas
            .Select(z => new ZoneResponseDto(z.Id,z.Nombre,z.Descripcion,
                z.CapacidadMaxima,z.CantidadActual,z.Activa,z.FechaCreacion))
            .ToListAsync();
    }

    public async Task<ZoneResponseDto?> GetByIdAsync(int id)
    {
        _logger.LogInformation("Obteniendo zona con ID {Id}", id);
        return await _context.Zonas
            .Where(z => z.Id == id)
            .Select(z => new ZoneResponseDto(z.Id, z.Nombre, z.Descripcion,
                z.CapacidadMaxima, z.CantidadActual, z.Activa, z.FechaCreacion))
            .FirstOrDefaultAsync();
    }


    public async Task<ZoneResponseDto> CreateAsync(CreateZoneDto dto)
    {
        if (await _context.Zonas.AnyAsync(z => z.Nombre == dto.Nombre)){
            _logger.LogWarning("Intento de crear una zona, ya existente: {Nombre}", dto.Nombre);
            throw new ArgumentException("Ya existe una zona con el mismo nombre");
        }

        var newZone = dto.Adapt<Zona>();
        newZone.FechaCreacion = DateTime.UtcNow;

        _context.Zonas.Add(newZone);
        await _context.SaveChangesAsync();

        return newZone.Adapt<ZoneResponseDto>();
    }

    public async Task<bool> UpdateAsync(int id, CreateZoneDto dto)
    {
        if (await _context.Zonas.AnyAsync(z => z.Nombre == dto.Nombre && z.Id != id))
            throw new ArgumentException("Ya existe una zona con el mismo nombre");

        var zona = await _context.Zonas.FirstOrDefaultAsync(z => z.Id == id);
        if (zona is null) return false;

        dto.Adapt(zona);

        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var zona = await _context.Zonas.FirstOrDefaultAsync(z => z.Id == id);
        if (zona is null) return false;

        _context.Zonas.Remove(zona);
        return await _context.SaveChangesAsync() > 0;
    }
}


// TODO: Usar un Mapeador Automático (Mapster), para evitar errores de asignación manual y mejorar la mantenibilidad del código.