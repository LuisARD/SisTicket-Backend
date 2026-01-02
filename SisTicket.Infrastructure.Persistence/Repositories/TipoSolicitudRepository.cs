using Microsoft.EntityFrameworkCore;
using SisTicket.Core.Domain.Entities;
using SisTicket.Core.Domain.Interfaces;
using SisTicket.Infrastructure.Persistence.Context;

namespace SisTicket.Infrastructure.Persistence.Repositories;

public class TipoSolicitudRepository : GenericRepository<TipoSolicitud>, ITipoSolicitudRepository
{
    public TipoSolicitudRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<bool> ExistsByNombreAsync(string nombre)
    {
        return await _dbSet.AnyAsync(t => t.Nombre == nombre && t.Activo);
    }

    public async Task<bool> ExistsByNombreAndAreaAsync(string nombre, int areaId)
    {
        return await _dbSet.AnyAsync(t => t.Nombre == nombre && t.AreaId == areaId && t.Activo);
    }

    public async Task<bool> ExistsByNombreAndAreaExcludingIdAsync(string nombre, int areaId, int excludeId)
    {
        return await _dbSet.AnyAsync(t => t.Nombre == nombre && t.AreaId == areaId && t.Id != excludeId && t.Activo);
    }

    public override async Task<TipoSolicitud?> GetByIdAsync(int id)
    {
        return await _dbSet
            .Include(t => t.Area)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public override async Task<IEnumerable<TipoSolicitud>> GetAllAsync()
    {
        return await _dbSet
            .Include(t => t.Area)
            .Where(t => t.Activo)
            .OrderBy(t => t.Area.Nombre)
            .ThenBy(t => t.Nombre)
            .ToListAsync();
    }
}
