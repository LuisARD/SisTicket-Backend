using SisTicket.Core.Application.DTOs.TipoSolicitud;

namespace SisTicket.Core.Application.Services.Interfaces;

public interface ITipoSolicitudService
{
    Task<IEnumerable<TipoSolicitudResponse>> GetAllAsync();
    Task<IEnumerable<TipoSolicitudResponse>> GetByAreaIdAsync(int areaId);
    Task<TipoSolicitudResponse> GetByIdAsync(int id);
    Task<TipoSolicitudResponse> CreateAsync(TipoSolicitudRequest request);
    Task<TipoSolicitudResponse> UpdateAsync(int id, TipoSolicitudRequest request);
    Task DeleteAsync(int id);
}
