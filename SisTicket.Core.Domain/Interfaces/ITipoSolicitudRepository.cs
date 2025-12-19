using SisTicket.Core.Domain.Entities;

namespace SisTicket.Core.Domain.Interfaces;

public interface ITipoSolicitudRepository : IGenericRepository<TipoSolicitud>
{
    Task<bool> ExistsByNombreAsync(string nombre);
}
