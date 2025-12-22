using SisTicket.Core.Application.DTOs.Solicitud;
using SisTicket.Core.Domain.Enums;

namespace SisTicket.Core.Application.Services.Interfaces;

public interface ISolicitudService
{
    Task<IEnumerable<SolicitudResponse>> GetAllAsync();
    Task<SolicitudResponse> GetByIdAsync(int id);
    Task<SolicitudResponse> CreateAsync(SolicitudRequest request, int solicitanteId);
    Task<SolicitudResponse> UpdateAsync(int id, SolicitudRequest request, int usuarioId);
    Task DeleteAsync(int id);
    Task<SolicitudResponse> AsignarGestorAsync(int solicitudId, int gestorId, int usuarioId);
    Task<SolicitudResponse> CambiarEstadoAsync(int solicitudId, EstadoSolicitud nuevoEstado, int usuarioId);
    Task<IEnumerable<SolicitudResponse>> GetBySolicitanteIdAsync(int solicitanteId);
    Task<IEnumerable<SolicitudResponse>> GetByGestorIdAsync(int gestorId);
    Task<IEnumerable<SolicitudResponse>> GetByFiltrosAsync(EstadoSolicitud? estado, int? prioridadId, DateTime? fechaDesde, DateTime? fechaHasta);
}
