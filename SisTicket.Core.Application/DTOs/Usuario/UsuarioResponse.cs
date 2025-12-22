namespace SisTicket.Core.Application.DTOs.Usuario;

public class UsuarioResponse
{
    public int Id { get; set; }
    public string NombreUsuario { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string NombreCompleto { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Rol { get; set; } = string.Empty;
    public int? AreaId { get; set; }
    public string? AreaNombre { get; set; }
    public DateTime FechaCreacion { get; set; }
    public bool Activo { get; set; }
}
