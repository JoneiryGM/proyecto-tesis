namespace Api_Eden.DTOs.AuthDto
{
    public record LoginDto(string email, string password);

    public record RegistroDto(
    string Nombre,
    string Apellido,
    string Email,
    string Password,
    string Rol = "Trabajador"
        );

    public record AuthResponseDto(string Token, string Nombre, string Email, string Rol);


}
