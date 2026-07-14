using Api_Eden.Controllers;
using Api_Eden.DTOs.GastosDto;
using Api_Eden.Services.GastosService.Interface;
using Microsoft.AspNetCore.Mvc;
using Moq;
using FluentAssertions;

namespace Api_Eden.Tests.Controllers;

public class GastoControllerTests
{
    private readonly Mock<IGastoService> _mockService;
    private readonly GastoController _controller;

    public GastoControllerTests()
    {
        // 1. Inicializamos el mock del servicio (interfaz real)
        _mockService = new Mock<IGastoService>();

        // 2. Inyectamos en el controller
        _controller = new GastoController(_mockService.Object);
    }

    [Fact]
    public async Task GetGastos_RetornaOkConLista()
    {
        // ARRANGE
        var listaGastos = new List<object> { new { Id = 1, Concepto = "Comida" } };
        _mockService.Setup(s => s.GetGastos()).ReturnsAsync(listaGastos);

        // ACT
        var resultado = await _controller.GetGastos();

        // ASSERT
        resultado.Should().BeOfType<OkObjectResult>();
        var okResult = resultado as OkObjectResult;
        okResult!.Value.Should().Be(listaGastos);
    }

    [Fact]
    public async Task GetGasto_ServicioNoEncuentraGasto_RetornaNotFound()
    {
        // ARRANGE
        _mockService
            .Setup(s => s.GetGasto(99))
            .ReturnsAsync((false, "Gasto no encontrado.", (object?)null));

        // ACT
        var resultado = await _controller.GetGasto(99);

        // ASSERT
        resultado.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task CrearGasto_ServicioRetornaFalse_RetornaBadRequest()
    {
        // ARRANGE
        var dto = new CrearGastoDto(
            CategoriaGastoId: 1,
            Concepto: "Compra de alimento",
            Monto: 100m,
            FechaGasto: DateOnly.FromDateTime(DateTime.Today),
            FormaPago: "Efectivo",
            NumeroFactura: null,
            NumeroTransaccion: null,
            NombreProveedor: null,
            TelefonoProveedor: null,
            AlimentoId: null,
            MedicamentoId: null,
            Observaciones: null,
            UsuarioRegistroId: 1);

        _mockService
            .Setup(s => s.CrearGasto(dto))
            .ReturnsAsync((false, "Categoría de gasto no encontrada."));

        // ACT
        var resultado = await _controller.CrearGasto(dto);

        // ASSERT
        resultado.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task EliminarGasto_ServicioRetornaFalse_RetornaNotFound()
    {
        // ARRANGE
        _mockService
            .Setup(s => s.EliminarGasto(99))
            .ReturnsAsync((false, "Gasto no encontrado."));

        // ACT
        var resultado = await _controller.EliminarGasto(99);

        // ASSERT
        resultado.Should().BeOfType<NotFoundObjectResult>();
    }
}