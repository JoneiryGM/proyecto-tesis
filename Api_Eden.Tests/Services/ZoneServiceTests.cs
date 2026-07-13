using Api_Eden.Data;
using Api_Eden.Models;
using Api_Eden.Services;
using Api_Eden.DTOs.Zone.Request;
using Moq;
using Moq.EntityFrameworkCore; // <-- La librería que instalamos
using Microsoft.Extensions.Logging;
using FluentAssertions;

namespace Api_Eden.Tests.Services;

public class ZoneServiceTests
{
    private readonly Mock<AppDbContext> _mockContext;
    private readonly Mock<ILogger<ZoneService>> _mockLogger;
    private readonly ZoneService _service;

    public ZoneServiceTests()
    {
        // 1. Inicializamos los mocks básicos
        _mockContext = new Mock<AppDbContext>();
        _mockLogger = new Mock<ILogger<ZoneService>>();

        // 2. Inyectamos en el servicio
        _service = new ZoneService(_mockContext.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GetAllAsync_RetornaListaDeZonas()
    {
        // ARRANGE: Preparamos los datos simulados para el albergue El Edén
        var listaZonas = new List<Zona>
        {
            new Zona 
            { 
                Id = 1, 
                Nombre = "Cuarentena Felina", 
                Activa = true, 
                CapacidadMaxima = 10,
                Animales = new List<Animale>() 
            },
            new Zona 
            { 
                Id = 2, 
                Nombre = "Patio Canino", 
                Activa = true, 
                CapacidadMaxima = 20,
                Animales = new List<Animale>() 
            }
        };

        // ¡Magia! Configuramos el DbSet simulado en una sola línea
        _mockContext.Setup(c => c.Zonas).ReturnsDbSet(listaZonas);

        // ACT: Ejecutamos el método
        var resultado = await _service.GetAllAsync();

        // ASSERT: Verificamos con FluentAssertions
        resultado.Should().NotBeNull();
        resultado.Should().HaveCount(2);
        resultado.First().Name.Should().Be("Cuarentena Felina");
    }

    [Fact]
    public async Task GetByIdAsync_ZonaNoExiste_LanzaKeyNotFoundException()
    {
        // ARRANGE: Lista vacía
        var listaVacia = new List<Zona>();
        _mockContext.Setup(c => c.Zonas).ReturnsDbSet(listaVacia);

        // ACT & ASSERT: Verificamos que lance la excepción correcta
        var accion = async () => await _service.GetByIdAsync(99);

        await accion.Should().ThrowAsync<KeyNotFoundException>()
                    .WithMessage("No existe una zona con ID 99.");
    }

     [Fact]
    public async Task CreateAsync_NombreDuplicado_LanzaArgumentException()
    {
        // ARRANGE: ya existe una zona con el mismo nombre
        var listaZonas = new List<Zona>
        {
            new Zona
            {
                Id = 1,
                Nombre = "Cuarentena Felina",
                Activa = true,
                CapacidadMaxima = 10,
                Animales = new List<Animale>()
            }
        };
        _mockContext.Setup(c => c.Zonas).ReturnsDbSet(listaZonas);

        var dto = new CreateZoneDto
        {
            Nombre = "Cuarentena Felina",
            CapacidadMaxima = 15
        };

        // ACT
        var accion = async () => await _service.CreateAsync(dto);

        // ASSERT
        await accion.Should().ThrowAsync<ArgumentException>()
                    .WithMessage("Ya existe una zona con el nombre 'Cuarentena Felina'.");
    }

    [Fact]
    public async Task DeleteAsync_ZonaNoExiste_RetornaFalse()
    {
        // ARRANGE: lista vacía de zonas
        var listaVacia = new List<Zona>();
        _mockContext.Setup(c => c.Zonas).ReturnsDbSet(listaVacia);

        // ACT
        var resultado = await _service.DeleteAsync(99);

        // ASSERT
        resultado.Should().BeFalse();
    }

    [Fact]
    public async Task CreateAsync_NombreVacio_LanzaArgumentException()
    {
        // ARRANGE
        var dto = new CreateZoneDto
        {
            Nombre = "",
            CapacidadMaxima = 10
        };

        // ACT
        var accion = async () => await _service.CreateAsync(dto);

        // ASSERT
        await accion.Should().ThrowAsync<ArgumentException>()
            .WithMessage("El nombre de la zona es obligatorio.");
    }

    [Fact]
    public async Task GetAnimalesByZonaAsync_ZonaNoExiste_LanzaKeyNotFoundException()
    {
        // ARRANGE: lista vacía de zonas, por lo que FindAsync devolverá null
        var listaVacia = new List<Zona>();
        _mockContext.Setup(c => c.Zonas).ReturnsDbSet(listaVacia);

        // ACT
        var accion = async () => await _service.GetAnimalesByZonaAsync(99);

        // ASSERT
        await accion.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("No existe una zona con ID 99.");
    }
}