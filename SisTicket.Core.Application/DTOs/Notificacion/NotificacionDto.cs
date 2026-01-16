using SisTicket.Core.Domain.Enums;

namespace SisTicket.Core.Application.DTOs.Notificacion;

public record NotificacionDto
{
    public Guid Id { get; init; }
    public TipoNotificacion Tipo { get; init; }
    public string Mensaje { get; init; } = string.Empty;
    
    public int UsuarioGeneradorId { get; init; }
    public string UsuarioGeneradorNombre { get; init; } = string.Empty;
    public string UsuarioGeneradorRol { get; init; } = string.Empty;
    
    public int SolicitudId { get; init; }
    public string NumeroSolicitud { get; init; } = string.Empty;
    
    public DateTime FechaCreacion { get; init; }
}
