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

    public async Task<(IEnumerable<int> destinatarios, NotificacionDto notificacion)> NotificarComentarioEliminadoAsync(
        int solicitudId,
        string numeroSolicitud,
        int usuarioEliminaId,
        int? gestorAsignadoId,
        int solicitanteId)
    {
        var usuarioElimina = await unitOfWork.Usuarios.GetByIdAsync(usuarioEliminaId);
        if (usuarioElimina == null) return ([], null!);

        var destinatarios = new List<int>();

        // Admins siempre reciben notificaciones
        var admins = await unitOfWork.Usuarios.GetAdministradoresAsync();
        destinatarios.AddRange(admins.Where(a => a.Id != usuarioEliminaId).Select(a => a.Id));

        // Gestor asignado (si existe y no es quien eliminó)
        if (gestorAsignadoId.HasValue && gestorAsignadoId != usuarioEliminaId)
        {
            destinatarios.Add(gestorAsignadoId.Value);
        }

        var notificacion = new NotificacionDto
        {
            Id = Guid.NewGuid(),
            Tipo = TipoNotificacion.ComentarioEliminado,
            Mensaje = $"{usuarioElimina.ObtenerNombreCompleto()} eliminó un comentario en la solicitud {numeroSolicitud}",
            UsuarioGeneradorId = usuarioEliminaId,
            UsuarioGeneradorNombre = usuarioElimina.ObtenerNombreCompleto(),
            UsuarioGeneradorRol = usuarioElimina.Rol.ToString(),
            SolicitudId = solicitudId,
            NumeroSolicitud = numeroSolicitud,
            FechaCreacion = DateTime.UtcNow
        };

        var destinatariosUnicos = destinatarios.Distinct().ToList();
        GuardarNotificacionParaUsuarios(destinatariosUnicos, notificacion);
        return (destinatariosUnicos, notificacion);
    }

    public async Task<IEnumerable<NotificacionDto>> ObtenerNotificacionesUsuarioAsync(int usuarioId)
    {
        var usuario = await unitOfWork.Usuarios.GetByIdAsync(usuarioId);
        if (usuario == null) return [];

        // Admin y SuperAdmin ven TODAS las notificaciones del sistema
        if (usuario.TienePermisoAdministrativo())
        {
            return await ObtenerTodasLasNotificacionesAsync();
        }

        // Usuario normal: solo sus notificaciones
        var cacheKey = $"{CACHE_KEY_PREFIX}{usuarioId}";
        
        if (cache.TryGetValue(cacheKey, out List<NotificacionDto>? notificaciones))
        {
            return notificaciones?.AsEnumerable() ?? [];
        }

        return [];
    }

    public Task<IEnumerable<NotificacionDto>> ObtenerTodasLasNotificacionesAsync()
    {
        var todasNotificaciones = new List<NotificacionDto>();

        // Obtener todas las claves de caché que comienzan con el prefijo
        // Nota: MemoryCache no tiene método para listar claves, 
        // por lo que necesitamos mantener un registro
        var usuarios = unitOfWork.Usuarios.GetAllAsync().Result;
        
        foreach (var usuario in usuarios)
        {
            var cacheKey = $"{CACHE_KEY_PREFIX}{usuario.Id}";
            
            if (cache.TryGetValue(cacheKey, out List<NotificacionDto>? notificaciones))
            {
                if (notificaciones != null)
                {
                    todasNotificaciones.AddRange(notificaciones);
                }
            }
        }

        // Eliminar duplicados y ordenar por fecha descendente
        var notificacionesUnicas = todasNotificaciones
            .GroupBy(n => n.Id)
            .Select(g => g.First())
            .OrderByDescending(n => n.FechaCreacion)
            .ToList();

        return Task.FromResult(notificacionesUnicas.AsEnumerable());
    }

    public async Task<bool> EliminarNotificacionAsync(Guid notificacionId, int usuarioId)
    {
        var usuario = await unitOfWork.Usuarios.GetByIdAsync(usuarioId);
        
        // Solo Admin o SuperAdmin pueden eliminar notificaciones
        if (usuario == null || !usuario.TienePermisoAdministrativo())
        {
            return false;
        }

        var eliminada = false;
        var usuarios = await unitOfWork.Usuarios.GetAllAsync();

        foreach (var user in usuarios)
        {
            var cacheKey = $"{CACHE_KEY_PREFIX}{user.Id}";
            
            if (cache.TryGetValue(cacheKey, out List<NotificacionDto>? notificaciones))
            {
                if (notificaciones != null)
                {
                    var notifAEliminar = notificaciones.FirstOrDefault(n => n.Id == notificacionId);
                    if (notifAEliminar != null)
                    {
                        notificaciones.Remove(notifAEliminar);
                        cache.Set(cacheKey, notificaciones, _duracionCache);
                        eliminada = true;
                    }
                }
            }
        }

        return eliminada;
    }

    public async Task LimpiarNotificacionesUsuarioAsync(int usuarioIdObjetivo, int adminId)
    {
        var admin = await unitOfWork.Usuarios.GetByIdAsync(adminId);
        
        // Solo Admin o SuperAdmin pueden limpiar notificaciones
        if (admin == null || !admin.TienePermisoAdministrativo())
        {
            return;
        }

        var cacheKey = $"{CACHE_KEY_PREFIX}{usuarioIdObjetivo}";
        cache.Remove(cacheKey);
    }

    public Task LimpiarNotificacionesAntiguasAsync()
    {
        // El MemoryCache automáticamente elimina entradas expiradas (24h)
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
