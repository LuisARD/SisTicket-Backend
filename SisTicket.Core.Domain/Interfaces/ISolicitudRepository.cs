using SisTicket.Core.Domain.Entities;
using SisTicket.Core.Domain.Enums;

namespace SisTicket.Core.Domain.Interfaces;

public interface ISolicitudRepository : IGenericRepository<Solicitud>
{
    Task<Solicitud?> GetByNumeroSolicitudAsync(string numeroSolicitud);
    Task<Solicitud?> GetByIdWithDetailsAsync(int id);
    Task<IEnumerable<Solicitud>> GetBySolicitanteIdAsync(int solicitanteId);
    Task<IEnumerable<Solicitud>> GetByGestorIdAsync(int gestorId);
    Task<IEnumerable<Solicitud>> GetByAreaIdAsync(int areaId);
    Task<IEnumerable<Solicitud>> GetByEstadoAsync(EstadoSolicitud estado);
    Task<IEnumerable<Solicitud>> GetByFiltrosAsync(
        EstadoSolicitud? estado = null,
        int? prioridadId = null,
        DateTime? fechaDesde = null,
        DateTime? fechaHasta = null);
    Task<string> GenerarNumeroSolicitudAsync();
    Task<bool> TieneAreaSolicitudesActivasAsync(int areaId);
    Task<bool> TieneTipoSolicitudSolicitudesActivasAsync(int tipoSolicitudId);
    Task<bool> TienePrioridadSolicitudesActivasAsync(int prioridadId);
}
