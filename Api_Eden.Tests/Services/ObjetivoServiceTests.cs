using Api_Eden.Data;
using Api_Eden.DTOs.ObjectivoDto;
using Api_Eden.Models;
using Api_Eden.Services.ObjetivoService;
using Moq;
using Moq.EntityFrameworkCore; // <-- La librería que instalamos
using FluentAssertions;

namespace Api_Eden.Tests.Services;

public class ObjetivoServiceTests
{
    private readonly Mock<AppDbContext> _mockContext;
    private readonly ObjetivoService _service;

    public ObjetivoServiceTests()
    {
        // 1. Inicializamos el mock del contexto
        _mockContext = new Mock<AppDbContext>();

        // 2. Inyectamos en el servicio
        _service = new ObjetivoService(_mockContext.Object);
    }

    [Fact]
    public async Task CrearAsync_MontoInvalido_RetornaFalso()
    {
        // ARRANGE: no hace falta preparar el DbSet, la validación ocurre antes de tocar el contexto
        var dto = new CrearObjetivoDto(
            Nombre: "Techo para el albergue",
            Descripcion: "Recaudación para reparar el techo",
            MontoObjetivo: 0,
            FechaLimite: null,
            Observaciones: null);

        // ACT
        var (ok, mensaje, id) = await _service.CrearAsync(dto, usuarioId: 1);

        // ASSERT
        ok.Should().BeFalse();
        mensaje.Should().Be("El monto objetivo debe ser mayor a 0.");
        id.Should().BeNull();
    }

    [Fact]
    public async Task CrearAsync_DatosValidos_CreaCorrectamente()
    {
        // ARRANGE
        var listaObjetivos = new List<Objetivo>();
        _mockContext.Setup(c => c.Objetivos).ReturnsDbSet(listaObjetivos);

        var dto = new CrearObjetivoDto(
            Nombre: "Techo para el albergue",
            Descripcion: "Recaudación para reparar el techo",
            MontoObjetivo: 5000m,
            FechaLimite: null,
            Observaciones: null);

        // ACT
        var (ok, mensaje, id) = await _service.CrearAsync(dto, usuarioId: 1);

        // ASSERT
        ok.Should().BeTrue();
        mensaje.Should().Be("Objetivo creado correctamente.");
        // El DbSet mockeado no sincroniza Add() con nuestra lista local, así que
        // verificamos con Moq que se intentó agregar el objetivo con los datos correctos.
        _mockContext.Verify(c => c.Objetivos.Add(It.Is<Objetivo>(o =>
            o.Nombre == "Techo para el albergue" && o.Estado == "Activo")), Times.Once);
    }

    [Fact]
    public async Task ActualizarAsync_ObjetivoNoExiste_RetornaFalso()
    {
        // ARRANGE: lista vacía, por lo que FindAsync no encontrará el objetivo
        var listaVacia = new List<Objetivo>();
        _mockContext.Setup(c => c.Objetivos).ReturnsDbSet(listaVacia);

        var dto = new ActualizarObjetivoDto(
            Nombre: "Nuevo nombre",
            Descripcion: null,
            MontoObjetivo: null,
            Estado: null,
            FechaLimite: null,
            Observaciones: null);

        // ACT
        var (ok, mensaje) = await _service.ActualizarAsync(99, dto);

        // ASSERT
        ok.Should().BeFalse();
        mensaje.Should().Be("Objetivo no encontrado.");
    }

    [Fact]
    public async Task EliminarAsync_ObjetivoNoExiste_RetornaFalso()
    {
        // ARRANGE: lista vacía, por lo que FindAsync no encontrará el objetivo
        var listaVacia = new List<Objetivo>();
        _mockContext.Setup(c => c.Objetivos).ReturnsDbSet(listaVacia);

        // ACT
        var (ok, mensaje) = await _service.EliminarAsync(99);

        // ASSERT
        ok.Should().BeFalse();
        mensaje.Should().Be("Objetivo no encontrado.");
    }
}