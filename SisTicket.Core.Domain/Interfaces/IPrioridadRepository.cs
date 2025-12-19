using SisTicket.Core.Domain.Entities;

namespace SisTicket.Core.Domain.Interfaces;

public interface IPrioridadRepository : IGenericRepository<Prioridad>
{
    Task<bool> ExistsByNombreAsync(string nombre);
    Task<IEnumerable<Prioridad>> GetOrderedByNivelAsync();
}
