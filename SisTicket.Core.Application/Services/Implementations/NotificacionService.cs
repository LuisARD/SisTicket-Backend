using Microsoft.Extensions.Caching.Memory;
using SisTicket.Core.Application.DTOs.Notificacion;
using SisTicket.Core.Application.Services.Interfaces;
using SisTicket.Core.Domain.Enums;
using SisTicket.Core.Domain.Interfaces;

namespace SisTicket.Core.Application.Services.Implementations;

public class NotificacionService(IMemoryCache cache, IUnitOfWork unitOfWork) : INotificacionService
{
    private const string CACHE_KEY_PREFIX = "notificaciones_usuario_";
    private readonly TimeSpan _duracionCache = TimeSpan.FromHours(24);

    public async Task<(IEnumerable<int> destinatarios, NotificacionDto notificacion)> NotificarSolicitudCreadaAsync(
        int solicitudId,
        string numeroSolicitud,
        int solicitanteId,
        int areaId)
    {
        var solicitante = await unitOfWork.Usuarios.GetByIdAsync(solicitanteId);
        if (solicitante == null) return ([], null!);

        var destinatarios = new List<int>();

        // Gestores del área
        var gestoresArea = await unitOfWork.Usuarios.GetGestoresByAreaIdAsync(areaId);
        destinatarios.AddRange(gestoresArea.Select(g => g.Id));

        // Admins y SuperAdmins
        var admins = await unitOfWork.Usuarios.GetAdministradoresAsync();
        destinatarios.AddRange(admins.Select(a => a.Id));

        var notificacion = new NotificacionDto
        {
            Id = Guid.NewGuid(),
            Tipo = TipoNotificacion.SolicitudCreada,
            Mensaje = $"Nueva solicitud {numeroSolicitud} creada por {solicitante.ObtenerNombreCompleto()}",
            UsuarioGeneradorId = solicitanteId,
            UsuarioGeneradorNombre = solicitante.ObtenerNombreCompleto(),
            UsuarioGeneradorRol = solicitante.Rol.ToString(),
            SolicitudId = solicitudId,
            NumeroSolicitud = numeroSolicitud,
            FechaCreacion = DateTime.UtcNow
        };

        var destinatariosUnicos = destinatarios.Distinct().ToList();
        GuardarNotificacionParaUsuarios(destinatariosUnicos, notificacion);
        return (destinatariosUnicos, notificacion);
    }

    public async Task<(IEnumerable<int> destinatarios, NotificacionDto notificacion)> NotificarGestorAsignadoAsync(
        int solicitudId,
        string numeroSolicitud,
        int gestorId,
        bool esAutoasignacion)
    {
        var gestor = await unitOfWork.Usuarios.GetByIdAsync(gestorId);
        if (gestor == null) return ([], null!);

        var solicitud = await unitOfWork.Solicitudes.GetByIdWithDetailsAsync(solicitudId);
        if (solicitud == null) return ([], null!);

        var destinatarios = new List<int> { solicitud.SolicitanteId };

        // Notificar a admins solo si es autoasignación
        if (esAutoasignacion)
        {
            var admins = await unitOfWork.Usuarios.GetAdministradoresAsync();
            destinatarios.AddRange(admins.Select(a => a.Id));
        }

        var mensaje = esAutoasignacion
            ? $"{gestor.ObtenerNombreCompleto()} se autoasignó la solicitud {numeroSolicitud}"
            : $"{gestor.ObtenerNombreCompleto()} fue asignado a la solicitud {numeroSolicitud}";

        var notificacion = new NotificacionDto
        {
            Id = Guid.NewGuid(),
            Tipo = TipoNotificacion.GestorAsignado,
            Mensaje = mensaje,
            UsuarioGeneradorId = gestorId,
            UsuarioGeneradorNombre = gestor.ObtenerNombreCompleto(),
            UsuarioGeneradorRol = gestor.Rol.ToString(),
            SolicitudId = solicitudId,
            NumeroSolicitud = numeroSolicitud,
            FechaCreacion = DateTime.UtcNow
        };

        var destinatariosUnicos = destinatarios.Distinct().ToList();
        GuardarNotificacionParaUsuarios(destinatariosUnicos, notificacion);
        return (destinatariosUnicos, notificacion);
    }

    public async Task<(IEnumerable<int> destinatarios, NotificacionDto notificacion)> NotificarComentarioAgregadoAsync(
        int solicitudId,
        string numeroSolicitud,
        int autorComentarioId,
        int? gestorAsignadoId,
        int solicitanteId)
    {
        var autor = await unitOfWork.Usuarios.GetByIdAsync(autorComentarioId);
        if (autor == null) return ([], null!);

        var destinatarios = new List<int>();

        // Admins siempre reciben notificaciones
        var admins = await unitOfWork.Usuarios.GetAdministradoresAsync();
        destinatarios.AddRange(admins.Where(a => a.Id != autorComentarioId).Select(a => a.Id));

        // Notificar según el rol del autor
        if (autor.EsGestor() && gestorAsignadoId == autorComentarioId)
        {
            destinatarios.Add(solicitanteId);
        }
        else if (autor.EsSolicitante())
        {
            if (gestorAsignadoId.HasValue)
                destinatarios.Add(gestorAsignadoId.Value);
        }
        else if (autor.TienePermisoAdministrativo())
        {
            if (gestorAsignadoId.HasValue && gestorAsignadoId != autorComentarioId)
                destinatarios.Add(gestorAsignadoId.Value);
            
            if (solicitanteId != autorComentarioId)
                destinatarios.Add(solicitanteId);
        }

        var notificacion = new NotificacionDto
        {
            Id = Guid.NewGuid(),
            Tipo = TipoNotificacion.ComentarioAgregado,
            Mensaje = $"{autor.ObtenerNombreCompleto()} comentó en la solicitud {numeroSolicitud}",
            UsuarioGeneradorId = autorComentarioId,
            UsuarioGeneradorNombre = autor.ObtenerNombreCompleto(),
            UsuarioGeneradorRol = autor.Rol.ToString(),
            SolicitudId = solicitudId,
            NumeroSolicitud = numeroSolicitud,
            FechaCreacion = DateTime.UtcNow
        };

        var destinatariosUnicos = destinatarios.Distinct().ToList();
        GuardarNotificacionParaUsuarios(destinatariosUnicos, notificacion);
        return (destinatariosUnicos, notificacion);
    }

    public async Task<(IEnumerable<int> destinatarios, NotificacionDto notificacion)> NotificarEstadoCambiadoAsync(
        int solicitudId,
        string numeroSolicitud,
        EstadoSolicitud estadoAnterior,
        EstadoSolicitud estadoNuevo,
        int usuarioCambioId,
        int? gestorAsignadoId,
        int solicitanteId)
    {
        var usuario = await unitOfWork.Usuarios.GetByIdAsync(usuarioCambioId);
        if (usuario == null) return ([], null!);

        var destinatarios = new List<int> { solicitanteId };

        if (gestorAsignadoId.HasValue && gestorAsignadoId != usuarioCambioId)
            destinatarios.Add(gestorAsignadoId.Value);

        var admins = await unitOfWork.Usuarios.GetAdministradoresAsync();
        destinatarios.AddRange(admins.Where(a => a.Id != usuarioCambioId).Select(a => a.Id));

        var notificacion = new NotificacionDto
        {
            Id = Guid.NewGuid(),
            Tipo = TipoNotificacion.EstadoCambiado,
            Mensaje = $"{usuario.ObtenerNombreCompleto()} cambió el estado de {numeroSolicitud} de '{estadoAnterior}' a '{estadoNuevo}'",
            UsuarioGeneradorId = usuarioCambioId,
            UsuarioGeneradorNombre = usuario.ObtenerNombreCompleto(),
            UsuarioGeneradorRol = usuario.Rol.ToString(),
            SolicitudId = solicitudId,
            NumeroSolicitud = numeroSolicitud,
            FechaCreacion = DateTime.UtcNow
        };

        var destinatariosUnicos = destinatarios.Distinct().ToList();
        GuardarNotificacionParaUsuarios(destinatariosUnicos, notificacion);
        return (destinatariosUnicos, notificacion);
    }

    public Task<IEnumerable<NotificacionDto>> ObtenerNotificacionesUsuarioAsync(int usuarioId)
    {
        var cacheKey = $"{CACHE_KEY_PREFIX}{usuarioId}";
        
        if (cache.TryGetValue(cacheKey, out List<NotificacionDto>? notificaciones))
        {
            return Task.FromResult(notificaciones?.AsEnumerable() ?? []);
        }

        return Task.FromResult(Enumerable.Empty<NotificacionDto>());
    }

    public Task LimpiarNotificacionesAntiguasAsync()
    {
        // El MemoryCache automáticamente elimina entradas expiradas
        // Este método existe por completitud de la interfaz
        return Task.CompletedTask;
    }

    /// <summary>
    /// Almacena la notificación en caché para cada usuario destinatario
    /// </summary>
    private void GuardarNotificacionParaUsuarios(IEnumerable<int> usuariosIds, NotificacionDto notificacion)
    {
        foreach (var usuarioId in usuariosIds)
        {
            var cacheKey = $"{CACHE_KEY_PREFIX}{usuarioId}";
            
            var notificaciones = cache.GetOrCreate(cacheKey, entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = _duracionCache;
                return new List<NotificacionDto>();
            }) ?? [];

            notificaciones.Add(notificacion);
            cache.Set(cacheKey, notificaciones, _duracionCache);
        }
    }
}
