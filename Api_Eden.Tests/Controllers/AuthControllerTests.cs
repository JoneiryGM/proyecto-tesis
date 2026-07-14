using Api_Eden.Controllers;
using Api_Eden.Data;
using Api_Eden.DTOs.AuthDto;
using Api_Eden.Models;
using Api_Eden.Services.EmailService.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using Moq.EntityFrameworkCore; // <-- La librería que instalamos
using FluentAssertions;

namespace Api_Eden.Tests.Controllers;

public class AuthControllerTests
{
    private readonly Mock<AppDbContext> _mockContext;
    private readonly IConfiguration _config;
    private readonly Mock<IEmailService> _mockEmailService;
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        // 1. Inicializamos los mocks básicos
        _mockContext = new Mock<AppDbContext>();
        _config = new ConfigurationBuilder().Build(); // ninguno de estos tests genera un JWT
        _mockEmailService = new Mock<IEmailService>();

        // 2. Inyectamos en el controller
        _controller = new AuthController(_mockContext.Object, _config, _mockEmailService.Object);
    }

    [Fact]
    public async Task Registro_EmailYaExiste_RetornaBadRequest()
    {
        // ARRANGE
        var listaUsuarios = new List<Usuario>
        {
            new Usuario { Id = 1, Nombre = "Juan", Apellido = "Perez", Email = "juan@eden.com", PasswordHash = "hash", Rol = "Trabajador", Activo = true }
        };
        _mockContext.Setup(c => c.Usuarios).ReturnsDbSet(listaUsuarios);

        var dto = new RegistroDto("Juan", "Perez", "juan@eden.com", "Trabajador", "OtraPassword");

        // ACT
        var resultado = await _controller.Registro(dto);

        // ASSERT
        resultado.Should().BeOfType<BadRequestObjectResult>();
        // El email no se debió intentar enviar, ya que la validación corta el flujo antes
        _mockEmailService.Verify(
            s => s.EnviarActivacionAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task VerificarToken_TokenInvalido_RetornaBadRequest()
    {
        // ARRANGE: ningún usuario tiene ese token de activación
        var listaVacia = new List<Usuario>();
        _mockContext.Setup(c => c.Usuarios).ReturnsDbSet(listaVacia);

        // ACT
        var resultado = await _controller.VerificarToken("token-invalido");

        // ASSERT
        resultado.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Actualizar_UsuarioNoExiste_RetornaNotFound()
    {
        // ARRANGE
        var listaVacia = new List<Usuario>();
        _mockContext.Setup(c => c.Usuarios).ReturnsDbSet(listaVacia);

        var dto = new ActualizarUsuarioDto("NuevoNombre", null, null, null, null, null, null);

        // ACT
        var resultado = await _controller.Actualizar(99, dto);

        // ASSERT
        resultado.Should().BeOfType<NotFoundObjectResult>();
    }
}