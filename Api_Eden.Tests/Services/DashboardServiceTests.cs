using Api_Eden.Data;
using Api_Eden.Models;
using Api_Eden.Services.DashboardService;
using Moq;
using Moq.EntityFrameworkCore; // <-- La librería que instalamos
using FluentAssertions;

namespace Api_Eden.Tests.Services;

public class DashboardServiceTests
{
    private readonly Mock<AppDbContext> _mockContext;
    private readonly DashboardService _service;

    public DashboardServiceTests()
    {
        // 1. Inicializamos el mock del contexto
        _mockContext = new Mock<AppDbContext>();

        // 2. Inyectamos en el servicio
        _service = new DashboardService(_mockContext.Object);
    }

    [Fact]
    public async Task GetResumenAsync_SinDatos_RetornaEstructuraVacia()
    {
        // ARRANGE: GetResumenAsync toca 6 tablas sin condición
        // (Animales, Zonas, Adopciones, Tratamientos, Alimentos, Gastos),
        // así que todas deben quedar configuradas, aunque sea vacías.
        _mockContext.Setup(c => c.Animales).ReturnsDbSet(new List<Animale>());
        _mockContext.Setup(c => c.Zonas).ReturnsDbSet(new List<Zona>());
        _mockContext.Setup(c => c.Adopciones).ReturnsDbSet(new List<Adopcione>());
        _mockContext.Setup(c => c.Tratamientos).ReturnsDbSet(new List<Tratamiento>());
        _mockContext.Setup(c => c.Alimentos).ReturnsDbSet(new List<Alimento>());
        _mockContext.Setup(c => c.Gastos).ReturnsDbSet(new List<Gasto>());

        // ACT
        var resultado = await _service.GetResumenAsync();

        // ASSERT
        resultado.Should().NotBeNull();
        resultado.Stats.Saludables.Should().Be(0);
        resultado.Stats.EnTratamiento.Should().Be(0);
        resultado.Stats.Criticos.Should().Be(0);
        resultado.Stats.Recuperados.Should().Be(0);
        resultado.Zonas.Should().BeEmpty();
        resultado.GastosMensuales.Should().BeEmpty();
        resultado.ActividadReciente.Should().BeEmpty();
    }
}