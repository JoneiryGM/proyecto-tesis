using Api_Eden.Data;
using Api_Eden.Models;
using Api_Eden.Services;
using Moq;
using Moq.EntityFrameworkCore; // <-- La librería que instalamos
using FluentAssertions;

namespace Api_Eden.Tests.Services;

public class AnimalServiceTests
{
    private readonly Mock<AppDbContext> _mockContext;
    private readonly AnimalService _service;

    public AnimalServiceTests()
    {
        // 1. Inicializamos el mock del contexto
        _mockContext = new Mock<AppDbContext>();

        // 2. Inyectamos en el servicio
        _service = new AnimalService(_mockContext.Object);
    }

    [Fact]
    public async Task GetAllAsync_RetornaListaDeAnimales()
    {
        // ARRANGE
        var listaAnimales = new List<Animale>
        {
            new Animale
            {
                Id = 1,
                Nombre = "Firulais",
                EspecieId = 1,
                FechaIngreso = DateOnly.FromDateTime(DateTime.Now),
                EstadoGeneral = "Activo",
                EstadoSalud = "Sano"
            },
            new Animale
            {
                Id = 2,
                Nombre = "Michi",
                EspecieId = 2,
                FechaIngreso = DateOnly.FromDateTime(DateTime.Now),
                EstadoGeneral = "Activo",
                EstadoSalud = "Sano"
            }
        };
        _mockContext.Setup(c => c.Animales).ReturnsDbSet(listaAnimales);

        // ACT
        var resultado = await _service.GetAllAsync();

        // ASSERT
        resultado.Should().NotBeNull();
        resultado.Should().HaveCount(2);
        resultado.First().Nombre.Should().Be("Firulais");
    }

    [Fact]
    public async Task GetByIdAsync_AnimalNoExiste_RetornaNull()
    {
        // ARRANGE
        var listaVacia = new List<Animale>();
        _mockContext.Setup(c => c.Animales).ReturnsDbSet(listaVacia);

        // ACT
        var resultado = await _service.GetByIdAsync(99);

        // ASSERT
        resultado.Should().BeNull();
    }

    [Fact]
    public async Task GetByIdAsync_AnimalExiste_RetornaDto()
    {
        // ARRANGE
        var listaAnimales = new List<Animale>
        {
            new Animale
            {
                Id = 1,
                Nombre = "Firulais",
                EspecieId = 1,
                FechaIngreso = DateOnly.FromDateTime(DateTime.Now),
                EstadoGeneral = "Activo",
                EstadoSalud = "Sano"
            }
        };
        _mockContext.Setup(c => c.Animales).ReturnsDbSet(listaAnimales);

        // ACT
        var resultado = await _service.GetByIdAsync(1);

        // ASSERT
        resultado.Should().NotBeNull();
        resultado!.Nombre.Should().Be("Firulais");
    }

    [Fact]
    public async Task DeleteAsync_AnimalNoExiste_RetornaFalse()
    {
        // ARRANGE
        var listaVacia = new List<Animale>();
        _mockContext.Setup(c => c.Animales).ReturnsDbSet(listaVacia);

        // ACT
        var resultado = await _service.DeleteAsync(99);

        // ASSERT
        resultado.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteAsync_AnimalExiste_EliminaConExito()
    {
        // ARRANGE
        var listaAnimales = new List<Animale>
        {
            new Animale
            {
                Id = 1,
                Nombre = "Firulais",
                EspecieId = 1,
                FechaIngreso = DateOnly.FromDateTime(DateTime.Now),
                EstadoGeneral = "Activo",
                EstadoSalud = "Sano"
            }
        };
        _mockContext.Setup(c => c.Animales).ReturnsDbSet(listaAnimales);
        // FindAsync necesita metadatos del modelo de EF que no existen en un contexto mockeado,
        // así que lo configuramos explícitamente para que "encuentre" el animal por su Id.
        _mockContext.Setup(c => c.Animales.FindAsync(1)).ReturnsAsync(listaAnimales.First(a => a.Id == 1));

        // ACT
        var resultado = await _service.DeleteAsync(1);

        // ASSERT
        resultado.Should().BeTrue();
    }

    [Fact]
    public async Task ActualizarEstadoAsync_AnimalNoExiste_RetornaFalse()
    {
        // ARRANGE
        var listaVacia = new List<Animale>();
        _mockContext.Setup(c => c.Animales).ReturnsDbSet(listaVacia);

        // ACT
        var resultado = await _service.ActualizarEstadoAsync(99, "Inactivo", "Enfermo");

        // ASSERT
        resultado.Should().BeFalse();
    }

    [Fact]
    public async Task ActualizarEstadoAsync_AnimalExiste_ActualizaEstadosCorrectamente()
    {
        // ARRANGE
        var listaAnimales = new List<Animale>
        {
            new Animale
            {
                Id = 1,
                Nombre = "Firulais",
                EspecieId = 1,
                FechaIngreso = DateOnly.FromDateTime(DateTime.Now),
                EstadoGeneral = "Activo",
                EstadoSalud = "Sano"
            }
        };
        _mockContext.Setup(c => c.Animales).ReturnsDbSet(listaAnimales);
        // FindAsync necesita metadatos del modelo de EF que no existen en un contexto mockeado,
        // así que lo configuramos explícitamente para que "encuentre" el animal por su Id.
        _mockContext.Setup(c => c.Animales.FindAsync(1)).ReturnsAsync(listaAnimales.First(a => a.Id == 1));

        // ACT
        var resultado = await _service.ActualizarEstadoAsync(1, "Adoptado", "Recuperado");

        // ASSERT
        resultado.Should().BeTrue();
        listaAnimales.First(a => a.Id == 1).EstadoGeneral.Should().Be("Adoptado");
        listaAnimales.First(a => a.Id == 1).EstadoSalud.Should().Be("Recuperado");
    }
}