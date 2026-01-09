using Microsoft.EntityFrameworkCore;
using SisTicket.Core.Domain.Entities;
using SisTicket.Core.Domain.Enums;
using SisTicket.Core.Domain.Interfaces;
using SisTicket.Infrastructure.Persistence.Context;

namespace SisTicket.Infrastructure.Persistence.Repositories;

public class AuditoriaLogRepository : GenericRepository<AuditoriaLog>, IAuditoriaLogRepository
{
    public AuditoriaLogRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<AuditoriaLog>> GetLogsAsync(
        int? usuarioId = null,
        TipoAccion? tipoAccion = null,
        string? entidad = null,
        DateTime? fechaDesde = null,
        DateTime? fechaHasta = null,
        int pageNumber = 1,
        int pageSize = 50)
    {
        var query = _dbSet
            .Include(a => a.Usuario)
            .AsQueryable();

        // Aplicar filtros
        if (usuarioId.HasValue)
        {
            query = query.Where(a => a.UsuarioId == usuarioId.Value);
        }

        if (tipoAccion.HasValue)
        {
            query = query.Where(a => a.TipoAccion == tipoAccion.Value);
        }

        if (!string.IsNullOrWhiteSpace(entidad))
        {
            query = query.Where(a => a.Entidad == entidad);
        }

        if (fechaDesde.HasValue)
        {
            query = query.Where(a => a.FechaHoraUtc >= fechaDesde.Value);
        }

        if (fechaHasta.HasValue)
        {
            query = query.Where(a => a.FechaHoraUtc <= fechaHasta.Value);
        }

        // Paginación
        var logs = await query
            .OrderByDescending(a => a.FechaHoraUtc)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return logs;
    }

    public async Task<int> GetTotalLogsAsync(
        int? usuarioId = null,
        TipoAccion? tipoAccion = null,
        string? entidad = null,
        DateTime? fechaDesde = null,
        DateTime? fechaHasta = null)
    {
        var query = _dbSet.AsQueryable();

        // Aplicar los mismos filtros
        if (usuarioId.HasValue)
        {
            query = query.Where(a => a.UsuarioId == usuarioId.Value);
        }

        if (tipoAccion.HasValue)
        {
            query = query.Where(a => a.TipoAccion == tipoAccion.Value);
        }

        if (!string.IsNullOrWhiteSpace(entidad))
        {
            query = query.Where(a => a.Entidad == entidad);
        }

        if (fechaDesde.HasValue)
        {
            query = query.Where(a => a.FechaHoraUtc >= fechaDesde.Value);
        }

        if (fechaHasta.HasValue)
        {
            query = query.Where(a => a.FechaHoraUtc <= fechaHasta.Value);
        }

        return await query.CountAsync();
    }
}
