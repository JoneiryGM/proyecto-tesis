using Api_Eden.Controllers;
using Api_Eden.Services.Dashboard.Interface;
using Api_Eden.Services.DashboardService;
using Microsoft.AspNetCore.Mvc;
using Moq;
using FluentAssertions;

namespace Api_Eden.Tests.Controllers;

public class DashboardControllerTests
{
    private readonly Mock<IDashboardService> _mockService;
    private readonly DashboardController _controller;

    public DashboardControllerTests()
    {
        // 1. Inicializamos el mock del servicio (interfaz real)
        _mockService = new Mock<IDashboardService>();

        // 2. Inyectamos en el controller
        _controller = new DashboardController(_mockService.Object);
    }

    [Fact]
    public async Task GetResumen_RetornaOkConResumenDelServicio()
    {
        // ARRANGE
        var resumen = new DashboardResumenDto(
            new DashboardStatsDto(0, 0, 0, 0, 0, 0, 0, 0, 0, 0),
            new List<EstadoSaludDto>(),
            new List<ZonaOcupacionDto>(),
            new List<GastoMensualDto>(),
            new List<GastoCategoriaDto>(),
            new List<ActividadDto>());

        _mockService.Setup(s => s.GetResumenAsync()).ReturnsAsync(resumen);

        // ACT
        var resultado = await _controller.GetResumen();

        // ASSERT
        resultado.Should().BeOfType<OkObjectResult>();
        var okResult = resultado as OkObjectResult;
        okResult!.Value.Should().Be(resumen);
    }
}