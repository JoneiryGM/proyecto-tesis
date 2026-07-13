using Api_Eden.Services.EmailService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using FluentAssertions;

namespace Api_Eden.Tests.Services;

public class EmailServiceTests
{
    private readonly Mock<IConfiguration> _mockConfig;
    private readonly Mock<ILogger<EmailService>> _mockLogger;
    private readonly EmailService _service;

    public EmailServiceTests()
    {
        // 1. Inicializamos los mocks básicos
        _mockConfig = new Mock<IConfiguration>();
        _mockLogger = new Mock<ILogger<EmailService>>();

        // 2. Inyectamos en el servicio
        _service = new EmailService(_mockConfig.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task EnviarActivacionAsync_SinApiKeyConfigurada_RetornaTrueSinEnviarEmailReal()
    {
        // ARRANGE: sin API key configurada → el servicio debe caer en modo desarrollo
        _mockConfig.Setup(c => c["SendGrid:ApiKey"]).Returns((string?)null);

        // ACT
        var resultado = await _service.EnviarActivacionAsync(
            "usuario@eden.com", "Juan", "https://eden.com/activar?token=abc123");

        // ASSERT: en modo desarrollo siempre retorna true (solo loguea el link)
        resultado.Should().BeTrue();
    }

    [Fact]
    public async Task EnviarActivacionAsync_ConApiKeyPlaceholder_RetornaTrueSinEnviarEmailReal()
    {
        // ARRANGE: la API key sigue siendo el placeholder del proyecto → también es modo desarrollo
        _mockConfig.Setup(c => c["SendGrid:ApiKey"]).Returns("TU_API_KEY_DE_SENDGRID");

        // ACT
        var resultado = await _service.EnviarActivacionAsync(
            "usuario@eden.com", "Juan", "https://eden.com/activar?token=abc123");

        // ASSERT
        resultado.Should().BeTrue();
    }

    [Fact]
    public async Task EnviarRecuperacionAsync_SinApiKeyConfigurada_RetornaTrueSinEnviarEmailReal()
    {
        // ARRANGE: sin API key configurada → modo desarrollo
        _mockConfig.Setup(c => c["SendGrid:ApiKey"]).Returns((string?)null);

        // ACT
        var resultado = await _service.EnviarRecuperacionAsync(
            "usuario@eden.com", "Juan", "https://eden.com/reset?token=abc123");

        // ASSERT
        resultado.Should().BeTrue();
    }

    [Fact]
    public async Task EnviarRecuperacionAsync_ConApiKeyPlaceholder_RetornaTrueSinEnviarEmailReal()
    {
        // ARRANGE: placeholder de API key → modo desarrollo
        _mockConfig.Setup(c => c["SendGrid:ApiKey"]).Returns("TU_API_KEY_DE_SENDGRID");

        // ACT
        var resultado = await _service.EnviarRecuperacionAsync(
            "usuario@eden.com", "Juan", "https://eden.com/reset?token=abc123");

        // ASSERT
        resultado.Should().BeTrue();
    }
}