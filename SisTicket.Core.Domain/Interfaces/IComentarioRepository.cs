using SisTicket.Core.Domain.Entities;

namespace SisTicket.Core.Domain.Interfaces;

public interface IComentarioRepository : IGenericRepository<Comentario>
{
    Task<IEnumerable<Comentario>> GetBySolicitudIdAsync(int solicitudId);
    Task<IEnumerable<Comentario>> GetByUsuarioIdAsync(int usuarioId);
}
