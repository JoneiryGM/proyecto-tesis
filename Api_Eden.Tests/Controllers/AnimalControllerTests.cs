using Api_Eden.Controllers;
using Api_Eden.Data;
using Api_Eden.DTOs.AnimalCreadoDto;
using Api_Eden.Models;
using Api_Eden.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Moq.EntityFrameworkCore; // <-- La librería que instalamos
using FluentAssertions;

namespace Api_Eden.Tests.Controllers;

public class AnimalControllerTests
{
    private readonly Mock<AppDbContext> _mockContext;
    private readonly AnimalController _controller;

    public AnimalControllerTests()
    {
        // 1. Inicializamos el mock del contexto
        _mockContext = new Mock<AppDbContext>();

        // 2. AnimalService no tiene interfaz y sus métodos no son virtuales,
        //    así que usamos una instancia REAL respaldada por el AppDbContext mockeado.
        var animalService = new AnimalService(_mockContext.Object);
        _controller = new AnimalController(animalService);
    }

    [Fact]
    public async Task GetAnimales_RetornaOkConListaDeAnimales()
    {
        // ARRANGE
        var listaAnimales = new List<Animale>
        {
            new Animale { Id = 1, Nombre = "Firulais", EspecieId = 1, FechaIngreso = DateOnly.FromDateTime(DateTime.Now), EstadoGeneral = "Activo", EstadoSalud = "Sano" }
        };
        _mockContext.Setup(c => c.Animales).ReturnsDbSet(listaAnimales);

        // ACT
        var resultado = await _controller.GetAnimales();

        // ASSERT
        resultado.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetAnimal_AnimalNoExiste_RetornaNotFound()
    {
        // ARRANGE
        var listaVacia = new List<Animale>();
        _mockContext.Setup(c => c.Animales).ReturnsDbSet(listaVacia);

        // ACT
        var resultado = await _controller.GetAnimal(99);

        // ASSERT
        resultado.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task DeleteAnimal_AnimalNoExiste_RetornaNotFound()
    {
        // ARRANGE
        var listaVacia = new List<Animale>();
        _mockContext.Setup(c => c.Animales).ReturnsDbSet(listaVacia);

        // ACT
        var resultado = await _controller.DeleteAnimal(99);

        // ASSERT
        resultado.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task ActualizarEstado_AnimalNoExiste_RetornaNotFound()
    {
        // ARRANGE
        var listaVacia = new List<Animale>();
        _mockContext.Setup(c => c.Animales).ReturnsDbSet(listaVacia);

        var dto = new ActualizarEstadoAnimalDto(
            EstadoGeneral: "Inactivo",
            EstadoSalud: "Enfermo");

        // ACT
        var resultado = await _controller.ActualizarEstado(99, dto);

        // ASSERT
        resultado.Should().BeOfType<NotFoundObjectResult>();
    }
}