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
        var solicitud = await _unitOfWork.Solicitudes.GetByIdAsync(request.SolicitudId);
        
        if (solicitud == null)
            throw new NotFoundException(nameof(Solicitud), request.SolicitudId);

        var usuario = await _unitOfWork.Usuarios.GetByIdAsync(usuarioId);
        
        if (usuario == null)
            throw new NotFoundException(nameof(Usuario), usuarioId);

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

        // Solo el autor del comentario o un admin/superadmin puede eliminarlo
        if (comentario.UsuarioId != usuarioId && !usuario.TienePermisoAdministrativo())
            throw new UnauthorizedException("No tiene permisos para eliminar este comentario");

        await _unitOfWork.Comentarios.DeleteAsync(id);
        await _unitOfWork.SaveChangesAsync();
    }
}
