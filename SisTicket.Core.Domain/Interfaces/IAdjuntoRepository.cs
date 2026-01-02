using SisTicket.Core.Domain.Entities;

namespace SisTicket.Core.Domain.Interfaces;

public interface IAdjuntoRepository : IGenericRepository<Adjunto>
{
    Task<IEnumerable<Adjunto>> GetBySolicitudIdAsync(int solicitudId);
    Task<Adjunto?> GetByIdWithRelacionesAsync(int id);
    Task<int> ContarPorSolicitudAsync(int solicitudId);
    Task<bool> ExisteAdjuntoEnSolicitudAsync(int adjuntoId, int solicitudId);
}
