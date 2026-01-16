using SisTicket.Core.Domain.Entities;

namespace SisTicket.Core.Domain.Interfaces;

public interface IUsuarioRepository : IGenericRepository<Usuario>
{
    Task<Usuario?> GetByEmailAsync(string email);
    Task<Usuario?> GetByNombreUsuarioAsync(string nombreUsuario);
    Task<bool> ExistsByEmailAsync(string email);
    Task<bool> ExistsByNombreUsuarioAsync(string nombreUsuario);
    Task<IEnumerable<Usuario>> GetByAreaIdAsync(int areaId);
    Task<IEnumerable<Usuario>> GetGestoresByAreaIdAsync(int areaId);
    Task<IEnumerable<Usuario>> GetAdministradoresAsync();
}
