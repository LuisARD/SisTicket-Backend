using AutoMapper;
using SisTicket.Core.Application.DTOs.Comentario;
using SisTicket.Core.Application.Exceptions;
using SisTicket.Core.Application.Services.Interfaces;
using SisTicket.Core.Domain.Entities;
using SisTicket.Core.Domain.Interfaces;

namespace SisTicket.Core.Application.Services.Implementations;

public class ComentarioService : IComentarioService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly INotificacionService _notificacionService;
    private readonly INotificacionBroadcaster _broadcaster;

    public ComentarioService(
        IUnitOfWork unitOfWork, 
        IMapper mapper,
        INotificacionService notificacionService,
        INotificacionBroadcaster broadcaster)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _notificacionService = notificacionService;
        _broadcaster = broadcaster;
    }

    public async Task<IEnumerable<ComentarioResponse>> GetBySolicitudIdAsync(int solicitudId)
    {
        var solicitud = await _unitOfWork.Solicitudes.GetByIdAsync(solicitudId);
        
        if (solicitud == null)
            throw new NotFoundException(nameof(Solicitud), solicitudId);

        var comentarios = await _unitOfWork.Comentarios.GetBySolicitudIdAsync(solicitudId);
        return _mapper.Map<IEnumerable<ComentarioResponse>>(comentarios);
    }

    public async Task<ComentarioResponse> CreateAsync(ComentarioRequest request, int usuarioId)
    {
        // Validar que existe la solicitud
        var solicitud = await _unitOfWork.Solicitudes.GetByIdWithDetailsAsync(request.SolicitudId);
        
        if (solicitud == null)
            throw new NotFoundException(nameof(Solicitud), request.SolicitudId);

        // Validar que existe el usuario
        var usuario = await _unitOfWork.Usuarios.GetByIdAsync(usuarioId);
        
        if (usuario == null)
            throw new NotFoundException(nameof(Usuario), usuarioId);

        // REGLA DE NEGOCIO: Pueden comentar:
        // 1. Solicitante: SOLO en sus propias solicitudes
        // 2. Gestor: Solo en solicitudes de su área
        // 3. Admin/SuperAdmin: En cualquier solicitud
        if (!PuedeComentarEnSolicitud(usuario, solicitud))
        {
            throw new UnauthorizedException(
                "No tiene permisos para comentar en esta solicitud.");
        }

        var comentario = new Comentario
        {
            Texto = request.Texto,
            SolicitudId = request.SolicitudId,
            UsuarioId = usuarioId
        };

        await _unitOfWork.Comentarios.AddAsync(comentario);
        await _unitOfWork.SaveChangesAsync();

        // Notificar nuevo comentario
        var (destinatarios, notificacion) = await _notificacionService.NotificarComentarioAgregadoAsync(
            comentario.SolicitudId,
            solicitud.NumeroSolicitud,
            usuarioId,
            solicitud.GestorAsignadoId,
            solicitud.SolicitanteId);

        // Enviar notificación en tiempo real
        if (destinatarios.Any())
        {
            await _broadcaster.EnviarNotificacionAsync(destinatarios, notificacion);
        }

        var comentarioCreado = await _unitOfWork.Comentarios.GetByIdAsync(comentario.Id);
        return _mapper.Map<ComentarioResponse>(comentarioCreado);
    }

    public async Task DeleteAsync(int id, int usuarioId)
    {
        var comentario = await _unitOfWork.Comentarios.GetByIdAsync(id);
        
        if (comentario == null)
            throw new NotFoundException(nameof(Comentario), id);

        var usuario = await _unitOfWork.Usuarios.GetByIdAsync(usuarioId);
        
        if (usuario == null)
            throw new UnauthorizedException();

        // REGLA DE NEGOCIO: Pueden eliminar comentarios:
        // 1. El autor del comentario
        // 2. Admin
        // 3. SuperAdmin
        if (!PuedeEliminarComentario(usuario, comentario))
        {
            throw new UnauthorizedException(
                "No tiene permisos para eliminar este comentario. " +
                "Solo el autor, Admin o SuperAdmin pueden eliminarlo.");
        }

        await _unitOfWork.Comentarios.DeleteAsync(id);
        await _unitOfWork.SaveChangesAsync();
    }

    // Método privado para validar permisos de creación de comentarios
    private bool PuedeComentarEnSolicitud(Usuario usuario, Solicitud solicitud)
    {
        // 1. Solicitante puede comentar SOLO en sus propias solicitudes
        if (usuario.EsSolicitante())
        {
            return solicitud.SolicitanteId == usuario.Id;
        }

        // 2. Gestor puede comentar solo en solicitudes de su área
        if (usuario.EsGestor())
        {
            return usuario.AreaId.HasValue && usuario.AreaId.Value == solicitud.AreaId;
        }

        // 3. Admin/SuperAdmin pueden comentar en cualquier solicitud
        if (usuario.TienePermisoAdministrativo())
        {
            return true;
        }

        return false;
    }

    // Método privado para validar permisos de eliminación de comentarios
    private bool PuedeEliminarComentario(Usuario usuario, Comentario comentario)
    {
        // 1. Es el autor del comentario
        if (usuario.Id == comentario.UsuarioId)
            return true;

        // 2. Es Admin o SuperAdmin
        if (usuario.TienePermisoAdministrativo())
            return true;

        return false;
    }
}
