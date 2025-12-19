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

    public override async Task<IEnumerable<TipoSolicitud>> GetAllAsync()
    {
        return await _dbSet
            .Where(t => t.Activo)
            .OrderBy(t => t.Nombre)
            .ToListAsync();
    }
}
