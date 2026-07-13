using Api_Eden.Data;
using Api_Eden.DTOs.MedicoDto;
using Api_Eden.Models;
using Api_Eden.Services.TratamientoService;
using Moq;
using Moq.EntityFrameworkCore; // <-- La librería que instalamos
using FluentAssertions;

namespace Api_Eden.Tests.Services;

public class MedicoServiceTests
{
    private readonly Mock<AppDbContext> _mockContext;
    private readonly MedicoService _service;

    public MedicoServiceTests()
    {
        // 1. Inicializamos el mock del contexto
        _mockContext = new Mock<AppDbContext>();

        // 2. Inyectamos en el servicio
        _service = new MedicoService(_mockContext.Object);
    }

    [Fact]
    public async Task GetMedicamentosAsync_RetornaSoloMedicamentosActivos()
    {
        // ARRANGE
        var listaMedicamentos = new List<Medicamento>
        {
            new Medicamento { Id = 1, Nombre = "Amoxicilina", Activo = true, FechaCreacion = DateTime.UtcNow },
            new Medicamento { Id = 2, Nombre = "Ibuprofeno", Activo = false, FechaCreacion = DateTime.UtcNow }
        };
        _mockContext.Setup(c => c.Medicamentos).ReturnsDbSet(listaMedicamentos);

        // ACT
        var resultado = await _service.GetMedicamentosAsync();

        // ASSERT
        resultado.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetVeterinariosAsync_RetornaSoloVeterinariosYAdministradoresActivos()
    {
        // ARRANGE
        var listaUsuarios = new List<Usuario>
        {
            new Usuario { Id = 1, Nombre = "Ana", Apellido = "Lopez", Email = "ana@eden.com", PasswordHash = "hash", Rol = "Veterinario", Activo = true },
            new Usuario { Id = 2, Nombre = "Luis", Apellido = "Diaz", Email = "luis@eden.com", PasswordHash = "hash", Rol = "Trabajador", Activo = true },
            new Usuario { Id = 3, Nombre = "Carlos", Apellido = "Ruiz", Email = "carlos@eden.com", PasswordHash = "hash", Rol = "Administrador", Activo = false }
        };
        _mockContext.Setup(c => c.Usuarios).ReturnsDbSet(listaUsuarios);

        // ACT
        var resultado = await _service.GetVeterinariosAsync();

        // ASSERT: solo Ana califica (Veterinario y Activo); Luis no es Veterinario/Admin, Carlos está inactivo
        resultado.Should().HaveCount(1);
    }

    [Fact]
    public async Task RegistrarHistorialAsync_AnimalNoExiste_RetornaFalso()
    {
        // ARRANGE: lista vacía, por lo que FindAsync no encontrará el animal
        var listaAnimalesVacia = new List<Animale>();
        _mockContext.Setup(c => c.Animales).ReturnsDbSet(listaAnimalesVacia);

        var dto = new RegistrarHistorialDto(
            AnimalId: 99,
            Diagnostico: "Chequeo general",
            Sintomas: null,
            Peso: null,
            Temperatura: null,
            VeterinarioId: 1,
            Observaciones: null);

        // ACT
        var (ok, mensaje, id) = await _service.RegistrarHistorialAsync(dto);

        // ASSERT
        ok.Should().BeFalse();
        mensaje.Should().Be("Animal no encontrado.");
        id.Should().BeNull();
    }

    [Fact]
    public async Task RegistrarHistorialAsync_VeterinarioSinPermisos_RetornaFalso()
    {
        // ARRANGE
        var listaAnimales = new List<Animale>
        {
            new Animale { Id = 1, Nombre = "Firulais", EspecieId = 1, FechaIngreso = DateOnly.FromDateTime(DateTime.Now) }
        };
        _mockContext.Setup(c => c.Animales).ReturnsDbSet(listaAnimales);
        _mockContext.Setup(c => c.Animales.FindAsync(1)).ReturnsAsync(listaAnimales.First(a => a.Id == 1));

        var listaUsuarios = new List<Usuario>
        {
            new Usuario { Id = 5, Nombre = "Luis", Apellido = "Diaz", Email = "luis@eden.com", PasswordHash = "hash", Rol = "Trabajador", Activo = true }
        };
        _mockContext.Setup(c => c.Usuarios).ReturnsDbSet(listaUsuarios);
        _mockContext.Setup(c => c.Usuarios.FindAsync(5)).ReturnsAsync(listaUsuarios.First(u => u.Id == 5));

        var dto = new RegistrarHistorialDto(
            AnimalId: 1,
            Diagnostico: "Chequeo general",
            Sintomas: null,
            Peso: null,
            Temperatura: null,
            VeterinarioId: 5,
            Observaciones: null);

        // ACT
        var (ok, mensaje, id) = await _service.RegistrarHistorialAsync(dto);

        // ASSERT
        ok.Should().BeFalse();
        mensaje.Should().Be("El usuario responsable no existe o no tiene permisos.");
        id.Should().BeNull();
    }

    [Fact]
    public async Task CrearMedicamentoAsync_NombreVacio_RetornaFalso()
    {
        // ARRANGE: no hace falta preparar el DbSet, la validación ocurre antes de tocar el contexto

        // ACT
        var (ok, data) = await _service.CrearMedicamentoAsync("   ");

        // ASSERT
        ok.Should().BeFalse();
        data.Should().BeNull();
    }

    [Fact]
    public async Task CrearMedicamentoAsync_MedicamentoYaExiste_RetornaElExistente()
    {
        // ARRANGE
        var listaMedicamentos = new List<Medicamento>
        {
            new Medicamento { Id = 1, Nombre = "Amoxicilina", Activo = true, FechaCreacion = DateTime.UtcNow }
        };
        _mockContext.Setup(c => c.Medicamentos).ReturnsDbSet(listaMedicamentos);

        // ACT: mismo nombre pero con distinto casing/espacios
        var (ok, data) = await _service.CrearMedicamentoAsync("  amoxicilina  ");

        // ASSERT
        ok.Should().BeTrue();
        data.Should().NotBeNull();
        listaMedicamentos.Should().HaveCount(1); // no se agregó un duplicado
    }
}