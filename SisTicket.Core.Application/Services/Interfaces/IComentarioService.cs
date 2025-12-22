using SisTicket.Core.Application.DTOs.Comentario;

namespace SisTicket.Core.Application.Services.Interfaces;

public interface IComentarioService
{
    Task<IEnumerable<ComentarioResponse>> GetBySolicitudIdAsync(int solicitudId);
    Task<ComentarioResponse> CreateAsync(ComentarioRequest request, int usuarioId);
    Task DeleteAsync(int id, int usuarioId);
}
