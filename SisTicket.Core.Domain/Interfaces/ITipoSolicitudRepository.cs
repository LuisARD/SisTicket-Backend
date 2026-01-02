using SisTicket.Core.Domain.Entities;

namespace SisTicket.Core.Domain.Interfaces;

public interface ITipoSolicitudRepository : IGenericRepository<TipoSolicitud>
{
    Task<bool> ExistsByNombreAsync(string nombre);
    Task<bool> ExistsByNombreAndAreaAsync(string nombre, int areaId);
    Task<bool> ExistsByNombreAndAreaExcludingIdAsync(string nombre, int areaId, int excludeId);
}
