using Api_Eden.Controllers;
using Api_Eden.Data;
using Api_Eden.DTOs.InventarioAlimentos;
using Api_Eden.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Moq.EntityFrameworkCore; // <-- La librería que instalamos
using FluentAssertions;

namespace Api_Eden.Tests.Controllers;

public class InventarioControllerTests
{
    private readonly Mock<AppDbContext> _mockContext;
    private readonly InventarioController _controller;

    public InventarioControllerTests()
    {
        // 1. Inicializamos el mock del contexto
        _mockContext = new Mock<AppDbContext>();

        // 2. Inyectamos en el controller (no hay capa de servicio de por medio)
        _controller = new InventarioController(_mockContext.Object);
    }

    [Fact]
    public async Task GetAlimento_AlimentoNoExiste_RetornaNotFound()
    {
        // ARRANGE: lista vacía, por lo que FindAsync no encontrará el alimento
        var listaVacia = new List<Alimento>();
        _mockContext.Setup(c => c.Alimentos).ReturnsDbSet(listaVacia);

        // ACT
        var resultado = await _controller.GetAlimento(99);

        // ASSERT
        resultado.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task PostAlimento_TipoAnimalInvalido_RetornaBadRequest()
    {
        // ARRANGE: la validación ocurre en el propio controller, antes de tocar el contexto
        var dto = new CrearAlimentoDto(
            Nombre: "Croquetas Premium",
            TipoAnimal: "Dinosaurio",
            Marca: "MarcaX",
            UnidadMedida: "kg",
            CantidadDisponible: 50,
            StockMinimo: 10,
            FechaVencimiento: null);

        // ACT
        var resultado = await _controller.PostAlimento(dto);

        // ASSERT
        resultado.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task DeleteAlimento_AlimentoNoExiste_RetornaNotFound()
    {
        // ARRANGE
        var listaVacia = new List<Alimento>();
        _mockContext.Setup(c => c.Alimentos).ReturnsDbSet(listaVacia);

        // ACT
        var resultado = await _controller.DeleteAlimento(99);

        // ASSERT
        resultado.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task RegistrarSalida_StockInsuficiente_RetornaBadRequest()
    {
        // ARRANGE: alimento con solo 5 unidades disponibles
        var listaAlimentos = new List<Alimento>
        {
            new Alimento { Id = 1, Nombre = "Croquetas", TipoAnimal = "Perro", UnidadMedida = "kg", CantidadDisponible = 5, StockMinimo = 2, Activo = true, FechaCreacion = DateTime.UtcNow }
        };
        _mockContext.Setup(c => c.Alimentos).ReturnsDbSet(listaAlimentos);
        _mockContext.Setup(c => c.Alimentos.FindAsync(1)).ReturnsAsync(listaAlimentos.First(a => a.Id == 1));

        var dto = new RegistrarMovimientoDto(
            Cantidad: 10,
            Motivo: null,
            UsuarioResponsableId: 1,
            Observaciones: null,
            CostoUnitario: null);

        // ACT: se intenta sacar más cantidad (10) de la disponible (5)
        var resultado = await _controller.RegistrarSalida(1, dto);

        // ASSERT
        resultado.Should().BeOfType<BadRequestObjectResult>();
    }
}