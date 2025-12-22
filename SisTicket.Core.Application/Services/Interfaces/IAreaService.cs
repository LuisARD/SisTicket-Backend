using SisTicket.Core.Application.DTOs.Area;

namespace SisTicket.Core.Application.Services.Interfaces;

public interface IAreaService
{
    Task<IEnumerable<AreaResponse>> GetAllAsync();
    Task<AreaResponse> GetByIdAsync(int id);
    Task<AreaResponse> CreateAsync(AreaRequest request);
    Task<AreaResponse> UpdateAsync(int id, AreaRequest request);
    Task DeleteAsync(int id);
}
