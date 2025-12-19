using SisTicket.Core.Domain.Entities;

namespace SisTicket.Core.Domain.Interfaces;

public interface IAreaRepository : IGenericRepository<Area>
{
    Task<Area?> GetByNombreAsync(string nombre);
    Task<bool> ExistsByNombreAsync(string nombre);
}
