using Api_Eden.Data;
using Api_Eden.DTOs.AdopcionDto;
using Api_Eden.Models;
using Api_Eden.Services.AdopcionesService;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.EntityFrameworkCore; // <-- La librería que instalamos
using FluentAssertions;

namespace Api_Eden.Tests.Services;

public class AdopcionServiceTests
{
    private readonly Mock<AppDbContext> _mockContext;
    private readonly Mock<ILogger<AdopcionService>> _mockLogger;
    private readonly AdopcionService _service;

    public AdopcionServiceTests()
    {
        // 1. Inicializamos los mocks básicos
        _mockContext = new Mock<AppDbContext>();
        _mockLogger = new Mock<ILogger<AdopcionService>>();

        // 2. Inyectamos en el servicio
        _service = new AdopcionService(_mockContext.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GetAdopcion_AdopcionNoExiste_RetornaFalso()
    {
        // ARRANGE
        var listaVacia = new List<Adopcione>();
        _mockContext.Setup(c => c.Adopciones).ReturnsDbSet(listaVacia);

        // ACT
        var (ok, mensaje, data) = await _service.GetAdopcion(99);

        // ASSERT
        ok.Should().BeFalse();
        mensaje.Should().Be("Adopción no encontrada.");
        data.Should().BeNull();
    }

    [Fact]
    public async Task ActualizarEstado_EstadoInvalido_RetornaFalso()
    {
        // ARRANGE: no hace falta preparar el DbSet, la validación ocurre antes de tocar el contexto
        var dto = new ActualizarEstadoAdopcionDto(
            Estado: "EstadoQueNoExiste",
            Observaciones: null);

        // ACT
        var (ok, mensaje) = await _service.ActualizarEstado(1, dto);

        // ASSERT
        ok.Should().BeFalse();
        mensaje.Should().Be("Estado inválido. Usa: Pendiente, Aprobada, Rechazada o Devuelto.");
    }

    [Fact]
    public async Task ActualizarEstado_AdopcionNoExiste_RetornaFalso()
    {
        // ARRANGE
        var listaVacia = new List<Adopcione>();
        _mockContext.Setup(c => c.Adopciones).ReturnsDbSet(listaVacia);

        var dto = new ActualizarEstadoAdopcionDto(
            Estado: "Aprobada",
            Observaciones: null);

        // ACT
        var (ok, mensaje) = await _service.ActualizarEstado(99, dto);

        // ASSERT
        ok.Should().BeFalse();
        mensaje.Should().Be("Adopción no encontrada.");
    }
}