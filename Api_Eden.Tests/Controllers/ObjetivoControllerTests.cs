using Api_Eden.Controllers;
using Api_Eden.DTOs.ObjectivoDto;
using Api_Eden.Services.ObjetivoService.Interface;
using Microsoft.AspNetCore.Mvc;
using Moq;
using FluentAssertions;

namespace Api_Eden.Tests.Controllers;

public class ObjetivoControllerTests
{
    private readonly Mock<IObjetivoService> _mockService;
    private readonly ObjetivoController _controller;

    public ObjetivoControllerTests()
    {
        // 1. Inicializamos el mock del servicio (interfaz real)
        _mockService = new Mock<IObjetivoService>();

        // 2. Inyectamos en el controller
        _controller = new ObjetivoController(_mockService.Object);
    }

    [Fact]
    public async Task GetAll_RetornaOkConListaDeObjetivos()
    {
        // ARRANGE
        var listaObjetivos = new List<ObjetivoResponseDto>();
        _mockService.Setup(s => s.GetAllAsync()).ReturnsAsync(listaObjetivos);

        // ACT
        var resultado = await _controller.GetAll();

        // ASSERT
        resultado.Should().BeOfType<OkObjectResult>();
        var okResult = resultado as OkObjectResult;
        okResult!.Value.Should().Be(listaObjetivos);
    }

    [Fact]
    public async Task Actualizar_ObjetivoNoExiste_RetornaNotFound()
    {
        // ARRANGE
        _mockService
            .Setup(s => s.ActualizarAsync(99, It.IsAny<ActualizarObjetivoDto>()))
            .ReturnsAsync((false, "Objetivo no encontrado."));

        var dto = new ActualizarObjetivoDto(
            Nombre: "Nuevo nombre",
            Descripcion: null,
            MontoObjetivo: null,
            Estado: null,
            FechaLimite: null,
            Observaciones: null);

        // ACT
        var resultado = await _controller.Actualizar(99, dto);

        // ASSERT
        resultado.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task Eliminar_ObjetivoNoExiste_RetornaNotFound()
    {
        // ARRANGE
        _mockService
            .Setup(s => s.EliminarAsync(99))
            .ReturnsAsync((false, "Objetivo no encontrado."));

        // ACT
        var resultado = await _controller.Eliminar(99);

        // ASSERT
        resultado.Should().BeOfType<NotFoundObjectResult>();
    }
}