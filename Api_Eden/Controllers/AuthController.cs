using Api_Eden.DTOs.AuthDto;
using Api_Eden.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Api_Eden.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(AuthService authService) : ControllerBase
{
    [HttpPost("Registro")]
    public async Task<IActionResult> Registro([FromBody] RegistroDto dto)
    {
        var (exito, mensaje) = await authService.RegistrarAsync(dto);
        return exito ? Ok(new { mensaje }) : BadRequest(new { mensaje });
    }

    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto) =>
        await authService.LoginAsync(dto) is { } resultado
            ? Ok(resultado)
            : Unauthorized(new { mensaje = "Credenciales inválidas" });

    [Authorize(Roles = "Administrador")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Actualizar(int id, [FromBody] ActualizarUsuarioDto dto)
    {
        var (exito, mensaje) = await authService.ActualizarAsync(id, dto);
        return exito ? Ok(new { mensaje }) : BadRequest(new { mensaje });
    }

    [Authorize(Roles = "Administrador")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        var idActual = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var (exito, mensaje) = await authService.EliminarAsync(id, idActual);
        return exito ? Ok(new { mensaje }) : BadRequest(new { mensaje });
    }
}