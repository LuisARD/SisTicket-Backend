using Microsoft.AspNetCore.SignalR;
using SisTicket.Core.Application.DTOs.Notificacion;
using SisTicket.Core.Application.Services.Interfaces;
using WebApp.SisTicket.Hubs;

namespace WebApp.SisTicket.Services;

/// <summary>
/// Implementación del broadcaster usando SignalR
/// </summary>
public class NotificacionBroadcaster(IHubContext<NotificacionHub> hubContext) : INotificacionBroadcaster
{
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
