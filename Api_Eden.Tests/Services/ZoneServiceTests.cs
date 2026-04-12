using Api_Eden.DTOs.Zone.Request;
using Api_Eden.DTOs.Zone.Response;
using Api_Eden.Models;
using Api_Eden.Services;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Api_Eden.Tests.Services;

public class ZoneServiceTests
{
    public ZoneServiceTests()
{
    TypeAdapterConfig<Zona, ZoneResponseDto>.NewConfig()
        .Map(dest => dest.Name, src => src.Nombre)
        .Map(dest => dest.Description, src => src.Descripcion)
        .Map(dest => dest.MaxCapacity, src => src.CapacidadMaxima)
        .Map(dest => dest.CurrentCapacity, src => src.CantidadActual)
        .Map(dest => dest.IsActive, src => src.Activa)
        .Map(dest => dest.CreatedAt, src => src.FechaCreacion);

    TypeAdapterConfig<CreateZoneDto, Zona>.NewConfig()
        .Map(dest => dest.Nombre, src => src.Nombre)
        .Map(dest => dest.CapacidadMaxima, src => src.CapacidadMaxima)
        .Map(dest => dest.CantidadActual, src => src.CantidadActual)
        .Map(dest => dest.Activa, src => src.Activa);
}
    private AppDbContext CrearContexto()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    private ZoneService CrearServicio(AppDbContext context)
    {
        var logger = new Mock<ILogger<ZoneService>>();
        return new ZoneService(context, logger.Object);
    }

    // ─── GetAllAsync ───────────────────────────────────────────

    [Fact]
    public async Task GetAllAsync_DebeRetornarTodasLasZonas()
    {
        var context = CrearContexto();
        context.Zonas.AddRange(
            new Zona { Nombre = "Zona A", CapacidadMaxima = 10, FechaCreacion = DateTime.Now },
            new Zona { Nombre = "Zona B", CapacidadMaxima = 20, FechaCreacion = DateTime.Now }
        );
        await context.SaveChangesAsync();

        var service = CrearServicio(context);
        var resultado = await service.GetAllAsync();

        Assert.Equal(2, resultado.Count());
    }

    [Fact]
    public async Task GetAllAsync_DebeRetornarListaVacia_CuandoNoHayZonas()
    {
        var context = CrearContexto();
        var service = CrearServicio(context);

        var resultado = await service.GetAllAsync();

        Assert.Empty(resultado);
    }

    // ─── GetByIdAsync ──────────────────────────────────────────

    [Fact]
    public async Task GetByIdAsync_DebeRetornarZona_CuandoExiste()
    {
        var context = CrearContexto();
        context.Zonas.Add(new Zona { Nombre = "Zona A", CapacidadMaxima = 10, FechaCreacion = DateTime.Now });
        await context.SaveChangesAsync();

        var service = CrearServicio(context);
        var zona = await context.Zonas.FirstAsync();
        var resultado = await service.GetByIdAsync(zona.Id);

        Assert.NotNull(resultado);
        Assert.Equal("Zona A", resultado.Name);
    }

    [Fact]
    public async Task GetByIdAsync_DebeRetornarNull_CuandoNoExiste()
    {
        var context = CrearContexto();
        var service = CrearServicio(context);

        var resultado = await service.GetByIdAsync(999);

        Assert.Null(resultado);
    }

    // ─── CreateAsync ───────────────────────────────────────────

    [Fact]
    public async Task CreateAsync_DebeCrearZona_CuandoDatosValidos()
    {
        var context = CrearContexto();
        var service = CrearServicio(context);

        var dto = new CreateZoneDto
        {
            Nombre = "Zona Nueva",
            Descripcion = "Descripción",
            CapacidadMaxima = 15,
            CantidadActual = 5,
            Activa = true
        };

        var resultado = await service.CreateAsync(dto);

        Assert.NotNull(resultado);
        Assert.Equal("Zona Nueva", resultado.Name);
        Assert.Equal(1, await context.Zonas.CountAsync());
    }

    [Fact]
    public async Task CreateAsync_DebeLanzarExcepcion_CuandoNombreDuplicado()
    {
        var context = CrearContexto();
        context.Zonas.Add(new Zona { Nombre = "Zona Existente", CapacidadMaxima = 10, FechaCreacion = DateTime.Now });
        await context.SaveChangesAsync();

        var service = CrearServicio(context);
        var dto = new CreateZoneDto
        {
            Nombre = "Zona Existente",
            CapacidadMaxima = 10,
            CantidadActual = 0,
            Activa = true
        };

        await Assert.ThrowsAsync<ArgumentException>(() => service.CreateAsync(dto));
    }

    // ─── UpdateAsync ───────────────────────────────────────────

    [Fact]
    public async Task UpdateAsync_DebeActualizarZona_CuandoExiste()
    {
        var context = CrearContexto();
        context.Zonas.Add(new Zona { Nombre = "Zona Vieja", CapacidadMaxima = 10, FechaCreacion = DateTime.Now });
        await context.SaveChangesAsync();

        var service = CrearServicio(context);
        var zona = await context.Zonas.FirstAsync();

        var dto = new CreateZoneDto
        {
            Nombre = "Zona Actualizada",
            CapacidadMaxima = 20,
            CantidadActual = 5,
            Activa = true
        };

        var resultado = await service.UpdateAsync(zona.Id, dto);

        Assert.True(resultado);
        Assert.Equal("Zona Actualizada", (await context.Zonas.FirstAsync()).Nombre);
    }

    [Fact]
    public async Task UpdateAsync_DebeRetornarFalse_CuandoNoExiste()
    {
        var context = CrearContexto();
        var service = CrearServicio(context);

        var dto = new CreateZoneDto
        {
            Nombre = "Zona X",
            CapacidadMaxima = 10,
            CantidadActual = 0,
            Activa = true
        };

        var resultado = await service.UpdateAsync(999, dto);

        Assert.False(resultado);
    }

    [Fact]
    public async Task UpdateAsync_DebeLanzarExcepcion_CuandoNombreDuplicado()
    {
        var context = CrearContexto();
        context.Zonas.AddRange(
            new Zona { Nombre = "Zona A", CapacidadMaxima = 10, FechaCreacion = DateTime.Now },
            new Zona { Nombre = "Zona B", CapacidadMaxima = 10, FechaCreacion = DateTime.Now }
        );
        await context.SaveChangesAsync();

        var service = CrearServicio(context);
        var zonaA = await context.Zonas.FirstAsync(z => z.Nombre == "Zona A");

        var dto = new CreateZoneDto
        {
            Nombre = "Zona B",
            CapacidadMaxima = 10,
            CantidadActual = 0,
            Activa = true
        };

        await Assert.ThrowsAsync<ArgumentException>(() => service.UpdateAsync(zonaA.Id, dto));
    }

    // ─── DeleteAsync ───────────────────────────────────────────

    [Fact]
    public async Task DeleteAsync_DebeEliminarZona_CuandoExiste()
    {
        var context = CrearContexto();
        context.Zonas.Add(new Zona { Nombre = "Zona A", CapacidadMaxima = 10, FechaCreacion = DateTime.Now });
        await context.SaveChangesAsync();

        var service = CrearServicio(context);
        var zona = await context.Zonas.FirstAsync();

        var resultado = await service.DeleteAsync(zona.Id);

        Assert.True(resultado);
        Assert.Equal(0, await context.Zonas.CountAsync());
    }

    [Fact]
    public async Task DeleteAsync_DebeRetornarFalse_CuandoNoExiste()
    {
        var context = CrearContexto();
        var service = CrearServicio(context);

        var resultado = await service.DeleteAsync(999);

        Assert.False(resultado);
    }
}