using Api_Eden.Data;
using Api_Eden.DTOs.AuthDto;
using Api_Eden.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Api_Eden.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IConfiguration _config;

        public AuthController(AppDbContext db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }

        [HttpPost("Registro")]

        public async Task<IActionResult> Registro([FromBody] RegistroDto registroDto)
        {

            try
            {
                if (await _db.Usuarios.AnyAsync(u => u.Email == registroDto.Email))
                {
                    return BadRequest(new { mensaje = "Este Email ya existe" });
                }
                var usuario = new Usuario
                {
                    Nombre = registroDto.Nombre,
                    Apellido = registroDto.Apellido,
                    Email = registroDto.Email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(registroDto.Password),
                    Rol = registroDto.Rol,
                    Activo = true,
                    FechaCreacion = DateTime.UtcNow,
                    FechaUltimaModificacion = DateTime.UtcNow
                };
                _db.Usuarios.Add(usuario);
                await _db.SaveChangesAsync();
                return Ok(new { mensaje = "Usuario registrado exitosamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error interno del servidor", ex.Message });
            }

        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                var usuario = await _db.Usuarios.FirstOrDefaultAsync(u => u.Email == loginDto.email);

                if (usuario == null || !BCrypt.Net.BCrypt.Verify(loginDto.password, usuario.PasswordHash))
                {
                    return Unauthorized(new { mensaje = "Credenciales inválidas" });
                }
                var token = GenerarToken(usuario);
                
                return Ok(new AuthResponseDto(token, usuario.Nombre, usuario.Email, usuario.Rol));

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error interno del servidor", ex.Message });
            }
        }

        private string GenerarToken(Usuario usuario)
        {

            var jwtConfig = _config.GetSection("Jwt");
            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(jwtConfig["Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[] {
            new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new Claim(ClaimTypes.Email,           usuario.Email),
            new Claim(ClaimTypes.Name,            $"{usuario.Nombre} {usuario.Apellido}"),
            new Claim(ClaimTypes.Role,            usuario.Rol)
            };
            var token = new System.IdentityModel.Tokens.Jwt.JwtSecurityToken(
                issuer: jwtConfig["Issuer"],
                audience: jwtConfig["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // Este enpoint fue creado en caso de que sea necesario hacer 

        [Authorize(Roles = "Administrador")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] ActualizarUsuarioDto dto)
        {
            var usuario = await _db.Usuarios.FindAsync(id);

            if (usuario is null)
                return NotFound("Usuario no encontrado.");

            if (!string.IsNullOrWhiteSpace(dto.Nombre))
                usuario.Nombre = dto.Nombre;

            if (!string.IsNullOrWhiteSpace(dto.Apellido))
                usuario.Apellido = dto.Apellido;

            if (!string.IsNullOrWhiteSpace(dto.Email))
            {
                if (await _db.Usuarios.AnyAsync(u => u.Email == dto.Email && u.Id != id))
                    return BadRequest("El email ya está en uso por otro usuario.");
                usuario.Email = dto.Email;
            }

            if (!string.IsNullOrWhiteSpace(dto.Rol))
            {
                var rolesValidos = new[] { "Administrador", "Veterinario", "Trabajador" };
                if (!rolesValidos.Contains(dto.Rol))
                    return BadRequest("Rol inválido. Usa: Administrador, Veterinario o Trabajador.");
                usuario.Rol = dto.Rol;
            }

            if (!string.IsNullOrWhiteSpace(dto.Password))
                usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            if (dto.Activo.HasValue)
                usuario.Activo = dto.Activo;

            usuario.FechaUltimaModificacion = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            return Ok(new { mensaje = "Usuario actualizado correctamente." });
        }

        // DELETE api/auth/{id}
        [Authorize(Roles = "Administrador")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var usuario = await _db.Usuarios.FindAsync(id);

            if (usuario is null)
                return NotFound("Usuario no encontrado.");

            var idActual = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            if (id == idActual)
                return BadRequest("No puedes eliminar tu propio usuario.");

            _db.Usuarios.Remove(usuario);
            await _db.SaveChangesAsync();

            return Ok(new { mensaje = "Usuario eliminado correctamente." });
        }

    }
    
}
