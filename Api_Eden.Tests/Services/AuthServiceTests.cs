using Api_Eden.Data;
using Api_Eden.DTOs.AuthDto;
using Api_Eden.Models;
using Api_Eden.Services;
using Microsoft.Extensions.Configuration;
using Moq;
using Moq.EntityFrameworkCore; // <-- La librería que instalamos
using FluentAssertions;

namespace Api_Eden.Tests.Services;

public class AuthServiceTests
{
    private readonly Mock<AppDbContext> _mockContext;
    private readonly IConfiguration _config;
    private readonly AuthService _service;

    public AuthServiceTests()
    {
        // 1. Inicializamos el mock del contexto
        _mockContext = new Mock<AppDbContext>();

        // 2. Configuración vacía (ninguno de estos tests genera un JWT, así que no hace falta Jwt:Key)
        _config = new ConfigurationBuilder().Build();

        // 3. Inyectamos en el servicio
        _service = new AuthService(_mockContext.Object, _config);
    }

    [Fact]
    public async Task RegistrarAsync_EmailYaExiste_RetornaFalso()
    {
        // ARRANGE
        var listaUsuarios = new List<Usuario>
        {
            new Usuario
            {
                Id = 1,
                Nombre = "Juan",
                Apellido = "Perez",
                Email = "juan@eden.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123"),
                Rol = "Trabajador",
                Activo = true
            }
        };
        _mockContext.Setup(c => c.Usuarios).ReturnsDbSet(listaUsuarios);

        var dto = new RegistroDto("Juan", "Perez", "juan@eden.com", "Trabajador", "OtraPassword");

        // ACT
        var (exito, mensaje) = await _service.RegistrarAsync(dto);

        // ASSERT
        exito.Should().BeFalse();
        mensaje.Should().Be("Este Email ya existe");
    }

    [Fact]
    public async Task RegistrarAsync_UsuarioNuevo_RegistraConExito()
    {
        // ARRANGE: no hay usuarios previos con ese email
        var listaUsuarios = new List<Usuario>();
        _mockContext.Setup(c => c.Usuarios).ReturnsDbSet(listaUsuarios);

        var dto = new RegistroDto("Maria", "Gomez", "maria@eden.com", "Veterinario", "Password123");

        // ACT
        var (exito, mensaje) = await _service.RegistrarAsync(dto);

        // ASSERT
        exito.Should().BeTrue();
        mensaje.Should().Be("Usuario registrado exitosamente");
    }

    [Fact]
    public async Task LoginAsync_UsuarioNoExiste_RetornaNull()
    {
        // ARRANGE
        var listaVacia = new List<Usuario>();
        _mockContext.Setup(c => c.Usuarios).ReturnsDbSet(listaVacia);

        var dto = new LoginDto("noexiste@eden.com", "Password123");

        // ACT
        var resultado = await _service.LoginAsync(dto);

        // ASSERT
        resultado.Should().BeNull();
    }

    [Fact]
    public async Task ActualizarAsync_UsuarioNoExiste_RetornaFalso()
    {
        // ARRANGE
        var listaVacia = new List<Usuario>();
        _mockContext.Setup(c => c.Usuarios).ReturnsDbSet(listaVacia);

        var dto = new ActualizarUsuarioDto("NuevoNombre", null, null, null, null, null, null);

        // ACT
        var (exito, mensaje) = await _service.ActualizarAsync(99, dto);

        // ASSERT
        exito.Should().BeFalse();
        mensaje.Should().Be("Usuario no encontrado");
    }

    [Fact]
    public async Task EliminarAsync_MismoUsuario_RetornaFalso()
    {
        // ARRANGE: no hace falta preparar el DbSet, la validación ocurre antes

        // ACT: el usuario 1 intenta eliminarse a sí mismo
        var (exito, mensaje) = await _service.EliminarAsync(1, 1);

        // ASSERT
        exito.Should().BeFalse();
        mensaje.Should().Be("No puedes eliminar tu propio usuario");
    }

    [Fact]
    public async Task EliminarAsync_UsuarioNoExiste_RetornaFalso()
    {
        // ARRANGE
        var listaVacia = new List<Usuario>();
        _mockContext.Setup(c => c.Usuarios).ReturnsDbSet(listaVacia);

        // ACT
        var (exito, mensaje) = await _service.EliminarAsync(99, 1);

        // ASSERT
        exito.Should().BeFalse();
        mensaje.Should().Be("Usuario no encontrado");
    }
}