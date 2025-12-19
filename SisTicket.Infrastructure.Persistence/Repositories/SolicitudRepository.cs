using Microsoft.EntityFrameworkCore;
using SisTicket.Core.Domain.Entities;
using SisTicket.Core.Domain.Enums;
using SisTicket.Core.Domain.Interfaces;
using SisTicket.Infrastructure.Persistence.Context;

namespace SisTicket.Infrastructure.Persistence.Repositories;

public class SolicitudRepository : GenericRepository<Solicitud>, ISolicitudRepository
{
    public SolicitudRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<Solicitud?> GetByNumeroSolicitudAsync(string numeroSolicitud)
    {
        return await _dbSet
            .Include(s => s.Solicitante)
            .Include(s => s.GestorAsignado)
            .Include(s => s.TipoSolicitud)
            .Include(s => s.Prioridad)
            .Include(s => s.Area)
            .FirstOrDefaultAsync(s => s.NumeroSolicitud == numeroSolicitud && s.Activo);
    }

    public async Task<Solicitud?> GetByIdWithDetailsAsync(int id)
    {
        return await _dbSet
            .Include(s => s.Solicitante)
            .Include(s => s.GestorAsignado)
            .Include(s => s.TipoSolicitud)
            .Include(s => s.Prioridad)
            .Include(s => s.Area)
            .Include(s => s.Comentarios)
                .ThenInclude(c => c.Usuario)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<IEnumerable<Solicitud>> GetBySolicitanteIdAsync(int solicitanteId)
    {
        return await _dbSet
            .Include(s => s.Solicitante)
            .Include(s => s.GestorAsignado)
            .Include(s => s.TipoSolicitud)
            .Include(s => s.Prioridad)
            .Include(s => s.Area)
            .Where(s => s.SolicitanteId == solicitanteId && s.Activo)
            .OrderByDescending(s => s.FechaCreacion)
            .ToListAsync();
    }

    public async Task<IEnumerable<Solicitud>> GetByGestorIdAsync(int gestorId)
    {
        return await _dbSet
            .Include(s => s.Solicitante)
            .Include(s => s.GestorAsignado)
            .Include(s => s.TipoSolicitud)
            .Include(s => s.Prioridad)
            .Include(s => s.Area)
            .Where(s => s.GestorAsignadoId == gestorId && s.Activo)
            .OrderByDescending(s => s.FechaCreacion)
            .ToListAsync();
    }

    public async Task<IEnumerable<Solicitud>> GetByAreaIdAsync(int areaId)
    {
        return await _dbSet
            .Include(s => s.Solicitante)
            .Include(s => s.GestorAsignado)
            .Include(s => s.TipoSolicitud)
            .Include(s => s.Prioridad)
            .Include(s => s.Area)
            .Where(s => s.AreaId == areaId && s.Activo)
            .OrderByDescending(s => s.FechaCreacion)
            .ToListAsync();
    }

    public async Task<IEnumerable<Solicitud>> GetByEstadoAsync(EstadoSolicitud estado)
    {
        return await _dbSet
            .Include(s => s.Solicitante)
            .Include(s => s.GestorAsignado)
            .Include(s => s.TipoSolicitud)
            .Include(s => s.Prioridad)
            .Include(s => s.Area)
            .Where(s => s.Estado == estado && s.Activo)
            .OrderByDescending(s => s.FechaCreacion)
            .ToListAsync();
    }

    public async Task<IEnumerable<Solicitud>> GetByFiltrosAsync(
        EstadoSolicitud? estado = null,
        int? prioridadId = null,
        DateTime? fechaDesde = null,
        DateTime? fechaHasta = null)
    {
        var query = _dbSet
            .Include(s => s.Solicitante)
            .Include(s => s.GestorAsignado)
            .Include(s => s.TipoSolicitud)
            .Include(s => s.Prioridad)
            .Include(s => s.Area)
            .Where(s => s.Activo);

        if (estado.HasValue)
        {
            query = query.Where(s => s.Estado == estado.Value);
        }

        if (prioridadId.HasValue)
        {
            query = query.Where(s => s.PrioridadId == prioridadId.Value);
        }

        if (fechaDesde.HasValue)
        {
            query = query.Where(s => s.FechaCreacion >= fechaDesde.Value);
        }

        if (fechaHasta.HasValue)
        {
            query = query.Where(s => s.FechaCreacion <= fechaHasta.Value);
        }

        return await query
            .OrderByDescending(s => s.FechaCreacion)
            .ToListAsync();
    }

    public async Task<string> GenerarNumeroSolicitudAsync()
    {
        var año = DateTime.UtcNow.Year;
        var prefijo = $"SOL-{año}-";

        var ultimaSolicitud = await _dbSet
            .Where(s => s.NumeroSolicitud.StartsWith(prefijo))
            .OrderByDescending(s => s.NumeroSolicitud)
            .FirstOrDefaultAsync();

        int numeroConsecutivo = 1;

        if (ultimaSolicitud != null)
        {
            var ultimoNumero = ultimaSolicitud.NumeroSolicitud.Replace(prefijo, "");
            if (int.TryParse(ultimoNumero, out int numero))
            {
                numeroConsecutivo = numero + 1;
            }
        }

        return $"{prefijo}{numeroConsecutivo:D6}";
    }

    public override async Task<Solicitud?> GetByIdAsync(int id)
    {
        return await GetByIdWithDetailsAsync(id);
    }

    public override async Task<IEnumerable<Solicitud>> GetAllAsync()
    {
        return await _dbSet
            .Include(s => s.Solicitante)
            .Include(s => s.GestorAsignado)
            .Include(s => s.TipoSolicitud)
            .Include(s => s.Prioridad)
            .Include(s => s.Area)
            .Where(s => s.Activo)
            .OrderByDescending(s => s.FechaCreacion)
            .ToListAsync();
    }
}
