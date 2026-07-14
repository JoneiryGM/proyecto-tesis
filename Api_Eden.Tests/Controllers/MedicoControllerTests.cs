using Api_Eden.Controllers;
using Api_Eden.DTOs.MedicoDto;
using Api_Eden.Services.TratamientoService.Interface;
using Microsoft.AspNetCore.Mvc;
using Moq;
using FluentAssertions;

namespace Api_Eden.Tests.Controllers;

public class MedicoControllerTests
{
    private readonly Mock<IMedicoService> _mockMedicoService;
    private readonly Mock<ITratamientoService> _mockTratamientoService;
    private readonly MedicoController _controller;

    public MedicoControllerTests()
    {
        // 1. Inicializamos los mocks de ambas interfaces
        _mockMedicoService = new Mock<IMedicoService>();
        _mockTratamientoService = new Mock<ITratamientoService>();

        // 2. Inyectamos en el controller
        _controller = new MedicoController(_mockMedicoService.Object, _mockTratamientoService.Object);
    }

    [Fact]
    public async Task GetMedicamentos_RetornaOkConLista()
    {
        // ARRANGE
        var listaMedicamentos = new List<object> { new { Id = 1, Nombre = "Amoxicilina" } };
        _mockMedicoService.Setup(s => s.GetMedicamentosAsync()).ReturnsAsync(listaMedicamentos);

        // ACT
        var resultado = await _controller.GetMedicamentos();

        // ASSERT
        resultado.Should().BeOfType<OkObjectResult>();
        var okResult = resultado as OkObjectResult;
        okResult!.Value.Should().Be(listaMedicamentos);
    }

    [Fact]
    public async Task CrearMedicamento_NombreVacio_RetornaBadRequest()
    {
        // ARRANGE: la validación ocurre en el propio controller, antes de llamar al servicio
        var dto = new CrearMedicamentoDto(
            Nombre: "   ",
            PrincipioActivo: null,
            Presentacion: null,
            Concentracion: null,
            Fabricante: null,
            Indicaciones: null,
            Contraindicaciones: null,
            EfectosSecundarios: null,
            RequiereReceta: false);

        // ACT
        var resultado = await _controller.CrearMedicamento(dto);

        // ASSERT
        resultado.Should().BeOfType<BadRequestObjectResult>();
        _mockMedicoService.Verify(s => s.CrearMedicamentoAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task RegistrarHistorial_ServicioRetornaFalse_RetornaBadRequest()
    {
        // ARRANGE
        var dto = new RegistrarHistorialDto(
            AnimalId: 1,
            Diagnostico: "Chequeo general",
            Sintomas: null,
            Peso: null,
            Temperatura: null,
            VeterinarioId: 1,
            Observaciones: null);

        _mockMedicoService
            .Setup(s => s.RegistrarHistorialAsync(dto))
            .ReturnsAsync((false, "Animal no encontrado.", (int?)null));

        // ACT
        var resultado = await _controller.RegistrarHistorial(dto);

        // ASSERT
        resultado.Should().BeOfType<BadRequestObjectResult>();
    }
}