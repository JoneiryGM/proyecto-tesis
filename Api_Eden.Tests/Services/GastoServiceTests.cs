using Api_Eden.Data;
using Api_Eden.DTOs.GastosDto;
using Api_Eden.Models;
using Api_Eden.Services.GastosService;
using Moq;
using Moq.EntityFrameworkCore; // <-- La librería que instalamos
using FluentAssertions;

namespace Api_Eden.Tests.Services;

public class GastoServiceTests
{
    private readonly Mock<AppDbContext> _mockContext;
    private readonly GastoService _service;

    public GastoServiceTests()
    {
        // 1. Inicializamos el mock del contexto
        _mockContext = new Mock<AppDbContext>();

        // 2. Inyectamos en el servicio
        _service = new GastoService(_mockContext.Object);
    }

    [Fact]
    public async Task GetGasto_GastoNoExiste_RetornaFalso()
    {
        // ARRANGE
        var listaVacia = new List<Gasto>();
        _mockContext.Setup(c => c.Gastos).ReturnsDbSet(listaVacia);

        // ACT
        var (ok, mensaje, data) = await _service.GetGasto(99);

        // ASSERT
        ok.Should().BeFalse();
        mensaje.Should().Be("Gasto no encontrado.");
        data.Should().BeNull();
    }

    [Fact]
    public async Task GetCategorias_RetornaSoloCategoriasActivas()
    {
        // ARRANGE
        var listaCategorias = new List<Categoriasgasto>
        {
            new Categoriasgasto { Id = 1, Nombre = "Alimentación", Activa = true },
            new Categoriasgasto { Id = 2, Nombre = "Descontinuada", Activa = false }
        };
        _mockContext.Setup(c => c.Categoriasgastos).ReturnsDbSet(listaCategorias);

        // ACT
        var resultado = (await _service.GetCategorias()) as IEnumerable<object>;

        // ASSERT
        resultado.Should().NotBeNull();
        resultado!.Should().HaveCount(1);
    }

    [Fact]
    public async Task ActualizarGasto_GastoNoExiste_RetornaFalso()
    {
        // ARRANGE
        var listaVacia = new List<Gasto>();
        _mockContext.Setup(c => c.Gastos).ReturnsDbSet(listaVacia);

        var dto = new ActualizarGastoDto(
            Concepto: "Compra de alimento",
            Monto: null,
            FormaPago: null,
            NumeroFactura: null,
            NumeroTransaccion: null,
            NombreProveedor: null,
            TelefonoProveedor: null,
            Observaciones: null);

        // ACT
        var (ok, mensaje) = await _service.ActualizarGasto(99, dto);

        // ASSERT
        ok.Should().BeFalse();
        mensaje.Should().Be("Gasto no encontrado.");
    }

    [Fact]
    public async Task ActualizarGasto_MontoInvalido_RetornaFalso()
    {
        // ARRANGE
        var listaGastos = new List<Gasto>
        {
            new Gasto { Id = 1, CategoriaGastoId = 1, Concepto = "Comida", Monto = 100m, FechaGasto = DateOnly.FromDateTime(DateTime.Today), FormaPago = "Efectivo" }
        };
        _mockContext.Setup(c => c.Gastos).ReturnsDbSet(listaGastos);
        _mockContext.Setup(c => c.Gastos.FindAsync(1)).ReturnsAsync(listaGastos.First(g => g.Id == 1));

        var dto = new ActualizarGastoDto(
            Concepto: null,
            Monto: -50m,
            FormaPago: null,
            NumeroFactura: null,
            NumeroTransaccion: null,
            NombreProveedor: null,
            TelefonoProveedor: null,
            Observaciones: null);

        // ACT
        var (ok, mensaje) = await _service.ActualizarGasto(1, dto);

        // ASSERT
        ok.Should().BeFalse();
        mensaje.Should().Be("El monto debe ser mayor a 0.");
    }

    [Fact]
    public async Task ActualizarGasto_FormaPagoInvalida_RetornaFalso()
    {
        // ARRANGE
        var listaGastos = new List<Gasto>
        {
            new Gasto { Id = 1, CategoriaGastoId = 1, Concepto = "Comida", Monto = 100m, FechaGasto = DateOnly.FromDateTime(DateTime.Today), FormaPago = "Efectivo" }
        };
        _mockContext.Setup(c => c.Gastos).ReturnsDbSet(listaGastos);
        _mockContext.Setup(c => c.Gastos.FindAsync(1)).ReturnsAsync(listaGastos.First(g => g.Id == 1));

        var dto = new ActualizarGastoDto(
            Concepto: null,
            Monto: null,
            FormaPago: "Bitcoin",
            NumeroFactura: null,
            NumeroTransaccion: null,
            NombreProveedor: null,
            TelefonoProveedor: null,
            Observaciones: null);

        // ACT
        var (ok, mensaje) = await _service.ActualizarGasto(1, dto);

        // ASSERT
        ok.Should().BeFalse();
        mensaje.Should().Be("Forma de pago inválida. Usa: Efectivo, Transferencia, Tarjeta o Cheque.");
    }

    [Fact]
    public async Task EliminarGasto_GastoNoExiste_RetornaFalso()
    {
        // ARRANGE
        var listaVacia = new List<Gasto>();
        _mockContext.Setup(c => c.Gastos).ReturnsDbSet(listaVacia);

        // ACT
        var (ok, mensaje) = await _service.EliminarGasto(99);

        // ASSERT
        ok.Should().BeFalse();
        mensaje.Should().Be("Gasto no encontrado.");
    }
}