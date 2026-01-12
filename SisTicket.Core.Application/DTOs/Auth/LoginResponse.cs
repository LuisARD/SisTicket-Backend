namespace SisTicket.Core.Application.DTOs.Auth;

public class LoginResponse
{
    public int Id { get; set; }
    public string NombreUsuario { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Rol { get; set; } = string.Empty;
    public string? Area { get; set; }
    public string Token { get; set; } = string.Empty;
    public bool TienePasswordTemporal { get; set; }
}
