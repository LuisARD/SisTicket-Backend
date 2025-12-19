using Microsoft.EntityFrameworkCore;
using SisTicket.Core.Domain.Entities;
using SisTicket.Core.Domain.Interfaces;
using SisTicket.Infrastructure.Persistence.Context;

namespace SisTicket.Infrastructure.Persistence.Repositories;

public class PrioridadRepository : GenericRepository<Prioridad>, IPrioridadRepository
{
    public PrioridadRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<bool> ExistsByNombreAsync(string nombre)
    {
        return await _dbSet.AnyAsync(p => p.Nombre == nombre && p.Activo);
    }

    public async Task<IEnumerable<Prioridad>> GetOrderedByNivelAsync()
    {
        return await _dbSet
            .Where(p => p.Activo)
            .OrderBy(p => p.Nivel)
            .ToListAsync();
    }

    public override async Task<IEnumerable<Prioridad>> GetAllAsync()
    {
        return await GetOrderedByNivelAsync();
    }
}
