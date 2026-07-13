using Api_Eden.Data;
using Api_Eden.DTOs.MedicoDto;
using Api_Eden.Models;
using Api_Eden.Services.TratamientoService;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using FluentAssertions;

namespace Api_Eden.Tests.Services;

public class FallecimientoServiceTests
{
    // Este servicio abre una transacción real (_db.Database.BeginTransactionAsync())
    // como primera línea del método, fuera del try/catch. Con Mock<AppDbContext> puro
    // eso lanza una excepción antes de llegar a la lógica de negocio, así que aquí
    // usamos un AppDbContext real con el proveedor InMemory de EF Core, que sí soporta
    // transacciones (como operación "no-op") sin necesitar una base de datos real.
    private static AppDbContext CrearContexto()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()) // BD aislada por test
            .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task RegistrarFallecimiento_AnimalNoExiste_RetornaFalso()
    {
        // ARRANGE: contexto vacío, no se agrega ningún animal
        using var context = CrearContexto();
        var service = new FallecimientoService(context);

        var dto = new RegistrarFallecimientoDto(
            AnimalId: 99,
            FechaFallecimiento: DateOnly.FromDateTime(DateTime.Today),
            CausaFallecimiento: "Causa natural",
            VeterinarioId: 1,
            UsuarioRegistroId: 1,
            Observaciones: null);

        // ACT
        var (ok, mensaje) = await service.RegistrarFallecimiento(dto);

        // ASSERT
        ok.Should().BeFalse();
        mensaje.Should().Be("Animal no encontrado.");
    }

    [Fact]
    public async Task RegistrarFallecimiento_AnimalYaFallecido_RetornaFalso()
    {
        // ARRANGE: animal ya marcado como fallecido
        using var context = CrearContexto();
        context.Animales.Add(new Animale
        {
            Id = 1,
            Nombre = "Firulais",
            EspecieId = 1,
            FechaIngreso = DateOnly.FromDateTime(DateTime.Now),
            EstadoGeneral = "Fallecido",
            UnidadEdad = "años"
        });
        await context.SaveChangesAsync();

        var service = new FallecimientoService(context);

        var dto = new RegistrarFallecimientoDto(
            AnimalId: 1,
            FechaFallecimiento: DateOnly.FromDateTime(DateTime.Today),
            CausaFallecimiento: "Causa natural",
            VeterinarioId: 1,
            UsuarioRegistroId: 1,
            Observaciones: null);

        // ACT
        var (ok, mensaje) = await service.RegistrarFallecimiento(dto);

        // ASSERT
        ok.Should().BeFalse();
        mensaje.Should().Be("El animal ya está registrado como fallecido.");
    }
}