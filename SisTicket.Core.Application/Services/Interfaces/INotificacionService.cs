using SisTicket.Core.Application.DTOs.Notificacion;
using SisTicket.Core.Domain.Enums;

namespace SisTicket.Core.Application.Services.Interfaces;

public interface INotificacionService
{
    /// <summary>
    /// Notifica a gestores del área y admins sobre una nueva solicitud
    /// Retorna tupla con IDs de destinatarios y la notificación creada
    /// </summary>
    Task<(IEnumerable<int> destinatarios, NotificacionDto notificacion)> NotificarSolicitudCreadaAsync(
        int solicitudId,
        string numeroSolicitud,
        int solicitanteId,
        int areaId);

    /// <summary>
    /// Notifica al solicitante y admins cuando un gestor es asignado
    /// Retorna tupla con IDs de destinatarios y la notificación creada
    /// </summary>
    Task<(IEnumerable<int> destinatarios, NotificacionDto notificacion)> NotificarGestorAsignadoAsync(
        int solicitudId,
        string numeroSolicitud,
        int gestorId,
        bool esAutoasignacion);

    /// <summary>
    /// Notifica según el rol del autor del comentario
    /// Retorna tupla con IDs de destinatarios y la notificación creada
    /// </summary>
    Task<(IEnumerable<int> destinatarios, NotificacionDto notificacion)> NotificarComentarioAgregadoAsync(
        int solicitudId,
        string numeroSolicitud,
        int autorComentarioId,
        int? gestorAsignadoId,
        int solicitanteId);

    /// <summary>
    /// Notifica sobre cambios de estado a solicitante, gestor y admins
    /// Retorna tupla con IDs de destinatarios y la notificación creada
    /// </summary>
    Task<(IEnumerable<int> destinatarios, NotificacionDto notificacion)> NotificarEstadoCambiadoAsync(
        int solicitudId,
        string numeroSolicitud,
        EstadoSolicitud estadoAnterior,
        EstadoSolicitud estadoNuevo,
        int usuarioCambioId,
        int? gestorAsignadoId,
        int solicitanteId);

    /// <summary>
    /// Obtiene notificaciones de las últimas 24h para un usuario
    /// </summary>
    Task<IEnumerable<NotificacionDto>> ObtenerNotificacionesUsuarioAsync(int usuarioId);

    /// <summary>
    /// Limpia notificaciones con más de 24 horas
    /// </summary>
    Task LimpiarNotificacionesAntiguasAsync();
}
