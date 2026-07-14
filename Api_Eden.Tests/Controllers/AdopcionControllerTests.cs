using Api_Eden.Controllers;
using Api_Eden.DTOs.AdopcionDto;
using Api_Eden.Services.AdopcionesService.Interface;
using Microsoft.AspNetCore.Mvc;
using Moq;
using FluentAssertions;

namespace Api_Eden.Tests.Controllers;

public class AdopcionControllerTests
{
    private readonly Mock<IAdopcionService> _mockService;
    private readonly AdopcionController _controller;

    public AdopcionControllerTests()
    {
        // 1. Inicializamos el mock del servicio (interfaz real)
        _mockService = new Mock<IAdopcionService>();

        // 2. Inyectamos en el controller
        _controller = new AdopcionController(_mockService.Object);
    }

    [Fact]
    public async Task GetAdopciones_RetornaOkConListaDeAdopciones()
    {
        // ARRANGE
        var listaAdopciones = new List<object> { new { Id = 1, NombreAdoptante = "Ana" } };
        _mockService.Setup(s => s.GetAdopciones()).ReturnsAsync(listaAdopciones);

        // ACT
        var resultado = await _controller.GetAdopciones();

        // ASSERT
        resultado.Should().BeOfType<OkObjectResult>();
        var okResult = resultado as OkObjectResult;
        okResult!.Value.Should().Be(listaAdopciones);
    }

    [Fact]
    public async Task GetAdopcion_ServicioNoEncuentraAdopcion_RetornaNotFound()
    {
        // ARRANGE
        _mockService
            .Setup(s => s.GetAdopcion(99))
            .ReturnsAsync((false, "Adopción no encontrada.", (object?)null));

        // ACT
        var resultado = await _controller.GetAdopcion(99);

        // ASSERT
        resultado.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task ActualizarEstado_ServicioRetornaFalse_RetornaBadRequest()
    {
        // ARRANGE
        var dto = new ActualizarEstadoAdopcionDto(
            Estado: "Aprobada",
            Observaciones: null);

        _mockService
            .Setup(s => s.ActualizarEstado(1, dto))
            .ReturnsAsync((false, "Adopción no encontrada."));

        // ACT
        var resultado = await _controller.ActualizarEstado(1, dto);

        // ASSERT
        resultado.Should().BeOfType<BadRequestObjectResult>();
    }
}