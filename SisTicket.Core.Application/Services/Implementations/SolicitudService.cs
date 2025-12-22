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

    public SolicitudService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
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

        var solicitudActualizada = await _unitOfWork.Solicitudes.GetByIdWithDetailsAsync(solicitud.Id);
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

        // Validar permisos según elrol
        if (!usuario.TienePermisoAdministrativo() && solicitud.GestorAsignadoId != usuarioId)
        {
            throw new UnauthorizedException("No tiene permisos para cambiar el estado de esta solicitud");
        }

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
