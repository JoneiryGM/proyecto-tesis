using Api_Eden.Controllers;
using Api_Eden.DTOs.DonacionesDto;
using Api_Eden.Services.DonacionesService.Interface;
using Microsoft.AspNetCore.Mvc;
using Moq;
using FluentAssertions;

namespace Api_Eden.Tests.Controllers;

public class DonacionControllerTests
{
    private readonly Mock<IDonacionService> _mockService;
    private readonly DonacionController _controller;

    public DonacionControllerTests()
    {
        // 1. Inicializamos el mock del servicio (interfaz real)
        _mockService = new Mock<IDonacionService>();

        // 2. Inyectamos en el controller
        _controller = new DonacionController(_mockService.Object);
    }

    [Fact]
    public async Task GetAll_RetornaOkConListaDeDonaciones()
    {
        // ARRANGE
        var listaDonaciones = new List<DonacionResponseDto>();
        _mockService.Setup(s => s.GetAllAsync()).ReturnsAsync(listaDonaciones);

        // ACT
        var resultado = await _controller.GetAll();

        // ASSERT
        resultado.Should().BeOfType<OkObjectResult>();
        var okResult = resultado as OkObjectResult;
        okResult!.Value.Should().Be(listaDonaciones);
    }

    [Fact]
    public async Task GetTipos_RetornaOkConListaDeTipos()
    {
        // ARRANGE
        var listaTipos = new List<object> { new { Id = 1, Nombre = "Dinero" } };
        _mockService.Setup(s => s.GetTiposAsync()).ReturnsAsync(listaTipos);

        // ACT
        var resultado = await _controller.GetTipos();

        // ASSERT
        resultado.Should().BeOfType<OkObjectResult>();
        var okResult = resultado as OkObjectResult;
        okResult!.Value.Should().Be(listaTipos);
    }

    [Fact]
    public async Task Eliminar_ServicioRetornaFalse_RetornaNotFound()
    {
        // ARRANGE
        _mockService
            .Setup(s => s.EliminarAsync(99))
            .ReturnsAsync((false, "Donación no encontrada."));

        // ACT
        var resultado = await _controller.Eliminar(99);

        // ASSERT
        resultado.Should().BeOfType<NotFoundObjectResult>();
    }
}