using SisTicket.Core.Application.DTOs.Prioridad;

namespace SisTicket.Core.Application.Services.Interfaces;

public interface IPrioridadService
{
    Task<IEnumerable<PrioridadResponse>> GetAllAsync();
    Task<PrioridadResponse> GetByIdAsync(int id);
    Task<PrioridadResponse> CreateAsync(PrioridadRequest request);
    Task<PrioridadResponse> UpdateAsync(int id, PrioridadRequest request);
    Task DeleteAsync(int id);
}
