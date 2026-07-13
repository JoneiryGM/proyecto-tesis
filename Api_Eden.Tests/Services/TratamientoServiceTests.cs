using Api_Eden.Data;
using Api_Eden.DTOs.MedicoDto;
using Api_Eden.Models;
using Api_Eden.Services.TratamientoService;
using Moq;
using Moq.EntityFrameworkCore; // <-- La librería que instalamos
using FluentAssertions;

namespace Api_Eden.Tests.Services;

public class TratamientoServiceTests
{
    private readonly Mock<AppDbContext> _mockContext;
    private readonly TratamientoService _service;

    public TratamientoServiceTests()
    {
        // 1. Inicializamos el mock del contexto
        _mockContext = new Mock<AppDbContext>();

        // 2. Inyectamos en el servicio
        _service = new TratamientoService(_mockContext.Object);
    }

    [Fact]
    public async Task ActualizarEstadoTratamiento_EstadoInvalido_RetornaFalso()
    {
        // ARRANGE: no hace falta preparar el DbSet, la validación ocurre antes de tocar el contexto

        // ACT
        var (ok, mensaje) = await _service.ActualizarEstadoTratamiento(1, "EstadoQueNoExiste", veterinarioId: 1);

        // ASSERT
        ok.Should().BeFalse();
        mensaje.Should().Be("Estado inválido. Usa: Activo, Completado o Suspendido.");
    }

    [Fact]
    public async Task ActualizarEstadoTratamiento_TratamientoNoExiste_RetornaFalso()
    {
        // ARRANGE: lista vacía de tratamientos
        var listaVacia = new List<Tratamiento>();
        _mockContext.Setup(c => c.Tratamientos).ReturnsDbSet(listaVacia);

        // ACT
        var (ok, mensaje) = await _service.ActualizarEstadoTratamiento(99, "Activo", veterinarioId: 1);

        // ASSERT
        ok.Should().BeFalse();
        mensaje.Should().Be("Tratamiento no encontrado.");
    }

    [Fact]
    public async Task RegistrarTratamiento_HistorialNoExiste_RetornaFalso()
    {
        // ARRANGE: lista vacía de historiales médicos
        var listaHistorialesVacia = new List<Historialmedico>();
        _mockContext.Setup(c => c.Historialmedicos).ReturnsDbSet(listaHistorialesVacia);

        var dto = new RegistrarTratamientoDto
        {
            HistorialMedicoId = 99,
            MedicamentoId = 1,
            Dosis = "10ml",
            Frecuencia = "Cada 12 horas",
            ViaAdministracion = "Oral",
            FechaInicio = DateTime.Today,
            FechaFin = DateTime.Today.AddDays(7),
            VeterinarioId = 1,
            Observaciones = "Ninguna"
        };

        // ACT
        var (ok, mensaje, id) = await _service.RegistrarTratamiento(dto);

        // ASSERT
        ok.Should().BeFalse();
        mensaje.Should().Be("Historial médico no encontrado.");
        id.Should().BeNull();
    }
}