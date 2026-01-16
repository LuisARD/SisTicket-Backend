using Microsoft.AspNetCore.SignalR;
using SisTicket.Core.Application.DTOs.Notificacion;
using WebApp.SisTicket.Hubs;

namespace WebApp.SisTicket.Services;

/// <summary>
/// Servicio para enviar notificaciones en tiempo real vía SignalR
/// </summary>
public class NotificacionBroadcaster(IHubContext<NotificacionHub> hubContext)
{
    /// <summary>
    /// Envía una notificación a múltiples usuarios conectados
    /// </summary>
    public async Task EnviarNotificacionAsync(IEnumerable<int> usuariosIds, NotificacionDto notificacion)
    {
        var tasks = usuariosIds.Select(async usuarioId =>
        {
            await hubContext.Clients
                .Group($"user_{usuarioId}")
                .SendAsync("RecibirNotificacion", notificacion);
        });

        await Task.WhenAll(tasks);
    }
}
