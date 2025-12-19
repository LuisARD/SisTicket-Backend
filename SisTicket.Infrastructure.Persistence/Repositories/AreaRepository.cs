using Microsoft.EntityFrameworkCore;
using SisTicket.Core.Domain.Entities;
using SisTicket.Core.Domain.Interfaces;
using SisTicket.Infrastructure.Persistence.Context;

namespace SisTicket.Infrastructure.Persistence.Repositories;

public class AreaRepository : GenericRepository<Area>, IAreaRepository
{
    public AreaRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<Area?> GetByNombreAsync(string nombre)
    {
        return await _dbSet
            .FirstOrDefaultAsync(a => a.Nombre == nombre && a.Activo);
    }

    public async Task<bool> ExistsByNombreAsync(string nombre)
    {
        return await _dbSet.AnyAsync(a => a.Nombre == nombre && a.Activo);
    }

    public override async Task<IEnumerable<Area>> GetAllAsync()
    {
        return await _dbSet
            .Where(a => a.Activo)
            .OrderBy(a => a.Nombre)
            .ToListAsync();
    }
}
