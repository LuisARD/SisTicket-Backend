using Microsoft.EntityFrameworkCore;
using SisTicket.Core.Domain.Entities;
using SisTicket.Core.Domain.Interfaces;
using SisTicket.Infrastructure.Persistence.Context;

namespace SisTicket.Infrastructure.Persistence.Repositories;

public class ComentarioRepository : GenericRepository<Comentario>, IComentarioRepository
{
    public ComentarioRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Comentario>> GetBySolicitudIdAsync(int solicitudId)
    {
        return await _dbSet
            .Include(c => c.Usuario)
            .Include(c => c.Solicitud)
            .Where(c => c.SolicitudId == solicitudId && c.Activo)
            .OrderBy(c => c.FechaCreacion)
            .ToListAsync();
    }

    public async Task<IEnumerable<Comentario>> GetByUsuarioIdAsync(int usuarioId)
    {
        return await _dbSet
            .Include(c => c.Usuario)
            .Include(c => c.Solicitud)
            .Where(c => c.UsuarioId == usuarioId && c.Activo)
            .OrderByDescending(c => c.FechaCreacion)
            .ToListAsync();
    }

    public override async Task<Comentario?> GetByIdAsync(int id)
    {
        return await _dbSet
            .Include(c => c.Usuario)
            .Include(c => c.Solicitud)
            .FirstOrDefaultAsync(c => c.Id == id);
    }
}
