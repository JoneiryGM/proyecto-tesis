namespace Api_Eden.Services;

using Api_Eden.Data;
using Api_Eden.DTOs.Zone.Request;
using Api_Eden.DTOs.Zone.Response;
using Api_Eden.Models;
using Mapster;
using Microsoft.EntityFrameworkCore;

using Microsoft.Extensions.Logging;

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
        try
        {
            _logger.LogInformation("Obteniendo todas las zonas");
            return await _context.Zonas
                .Select(z => new ZoneResponseDto(z.Id, z.Nombre, z.Descripcion,
                    z.CapacidadMaxima, z.CantidadActual, z.Activa, z.FechaCreacion))
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener todas las zonas");
            throw;
        }
    }

    public async Task<ZoneResponseDto?> GetByIdAsync(int id)
    {
        try
        {
            if (id <= 0)
            {
                _logger.LogWarning("ID de zona inválido: {Id}", id);
                throw new ArgumentException("El ID debe ser mayor a 0.");
            }

            _logger.LogInformation("Obteniendo zona con ID {Id}", id);
            var zona = await _context.Zonas
                .Where(z => z.Id == id)
                .Select(z => new ZoneResponseDto(z.Id, z.Nombre, z.Descripcion,
                    z.CapacidadMaxima, z.CantidadActual, z.Activa, z.FechaCreacion))
                .FirstOrDefaultAsync();

            if (zona is null)
            {
                _logger.LogWarning("Zona con ID {Id} no encontrada", id);
                throw new KeyNotFoundException($"No existe una zona con ID {id}.");
            }

            return zona;
        }
        catch (Exception ex) when (ex is not ArgumentException && ex is not KeyNotFoundException)
        {
            _logger.LogError(ex, "Error al obtener zona con ID {Id}", id);
            throw;
        }
    }

    public async Task<ZoneResponseDto> CreateAsync(CreateZoneDto dto)
    {
        try
        {
            // Validar nombre
            if (string.IsNullOrWhiteSpace(dto.Nombre))
                throw new ArgumentException("El nombre de la zona es obligatorio.");

            if (dto.Nombre.Length > 100)
                throw new ArgumentException("El nombre no puede superar los 100 caracteres.");

            // Validar capacidad
            if (dto.CapacidadMaxima <= 0)
                throw new ArgumentException("La capacidad máxima debe ser mayor a 0.");

            // Verificar nombre duplicado
            if (await _context.Zonas.AnyAsync(z => z.Nombre == dto.Nombre))
            {
                _logger.LogWarning("Intento de crear zona duplicada: {Nombre}", dto.Nombre);
                throw new ArgumentException($"Ya existe una zona con el nombre '{dto.Nombre}'.");
            }

            var newZone = dto.Adapt<Zona>();
            newZone.FechaCreacion = DateTime.Now;
            newZone.CantidadActual = 0; // siempre empieza en 0
            newZone.Activa = true;

            _context.Zonas.Add(newZone);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Zona creada correctamente: {Nombre}", dto.Nombre);
            return newZone.Adapt<ZoneResponseDto>();
        }
        catch (Exception ex) when (ex is not ArgumentException)
        {
            _logger.LogError(ex, "Error al crear zona: {Nombre}", dto.Nombre);
            throw;
        }
    }

    public async Task<bool> UpdateAsync(int id, CreateZoneDto dto)
    {
        try
        {
            if (id <= 0)
                throw new ArgumentException("El ID debe ser mayor a 0.");

            if (string.IsNullOrWhiteSpace(dto.Nombre))
                throw new ArgumentException("El nombre de la zona es obligatorio.");

            if (dto.Nombre.Length > 100)
                throw new ArgumentException("El nombre no puede superar los 100 caracteres.");

            if (dto.CapacidadMaxima <= 0)
                throw new ArgumentException("La capacidad máxima debe ser mayor a 0.");

            var zona = await _context.Zonas.FirstOrDefaultAsync(z => z.Id == id);
            if (zona is null)
            {
                _logger.LogWarning("Zona con ID {Id} no encontrada para actualizar", id);
                return false;
            }

            // Verificar nombre duplicado en otra zona
            if (await _context.Zonas.AnyAsync(z => z.Nombre == dto.Nombre && z.Id != id))
                throw new ArgumentException($"Ya existe otra zona con el nombre '{dto.Nombre}'.");

            // Validar que la nueva capacidad no sea menor a la cantidad actual
            if (dto.CapacidadMaxima < zona.CantidadActual)
                throw new ArgumentException(
                    $"La capacidad máxima ({dto.CapacidadMaxima}) no puede ser menor a la cantidad actual de animales ({zona.CantidadActual}).");

            dto.Adapt(zona);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Zona {Id} actualizada correctamente", id);
            return true;
        }
        catch (Exception ex) when (ex is not ArgumentException && ex is not KeyNotFoundException)
        {
            _logger.LogError(ex, "Error al actualizar zona con ID {Id}", id);
            throw;
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        try
        {
            if (id <= 0)
                throw new ArgumentException("El ID debe ser mayor a 0.");

            var zona = await _context.Zonas
                .Include(z => z.Animales)
                .FirstOrDefaultAsync(z => z.Id == id);

            if (zona is null)
            {
                _logger.LogWarning("Zona con ID {Id} no encontrada para eliminar", id);
                return false;
            }

            // No eliminar si tiene animales activos
            var tieneAnimales = zona.Animales.Any(a => a.EstadoGeneral == "Activo");
            if (tieneAnimales)
                throw new InvalidOperationException(
                    $"No se puede eliminar la zona '{zona.Nombre}' porque tiene {zona.CantidadActual} animal(es) activo(s). Muévalos primero.");

            // Baja lógica en lugar de eliminación física
            zona.Activa = false;
            await _context.SaveChangesAsync();

            _logger.LogInformation("Zona {Id} desactivada correctamente", id);
            return true;
        }
        catch (Exception ex) when (ex is not ArgumentException && ex is not InvalidOperationException)
        {
            _logger.LogError(ex, "Error al eliminar zona con ID {Id}", id);
            throw;
        }
    }
}

// TODO: Usar un Mapeador Automático (Mapster), para evitar errores de asignación manual y mejorar la mantenibilidad del código.