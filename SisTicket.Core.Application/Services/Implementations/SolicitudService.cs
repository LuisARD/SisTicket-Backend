using AutoMapper;
using SisTicket.Core.Application.DTOs.Solicitud;
using SisTicket.Core.Application.Exceptions;
using SisTicket.Core.Application.Services.Interfaces;
using SisTicket.Core.Domain.Entities;
using SisTicket.Core.Domain.Enums;
using SisTicket.Core.Domain.Interfaces;

namespace SisTicket.Core.Application.Services.Implementations;

public class SolicitudService : ISolicitudService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IAuditoriaService _auditoriaService;
    private readonly INotificacionService _notificacionService;
    private readonly INotificacionBroadcaster _broadcaster;

    public SolicitudService(
        IUnitOfWork unitOfWork, 
        IMapper mapper, 
        IAuditoriaService auditoriaService,
        INotificacionService notificacionService,
        INotificacionBroadcaster broadcaster)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _auditoriaService = auditoriaService;
        _notificacionService = notificacionService;
        _broadcaster = broadcaster;
    }

    public async Task<IEnumerable<SolicitudResponse>> GetAllAsync()
    {
        var solicitudes = await _unitOfWork.Solicitudes.GetAllAsync();
        return _mapper.Map<IEnumerable<SolicitudResponse>>(solicitudes);
    }

    public async Task<SolicitudResponse> GetByIdAsync(int id)
    {
        var solicitud = await _unitOfWork.Solicitudes.GetByIdWithDetailsAsync(id);
        
        if (solicitud == null)
            throw new NotFoundException(nameof(Solicitud), id);

        return _mapper.Map<SolicitudResponse>(solicitud);
    }

    public async Task<SolicitudResponse> CreateAsync(SolicitudRequest request, int solicitanteId)
    {
        // Validar que el solicitante existe
        var solicitante = await _unitOfWork.Usuarios.GetByIdAsync(solicitanteId);
        if (solicitante == null)
            throw new NotFoundException(nameof(Usuario), solicitanteId);

        // Validar que existe el tipo de solicitud
        var tipoSolicitud = await _unitOfWork.TiposSolicitud.GetByIdAsync(request.TipoSolicitudId);
        if (tipoSolicitud == null)
            throw new NotFoundException(nameof(TipoSolicitud), request.TipoSolicitudId);

        // Validar que existe la prioridad
        var prioridad = await _unitOfWork.Prioridades.GetByIdAsync(request.PrioridadId);
        if (prioridad == null)
            throw new NotFoundException(nameof(Prioridad), request.PrioridadId);

        // Validar que existe el área
        var area = await _unitOfWork.Areas.GetByIdAsync(request.AreaId);
        if (area == null)
            throw new NotFoundException(nameof(Area), request.AreaId);

        // Generar número de solicitud automático
        var numeroSolicitud = await _unitOfWork.Solicitudes.GenerarNumeroSolicitudAsync();

        var solicitud = new Solicitud
        {
            NumeroSolicitud = numeroSolicitud,
            Titulo = request.Titulo,
            Descripcion = request.Descripcion,
            Estado = EstadoSolicitud.Nueva,
            SolicitanteId = solicitanteId,
            TipoSolicitudId = request.TipoSolicitudId,
            PrioridadId = request.PrioridadId,
            AreaId = request.AreaId
        };

        await _unitOfWork.Solicitudes.AddAsync(solicitud);
        await _unitOfWork.SaveChangesAsync();

        // Notificar creación de solicitud a gestores del área y admins
        await _notificacionService.NotificarSolicitudCreadaAsync(
            solicitud.Id,
            solicitud.NumeroSolicitud,
            solicitanteId,
            solicitud.AreaId);

        var solicitudCreada = await _unitOfWork.Solicitudes.GetByIdWithDetailsAsync(solicitud.Id);
        return _mapper.Map<SolicitudResponse>(solicitudCreada);
    }

    public async Task<SolicitudResponse> UpdateAsync(int id, SolicitudRequest request, int usuarioId)
    {
        var solicitud = await _unitOfWork.Solicitudes.GetByIdWithDetailsAsync(id);
        
        if (solicitud == null)
            throw new NotFoundException(nameof(Solicitud), id);

        var usuario = await _unitOfWork.Usuarios.GetByIdAsync(usuarioId);
        if (usuario == null)
            throw new UnauthorizedException();

        // REGLA DE NEGOCIO: Solo el solicitante puede editar si está en estado Nueva y sin gestor asignado
        if (!solicitud.PuedeSerEditadaPorSolicitante())
        {
            throw new ValidationException("La solicitud no puede ser editada porque ya tiene un gestor asignado o no está en estado Nueva");
        }

        // Validar que es el solicitante quien intenta editar
        if (solicitud.SolicitanteId != usuarioId && !usuario.TienePermisoAdministrativo())
        {
            throw new UnauthorizedException("Solo el solicitante puede editar la solicitud");
        }

        // Capturar valores ANTES del cambio
        var valoresAntiguos = new
        {
            Titulo = solicitud.Titulo,
            Descripcion = solicitud.Descripcion,
            TipoSolicitud = solicitud.TipoSolicitud?.Nombre,
            Prioridad = solicitud.Prioridad?.Nombre,
            Area = solicitud.Area?.Nombre
        };

        // Actualización parcial: solo actualizar campos con valores

        // Actualizar título si se envía
        if (!string.IsNullOrWhiteSpace(request.Titulo))
        {
            solicitud.Titulo = request.Titulo;
        }

        // Actualizar descripción si se envía
        if (!string.IsNullOrWhiteSpace(request.Descripcion))
        {
            solicitud.Descripcion = request.Descripcion;
        }

        // Actualizar tipo de solicitud si se envía
        if (request.TipoSolicitudId > 0)
        {
            var tipoSolicitud = await _unitOfWork.TiposSolicitud.GetByIdAsync(request.TipoSolicitudId);
            if (tipoSolicitud == null)
                throw new NotFoundException(nameof(TipoSolicitud), request.TipoSolicitudId);
            
            solicitud.TipoSolicitudId = request.TipoSolicitudId;
        }

        // Actualizar prioridad si se envía
        if (request.PrioridadId > 0)
        {
            var prioridad = await _unitOfWork.Prioridades.GetByIdAsync(request.PrioridadId);
            if (prioridad == null)
                throw new NotFoundException(nameof(Prioridad), request.PrioridadId);
            
            solicitud.PrioridadId = request.PrioridadId;
        }

        // Actualizar área si se envía
        if (request.AreaId > 0)
        {
            var area = await _unitOfWork.Areas.GetByIdAsync(request.AreaId);
            if (area == null)
                throw new NotFoundException(nameof(Area), request.AreaId);
            
            solicitud.AreaId = request.AreaId;
        }

        await _unitOfWork.Solicitudes.UpdateAsync(solicitud);
        await _unitOfWork.SaveChangesAsync();

        // Recargar con relaciones para obtener valores actualizados
        var solicitudActualizada = await _unitOfWork.Solicitudes.GetByIdWithDetailsAsync(solicitud.Id);
        
        // Capturar valores DESPUÉS del cambio
        var valoresNuevos = new
        {
            Titulo = solicitudActualizada.Titulo,
            Descripcion = solicitudActualizada.Descripcion,
            TipoSolicitud = solicitudActualizada.TipoSolicitud?.Nombre,
            Prioridad = solicitudActualizada.Prioridad?.Nombre,
            Area = solicitudActualizada.Area?.Nombre
        };

        // Registrar auditoría
        await _auditoriaService.RegistrarAsync(
            usuarioId,
            usuario.NombreUsuario,
            usuario.Rol.ToString(),
            TipoAccion.Actualizar,
            "Solicitudes",
            id,
            $"PUT /api/solicitudes/{id}",
            "Solicitud modificada exitosamente",
            valoresAntiguos: valoresAntiguos,
            valoresNuevos: valoresNuevos,
            ipAddress: null,
            exitoso: true
        );

        return _mapper.Map<SolicitudResponse>(solicitudActualizada);
    }

    public async Task DeleteAsync(int id)
    {
        var solicitud = await _unitOfWork.Solicitudes.GetByIdAsync(id);
        
        if (solicitud == null)
            throw new NotFoundException(nameof(Solicitud), id);

        // Soft delete
        await _unitOfWork.Solicitudes.DeleteAsync(id);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<SolicitudResponse> AsignarGestorAsync(int solicitudId, int gestorId, int usuarioId)
    {
        var solicitud = await _unitOfWork.Solicitudes.GetByIdWithDetailsAsync(solicitudId);
        
        if (solicitud == null)
            throw new NotFoundException(nameof(Solicitud), solicitudId);

        var usuario = await _unitOfWork.Usuarios.GetByIdAsync(usuarioId);
        if (usuario == null)
            throw new UnauthorizedException();

        // REGLA DE NEGOCIO: Solo Admin o SuperAdmin pueden asignar gestores
        if (!usuario.TienePermisoAdministrativo())
        {
            throw new UnauthorizedException("Solo administradores pueden asignar gestores");
        }

        var gestor = await _unitOfWork.Usuarios.GetByIdAsync(gestorId);
        if (gestor == null)
            throw new NotFoundException(nameof(Usuario), gestorId);

        // Validar que el usuario es un gestor
        if (!gestor.EsGestor())
        {
            throw new ValidationException("El usuario seleccionado no es un gestor");
        }

        // REGLA DE NEGOCIO: El gestor solo puede trabajar solicitudes de su área
        if (gestor.AreaId != solicitud.AreaId)
        {
            throw new ValidationException("El gestor debe pertenecer al área de la solicitud");
        }

        // Asignar gestor (esto también cambia el estado a EnProceso si está en Nueva)
        solicitud.AsignarGestor(gestorId);

        await _unitOfWork.Solicitudes.UpdateAsync(solicitud);
        await _unitOfWork.SaveChangesAsync();

        // Notificar asignación manual de gestor por Admin
        var (destinatarios, notificacion) = await _notificacionService.NotificarGestorAsignadoAsync(
            solicitudId,
            solicitud.NumeroSolicitud,
            gestorId,
            esAutoasignacion: false);

        // Enviar notificación en tiempo real
        if (destinatarios.Any())
        {
            await _broadcaster.EnviarNotificacionAsync(destinatarios, notificacion);
        }

        var solicitudActualizada = await _unitOfWork.Solicitudes.GetByIdWithDetailsAsync(solicitud.Id);
        return _mapper.Map<SolicitudResponse>(solicitudActualizada);
    }

    public async Task<SolicitudResponse> TomarSolicitudAsync(int solicitudId, int gestorId)
    {
        var solicitud = await _unitOfWork.Solicitudes.GetByIdWithDetailsAsync(solicitudId);
        
        if (solicitud == null)
            throw new NotFoundException(nameof(Solicitud), solicitudId);

        var gestor = await _unitOfWork.Usuarios.GetByIdAsync(gestorId);
        if (gestor == null)
            throw new NotFoundException(nameof(Usuario), gestorId);

        // VALIDACIÓN 1: Verificar que es un gestor
        if (!gestor.EsGestor())
        {
            throw new ValidationException("Solo los gestores pueden tomar solicitudes");
        }

        // VALIDACIÓN 2: Verificar que el gestor tiene área asignada
        if (!gestor.AreaId.HasValue)
        {
            throw new ValidationException("El gestor no tiene un área asignada");
        }

        // VALIDACIÓN 3: Verificar que pertenece al área de la solicitud
        if (gestor.AreaId.Value != solicitud.AreaId)
        {
            throw new ValidationException("Solo puede tomar solicitudes de su área asignada");
        }

        // VALIDACIÓN 4: Verificar que la solicitud no tiene gestor asignado
        if (solicitud.GestorAsignadoId.HasValue)
        {
            throw new ValidationException("Esta solicitud ya tiene un gestor asignado");
        }

        // VALIDACIÓN 5: Verificar que está en estado Nueva
        if (solicitud.Estado != EstadoSolicitud.Nueva)
        {
            throw new ValidationException("Solo se pueden tomar solicitudes en estado Nueva");
        }

        // Asignar gestor (cambia estado a EnProceso automáticamente)
        solicitud.AsignarGestor(gestorId);

        await _unitOfWork.Solicitudes.UpdateAsync(solicitud);
        await _unitOfWork.SaveChangesAsync();

        var solicitudActualizada = await _unitOfWork.Solicitudes.GetByIdWithDetailsAsync(solicitud.Id);
        return _mapper.Map<SolicitudResponse>(solicitudActualizada);
    }

    public async Task<SolicitudResponse> CambiarEstadoAsync(int solicitudId, EstadoSolicitud nuevoEstado, int usuarioId)
    {
        var solicitud = await _unitOfWork.Solicitudes.GetByIdWithDetailsAsync(solicitudId);
        
        if (solicitud == null)
            throw new NotFoundException(nameof(Solicitud), solicitudId);

        var usuario = await _unitOfWork.Usuarios.GetByIdAsync(usuarioId);
        if (usuario == null)
            throw new UnauthorizedException();

        // Validar permisos según el rol
        if (!usuario.TienePermisoAdministrativo() && solicitud.GestorAsignadoId != usuarioId)
        {
            throw new UnauthorizedException("No tiene permisos para cambiar el estado de esta solicitud");
        }

        // Capturar estado ANTES del cambio
        var valoresAntiguos = new
        {
            Estado = solicitud.Estado.ToString(),
            NumeroSolicitud = solicitud.NumeroSolicitud
        };

        var estadoAnterior = solicitud.Estado;

        // Cambiar estado (valida transiciones)
        try
        {
            solicitud.CambiarEstado(nuevoEstado);
        }
        catch (InvalidOperationException ex)
        {
            throw new ValidationException(ex.Message);
        }

        await _unitOfWork.Solicitudes.UpdateAsync(solicitud);
        await _unitOfWork.SaveChangesAsync();

        // Notificar cambio de estado
        var (destinatarios, notificacion) = await _notificacionService.NotificarEstadoCambiadoAsync(
            solicitudId,
            solicitud.NumeroSolicitud,
            estadoAnterior,
            nuevoEstado,
            usuarioId,
            solicitud.GestorAsignadoId,
            solicitud.SolicitanteId);

        // Enviar notificación en tiempo real
        if (destinatarios.Any())
        {
            await _broadcaster.EnviarNotificacionAsync(destinatarios, notificacion);
        }

        // Capturar estado DESPUÉS del cambio
        var valoresNuevos = new
        {
            Estado = solicitud.Estado.ToString(),
            NumeroSolicitud = solicitud.NumeroSolicitud
        };

        // Registrar auditoría
        await _auditoriaService.RegistrarAsync(
            usuarioId,
            usuario.NombreUsuario,
            usuario.Rol.ToString(),
            TipoAccion.CambiarEstado,
            "Solicitudes",
            solicitudId,
            $"POST /api/solicitudes/{solicitudId}/cambiar-estado",
            $"Estado cambiado de {valoresAntiguos.Estado} a {valoresNuevos.Estado}",
            valoresAntiguos: valoresAntiguos,
            valoresNuevos: valoresNuevos,
            ipAddress: null,
            exitoso: true
        );

        var solicitudActualizada = await _unitOfWork.Solicitudes.GetByIdWithDetailsAsync(solicitud.Id);
        return _mapper.Map<SolicitudResponse>(solicitudActualizada);
    }

    public async Task<IEnumerable<SolicitudResponse>> GetBySolicitanteIdAsync(int solicitanteId)
    {
        var solicitudes = await _unitOfWork.Solicitudes.GetBySolicitanteIdAsync(solicitanteId);
        return _mapper.Map<IEnumerable<SolicitudResponse>>(solicitudes);
    }

    public async Task<IEnumerable<SolicitudResponse>> GetByGestorIdAsync(int gestorId)
    {
        var solicitudes = await _unitOfWork.Solicitudes.GetByGestorIdAsync(gestorId);
        return _mapper.Map<IEnumerable<SolicitudResponse>>(solicitudes);
    }

    public async Task<IEnumerable<SolicitudResponse>> GetByAreaIdAsync(int areaId)
    {
        var area = await _unitOfWork.Areas.GetByIdAsync(areaId);
        if (area == null)
            throw new NotFoundException(nameof(Area), areaId);

        var solicitudes = await _unitOfWork.Solicitudes.GetByAreaIdAsync(areaId);
        return _mapper.Map<IEnumerable<SolicitudResponse>>(solicitudes);
    }

    public async Task<IEnumerable<SolicitudResponse>> GetSolicitudesGestorAreaAsync(int gestorId)
    {
        var gestor = await _unitOfWork.Usuarios.GetByIdAsync(gestorId);
        
        if (gestor == null)
            throw new NotFoundException(nameof(Usuario), gestorId);

        if (!gestor.EsGestor())
            throw new ValidationException("El usuario no es un gestor");

        if (!gestor.AreaId.HasValue)
            throw new ValidationException("El gestor no tiene un área asignada");

        // Obtener todas las solicitudes del área
        var todasSolicitudesArea = await _unitOfWork.Solicitudes.GetByAreaIdAsync(gestor.AreaId.Value);

        // Filtrar: solo las asignadas al gestor + las sin asignar
        var solicitudesFiltradas = todasSolicitudesArea
            .Where(s => s.GestorAsignadoId == gestorId || s.GestorAsignadoId == null)
            .ToList();

        return _mapper.Map<IEnumerable<SolicitudResponse>>(solicitudesFiltradas);
    }

    public async Task<IEnumerable<SolicitudResponse>> GetByFiltrosAsync(
        EstadoSolicitud? estado, 
        int? prioridadId, 
        DateTime? fechaDesde, 
        DateTime? fechaHasta)
    {
        var solicitudes = await _unitOfWork.Solicitudes.GetByFiltrosAsync(estado, prioridadId, fechaDesde, fechaHasta);
        return _mapper.Map<IEnumerable<SolicitudResponse>>(solicitudes);
    }
}
