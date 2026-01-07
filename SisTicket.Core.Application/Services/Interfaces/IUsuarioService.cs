using SisTicket.Core.Application.DTOs.Usuario;

namespace SisTicket.Core.Application.Services.Interfaces;

public interface IUsuarioService
{
    Task<IEnumerable<UsuarioResponse>> GetAllAsync();
    Task<UsuarioResponse> GetByIdAsync(int id);
    Task<UsuarioResponse> CreateAsync(UsuarioRequest request, int usuarioActualId);
    Task<UsuarioResponse> UpdateAsync(int id, UsuarioRequest request, int usuarioActualId);
    Task DeleteAsync(int id, int usuarioActualId);
    Task<IEnumerable<UsuarioResponse>> GetByAreaIdAsync(int areaId);
    Task<IEnumerable<UsuarioResponse>> GetGestoresByAreaIdAsync(int areaId);
    Task<UsuarioResponse> CambiarEstadoAsync(int id, bool activo, int usuarioActualId);
}
