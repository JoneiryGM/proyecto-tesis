using Api_Eden.Controllers;
using Api_Eden.Data;
using Api_Eden.DTOs.Zone.Request;
using Api_Eden.Models;
using Api_Eden.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.EntityFrameworkCore; // <-- La librería que instalamos
using FluentAssertions;

namespace Api_Eden.Tests.Controllers;

public class ZoneControllerTests
{
    private readonly Mock<AppDbContext> _mockContext;
    private readonly Mock<ILogger<ZoneService>> _mockLogger;
    private readonly ZoneController _controller;

    public ZoneControllerTests()
    {
        // 1. Inicializamos los mocks básicos
        _mockContext = new Mock<AppDbContext>();
        _mockLogger = new Mock<ILogger<ZoneService>>();

        // 2. ZoneService no tiene interfaz y sus métodos no son virtuales,
        //    así que usamos una instancia REAL de ZoneService (respaldada por el
        //    AppDbContext mockeado) y se la inyectamos al controller real.
        var zoneService = new ZoneService(_mockContext.Object, _mockLogger.Object);
        _controller = new ZoneController(zoneService);
    }

    [Fact]
    public async Task GetAll_RetornaOkConListaDeZonas()
    {
        // ARRANGE
        var listaZonas = new List<Zona>
        {
            new Zona { Id = 1, Nombre = "Cuarentena Felina", Activa = true, CapacidadMaxima = 10, Animales = new List<Animale>() },
            new Zona { Id = 2, Nombre = "Patio Canino", Activa = true, CapacidadMaxima = 20, Animales = new List<Animale>() }
        };
        _mockContext.Setup(c => c.Zonas).ReturnsDbSet(listaZonas);

        // ACT
        var resultado = await _controller.GetAll();

        // ASSERT
        resultado.Result.Should().BeOfType<OkObjectResult>();
        var okResult = resultado.Result as OkObjectResult;
        okResult!.Value.Should().BeAssignableTo<IEnumerable<object>>();
    }

    [Fact]
    public async Task GetById_ZonaNoExiste_RetornaNotFound()
    {
        // ARRANGE: lista vacía, por lo que ZoneService.GetByIdAsync lanzará KeyNotFoundException
        var listaVacia = new List<Zona>();
        _mockContext.Setup(c => c.Zonas).ReturnsDbSet(listaVacia);

        // ACT
        var resultado = await _controller.GetById(99);

        // ASSERT
        resultado.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task Create_NombreVacio_RetornaBadRequest()
    {
        // ARRANGE: no hace falta preparar el DbSet, la validación ocurre antes de tocar el contexto
        var dto = new CreateZoneDto
        {
            Nombre = "",
            Descripcion = null,
            CapacidadMaxima = 10,
            CantidadActual = 0,
            Activa = true
        };

        // ACT
        var resultado = await _controller.Create(dto);

        // ASSERT
        resultado.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Delete_ZonaNoExiste_RetornaNotFound()
    {
        // ARRANGE: lista vacía, por lo que ZoneService.DeleteAsync retornará false
        var listaVacia = new List<Zona>();
        _mockContext.Setup(c => c.Zonas).ReturnsDbSet(listaVacia);

        // ACT
        var resultado = await _controller.Delete(99);

        // ASSERT
        resultado.Should().BeOfType<NotFoundObjectResult>();
    }
}