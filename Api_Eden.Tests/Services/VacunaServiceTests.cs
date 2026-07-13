using Api_Eden.Data;
using Api_Eden.Models;
using Api_Eden.Services.TratamientoService;
using Moq;
using Moq.EntityFrameworkCore; // <-- La librería que instalamos
using FluentAssertions;

namespace Api_Eden.Tests.Services;

public class VacunaServiceTests
{
    private readonly Mock<AppDbContext> _mockContext;
    private readonly VacunaService _service;

    public VacunaServiceTests()
    {
        // 1. Inicializamos el mock del contexto
        _mockContext = new Mock<AppDbContext>();

        // 2. Inyectamos en el servicio
        _service = new VacunaService(_mockContext.Object);
    }

    [Fact]
    public async Task GetVacunasPorAnimal_AnimalNoExiste_RetornaFalso()
    {
        // ARRANGE: lista vacía, por lo que FindAsync no encontrará el animal
        var listaAnimalesVacia = new List<Animale>();
        _mockContext.Setup(c => c.Animales).ReturnsDbSet(listaAnimalesVacia);

        // ACT
        var (ok, mensaje, data) = await _service.GetVacunasPorAnimal(99);

        // ASSERT
        ok.Should().BeFalse();
        mensaje.Should().Be("Animal no encontrado.");
        data.Should().BeNull();
    }

    [Fact]
    public async Task GetVacunasPorAnimal_AnimalSinVacunas_RetornaFalso()
    {
        // ARRANGE
        var listaAnimales = new List<Animale>
        {
            new Animale { Id = 1, Nombre = "Firulais", EspecieId = 1, FechaIngreso = DateOnly.FromDateTime(DateTime.Now) }
        };
        _mockContext.Setup(c => c.Animales).ReturnsDbSet(listaAnimales);
        // FindAsync necesita configurarse explícitamente para "encontrar" el animal
        _mockContext.Setup(c => c.Animales.FindAsync(1)).ReturnsAsync(listaAnimales.First(a => a.Id == 1));

        var listaVacunasVacia = new List<Vacuna>();
        _mockContext.Setup(c => c.Vacunas).ReturnsDbSet(listaVacunasVacia);

        // ACT
        var (ok, mensaje, data) = await _service.GetVacunasPorAnimal(1);

        // ASSERT
        ok.Should().BeFalse();
        mensaje.Should().Be("El animal no tiene vacunas registradas.");
        data.Should().BeNull();
    }

    [Fact]
    public async Task GetVacunasPorAnimal_AnimalConVacunas_RetornaListadoOk()
    {
        // ARRANGE
        var listaAnimales = new List<Animale>
        {
            new Animale { Id = 1, Nombre = "Firulais", EspecieId = 1, FechaIngreso = DateOnly.FromDateTime(DateTime.Now) }
        };
        _mockContext.Setup(c => c.Animales).ReturnsDbSet(listaAnimales);
        _mockContext.Setup(c => c.Animales.FindAsync(1)).ReturnsAsync(listaAnimales.First(a => a.Id == 1));

        var listaVacunas = new List<Vacuna>
        {
            new Vacuna
            {
                Id = 1,
                AnimalId = 1,
                TipoVacunaId = 1,
                FechaAplicacion = DateOnly.FromDateTime(DateTime.Today.AddDays(-30)),
                ProximaDosis = DateOnly.FromDateTime(DateTime.Today.AddMonths(11)),
                VeterinarioId = 1,
                TipoVacuna = new Tiposvacuna { Id = 1, Nombre = "Rabia" },
                Veterinario = new Usuario { Id = 1, Nombre = "Ana", Apellido = "Lopez", Email = "ana@eden.com", PasswordHash = "hash", Rol = "Veterinario" }
            }
        };
        _mockContext.Setup(c => c.Vacunas).ReturnsDbSet(listaVacunas);

        // ACT
        var (ok, mensaje, data) = await _service.GetVacunasPorAnimal(1);

        // ASSERT
        ok.Should().BeTrue();
        mensaje.Should().Be("OK");
        data.Should().NotBeNull();
    }
}