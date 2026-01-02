using Microsoft.EntityFrameworkCore;
using SisTicket.Core.Domain.Entities;
using SisTicket.Core.Domain.Interfaces;
using SisTicket.Infrastructure.Persistence.Context;

namespace SisTicket.Infrastructure.Persistence.Repositories;

public class AdjuntoRepository : GenericRepository<Adjunto>, IAdjuntoRepository
{
    public AdjuntoRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Adjunto>> GetBySolicitudIdAsync(int solicitudId)
    {
        return await _dbSet
            .Include(a => a.CargadoPor)
            .Where(a => a.SolicitudId == solicitudId && a.Activo)
            .OrderByDescending(a => a.FechaCreacion)
            .ToListAsync();
    }

    public async Task<Adjunto?> GetByIdWithRelacionesAsync(int id)
    {
        return await _dbSet
            .Include(a => a.Solicitud)
            .Include(a => a.CargadoPor)
            .FirstOrDefaultAsync(a => a.Id == id && a.Activo);
    }

    public async Task<int> ContarPorSolicitudAsync(int solicitudId)
    {
        return await _dbSet
            .Where(a => a.SolicitudId == solicitudId && a.Activo)
            .CountAsync();
    }

    public async Task<bool> ExisteAdjuntoEnSolicitudAsync(int adjuntoId, int solicitudId)
    {
        return await _dbSet
            .AnyAsync(a => a.Id == adjuntoId && a.SolicitudId == solicitudId && a.Activo);
    }
}
