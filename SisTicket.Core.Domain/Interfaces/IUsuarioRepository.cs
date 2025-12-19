using SisTicket.Core.Domain.Entities;

namespace SisTicket.Core.Domain.Interfaces;

public interface IUsuarioRepository : IGenericRepository<Usuario>
{
    Task<Usuario?> GetByEmailAsync(string email);
    Task<bool> ExistsByEmailAsync(string email);
    Task<IEnumerable<Usuario>> GetByAreaIdAsync(int areaId);
    Task<IEnumerable<Usuario>> GetGestoresByAreaIdAsync(int areaId);
}
