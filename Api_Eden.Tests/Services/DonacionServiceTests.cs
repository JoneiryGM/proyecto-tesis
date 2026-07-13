using Api_Eden.Data;
using Api_Eden.DTOs.DonacionesDto;
using Api_Eden.Models;
using Api_Eden.Services.DonacionService;
using Moq;
using Moq.EntityFrameworkCore; // <-- La librería que instalamos
using FluentAssertions;

namespace Api_Eden.Tests.Services;

public class DonacionServiceTests
{
    private readonly Mock<AppDbContext> _mockContext;
    private readonly DonacionService _service;

    public DonacionServiceTests()
    {
        // 1. Inicializamos el mock del contexto
        _mockContext = new Mock<AppDbContext>();

        // 2. Inyectamos en el servicio
        _service = new DonacionService(_mockContext.Object);
    }

    [Fact]
    public async Task GetTiposAsync_RetornaSoloTiposActivos()
    {
        // ARRANGE
        var listaTipos = new List<Tiposdonacion>
        {
            new Tiposdonacion { Id = 1, Nombre = "Dinero", Activa = true },
            new Tiposdonacion { Id = 2, Nombre = "Descontinuado", Activa = false }
        };
        _mockContext.Setup(c => c.Tiposdonacions).ReturnsDbSet(listaTipos);

        // ACT
        var resultado = (await _service.GetTiposAsync()) as IEnumerable<object>;

        // ASSERT
        resultado.Should().NotBeNull();
        resultado!.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetDonantesAsync_RetornaSoloDonantesActivos()
    {
        // ARRANGE
        var listaDonantes = new List<Donante>
        {
            new Donante { Id = 1, Nombre = "Ana Pérez", TipoDonante = "Persona", Activo = true, FechaRegistro = DateOnly.FromDateTime(DateTime.Today) },
            new Donante { Id = 2, Nombre = "Donante Inactivo", TipoDonante = "Persona", Activo = false, FechaRegistro = DateOnly.FromDateTime(DateTime.Today) }
        };
        _mockContext.Setup(c => c.Donantes).ReturnsDbSet(listaDonantes);

        // ACT
        var resultado = (await _service.GetDonantesAsync()) as IEnumerable<object>;

        // ASSERT
        resultado.Should().NotBeNull();
        resultado!.Should().HaveCount(1);
    }

    [Fact]
    public async Task EliminarAsync_DonacionNoExiste_RetornaFalso()
    {
        // ARRANGE: lista vacía, por lo que FindAsync no encontrará la donación
        var listaVacia = new List<Donacione>();
        _mockContext.Setup(c => c.Donaciones).ReturnsDbSet(listaVacia);

        // ACT
        var (ok, mensaje) = await _service.EliminarAsync(99);

        // ASSERT
        ok.Should().BeFalse();
        mensaje.Should().Be("Donación no encontrada.");
    }

    [Fact]
    public async Task RegistrarAsync_DonacionSimpleConDonanteExistente_RegistraCorrectamente()
    {
        // ARRANGE: donante ya existe (DonanteId provisto) y sin objetivo asociado,
        // así que el servicio solo debería tocar _db.Donaciones
        var listaDonaciones = new List<Donacione>();
        _mockContext.Setup(c => c.Donaciones).ReturnsDbSet(listaDonaciones);

        var dto = new RegistrarDonacionDto(
            DonanteId: 1,
            NombreDonante: null,
            EmailDonante: null,
            TelefonoDonante: null,
            TipoDonacionId: 1,
            MontoDinero: 500m,
            ValorEstimado: null,
            CantidadArticulos: null,
            DescripcionDonacion: null,
            FormaPago: "Efectivo",
            NumeroTransaccion: null,
            FechaDonacion: null,
            ObjetivoId: null,
            Observaciones: null);

        // ACT
        var (ok, mensaje, id) = await _service.RegistrarAsync(dto, usuarioId: 1);

        // ASSERT
        ok.Should().BeTrue();
        mensaje.Should().Be("Donación registrada correctamente.");
        // El DbSet mockeado no sincroniza Add() con nuestra lista local, así que
        // verificamos con Moq que se intentó agregar la donación con los datos correctos.
        _mockContext.Verify(c => c.Donaciones.Add(It.Is<Donacione>(d =>
            d.DonanteId == 1 && d.MontoDinero == 500m)), Times.Once);
    }
}