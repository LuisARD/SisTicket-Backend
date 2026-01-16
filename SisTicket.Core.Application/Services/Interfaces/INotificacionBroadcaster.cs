using SisTicket.Core.Application.DTOs.Notificacion;

namespace SisTicket.Core.Application.Services.Interfaces;

/// <summary>
/// Servicio para enviar notificaciones en tiempo real vía SignalR
/// </summary>
public interface INotificacionBroadcaster
{
    /// <summary>
    /// Envía una notificación a múltiples usuarios conectados
    /// </summary>
    Task EnviarNotificacionAsync(IEnumerable<int> usuariosIds, NotificacionDto notificacion);
}
