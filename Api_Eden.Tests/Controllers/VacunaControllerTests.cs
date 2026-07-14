using Api_Eden.Controllers;
using Api_Eden.DTOs.MedicoDto;
using Api_Eden.Services.TratamientoService.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using FluentAssertions;
using System.Security.Claims;

namespace Api_Eden.Tests.Controllers;

public class VacunaControllerTests
{
    private readonly Mock<IVacunaService> _mockVacunaService;
    private readonly VacunaController _controller;

    public VacunaControllerTests()
    {
        // 1. Inicializamos el mock del servicio (aquí sí es una interfaz real)
        _mockVacunaService = new Mock<IVacunaService>();

        // 2. Inyectamos en el controller
        _controller = new VacunaController(_mockVacunaService.Object);
    }

    [Fact]
    public async Task GetVacunasPorAnimal_ServicioNoEncuentraAnimal_RetornaNotFound()
    {
        // ARRANGE
        _mockVacunaService
            .Setup(s => s.GetVacunasPorAnimal(99))
            .ReturnsAsync((false, "Animal no encontrado.", (object?)null));

        // ACT
        var resultado = await _controller.GetVacunasPorAnimal(99);

        // ASSERT
        resultado.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task GetVacunasPorAnimal_ServicioRetornaDatos_RetornaOkConData()
    {
        // ARRANGE
        var datosVacunas = new List<object> { new { Id = 1, TipoVacuna = "Rabia" } };
        _mockVacunaService
            .Setup(s => s.GetVacunasPorAnimal(1))
            .ReturnsAsync((true, "OK", (object?)datosVacunas));

        // ACT
        var resultado = await _controller.GetVacunasPorAnimal(1);

        // ASSERT
        resultado.Should().BeOfType<OkObjectResult>();
        var okResult = resultado as OkObjectResult;
        okResult!.Value.Should().Be(datosVacunas);
    }

    [Fact]
    public async Task RegistrarVacuna_UsuarioNoAutenticado_RetornaUnauthorized()
    {
        // ARRANGE: usuario sin el claim NameIdentifier (no autenticado correctamente)
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity())
            }
        };

        var dto = new RegistrarVacunaDto(
            AnimalId: 1,
            TipoVacunaId: 1,
            FechaAplicacion: DateOnly.FromDateTime(DateTime.Today),
            ProximaDosis: null,
            Lote: null,
            VeterinarioId: 0,
            Observaciones: null);

        // ACT
        var resultado = await _controller.RegistrarVacuna(dto);

        // ASSERT
        resultado.Should().BeOfType<UnauthorizedObjectResult>();
        // El servicio nunca debió ser llamado, ya que la validación del usuario ocurre antes
        _mockVacunaService.Verify(s => s.RegistrarVacuna(It.IsAny<RegistrarVacunaDto>()), Times.Never);
    }
}