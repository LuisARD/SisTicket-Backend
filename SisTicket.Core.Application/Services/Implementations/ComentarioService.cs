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

    public ComentarioService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
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

        // REGLA DE NEGOCIO: Solo pueden comentar:
        // 1. Los gestores del área de la solicitud
        // 2. Admin o SuperAdmin
        // NOTA: El solicitante solo puede VER comentarios, NO crearlos
        if (!PuedeComentarEnSolicitud(usuario, solicitud))
        {
            throw new UnauthorizedException(
                "No tiene permisos para comentar en esta solicitud. " +
                "Solo gestores del área, Admin o SuperAdmin pueden comentar.");
        }

        var comentario = new Comentario
        {
            Texto = request.Texto,
            SolicitudId = request.SolicitudId,
            UsuarioId = usuarioId
        };

        await _unitOfWork.Comentarios.AddAsync(comentario);
        await _unitOfWork.SaveChangesAsync();

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
        // 1. Es gestor del área de la solicitud
        if (usuario.EsGestor() && usuario.AreaId == solicitud.AreaId)
            return true;

        // 2. Es Admin o SuperAdmin
        if (usuario.TienePermisoAdministrativo())
            return true;

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
