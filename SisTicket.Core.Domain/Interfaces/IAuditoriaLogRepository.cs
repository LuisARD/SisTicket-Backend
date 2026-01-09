using SisTicket.Core.Domain.Entities;
using SisTicket.Core.Domain.Enums;

namespace SisTicket.Core.Domain.Interfaces;

public interface IAuditoriaLogRepository : IGenericRepository<AuditoriaLog>
{
    Task<IEnumerable<AuditoriaLog>> GetLogsAsync(
        int? usuarioId = null,
        TipoAccion? tipoAccion = null,
        string? entidad = null,
        DateTime? fechaDesde = null,
        DateTime? fechaHasta = null,
        int pageNumber = 1,
        int pageSize = 50);
    
    Task<int> GetTotalLogsAsync(
        int? usuarioId = null,
        TipoAccion? tipoAccion = null,
        string? entidad = null,
        DateTime? fechaDesde = null,
        DateTime? fechaHasta = null);
}
