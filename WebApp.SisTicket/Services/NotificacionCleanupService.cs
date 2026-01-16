using SisTicket.Core.Application.Services.Interfaces;

namespace WebApp.SisTicket.Services;

/// <summary>
/// Servicio en segundo plano que limpia notificaciones antiguas cada hora
/// </summary>
public class NotificacionCleanupService(
    IServiceProvider serviceProvider,
    ILogger<NotificacionCleanupService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Servicio de limpieza de notificaciones iniciado");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = serviceProvider.CreateScope();
                var notificacionService = scope.ServiceProvider.GetRequiredService<INotificacionService>();
                
                await notificacionService.LimpiarNotificacionesAntiguasAsync();
                
                logger.LogDebug("Limpieza de notificaciones ejecutada");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al limpiar notificaciones antiguas");
            }

            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }
        
        logger.LogInformation("Servicio de limpieza de notificaciones detenido");
    }
}
