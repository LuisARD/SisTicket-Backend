namespace SisTicket.Core.Application.DTOs.Usuario;

public class UsuarioRequest
{
    public string NombreUsuario { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public int Rol { get; set; }
    public int? AreaId { get; set; }
}
