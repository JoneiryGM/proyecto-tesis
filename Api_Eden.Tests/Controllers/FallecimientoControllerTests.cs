using Api_Eden.Controllers;
using Api_Eden.DTOs.MedicoDto;
using Api_Eden.Services.TratamientoService.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using FluentAssertions;
using System.Security.Claims;

namespace Api_Eden.Tests.Controllers;

public class FallecimientoControllerTests
{
    private readonly Mock<IFallecimientoService> _mockService;
    private readonly FallecimientoController _controller;

    public FallecimientoControllerTests()
    {
        // 1. Inicializamos el mock del servicio (interfaz real)
        _mockService = new Mock<IFallecimientoService>();

        // 2. Inyectamos en el controller
        _controller = new FallecimientoController(_mockService.Object);
    }

    private static RegistrarFallecimientoDto CrearDto() => new(
        AnimalId: 1,
        FechaFallecimiento: DateOnly.FromDateTime(DateTime.Today),
        CausaFallecimiento: "Causa natural",
        VeterinarioId: 0,   // el controller lo sobrescribe con el usuario autenticado
        UsuarioRegistroId: 0,
        Observaciones: null);

    [Fact]
    public async Task RegistrarFallecimiento_UsuarioNoAutenticado_RetornaUnauthorized()
    {
        // ARRANGE: usuario sin el claim NameIdentifier (no identificable)
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity())
            }
        };

        // ACT
        var resultado = await _controller.RegistrarFallecimiento(CrearDto());

        // ASSERT
        resultado.Should().BeOfType<UnauthorizedObjectResult>();
        // El servicio nunca debió ser llamado, ya que la validación del usuario ocurre antes
        _mockService.Verify(s => s.RegistrarFallecimiento(It.IsAny<RegistrarFallecimientoDto>()), Times.Never);
    }

    [Fact]
    public async Task RegistrarFallecimiento_ServicioRetornaFalse_RetornaBadRequest()
    {
        // ARRANGE: usuario autenticado con id = 5
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(
                    new[] { new Claim(ClaimTypes.NameIdentifier, "5") }))
            }
        };

        _mockService
            .Setup(s => s.RegistrarFallecimiento(It.Is<RegistrarFallecimientoDto>(d =>
                d.VeterinarioId == 5 && d.UsuarioRegistroId == 5)))
            .ReturnsAsync((false, "Animal no encontrado."));

        // ACT
        var resultado = await _controller.RegistrarFallecimiento(CrearDto());

        // ASSERT
        resultado.Should().BeOfType<BadRequestObjectResult>();
    }
}