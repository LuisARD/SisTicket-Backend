using SisTicket.Core.Domain.Common;

namespace SisTicket.Core.Domain.Entities;

public class Area : BaseEntity
{
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    
    public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
    public ICollection<Solicitud> Solicitudes { get; set; } = new List<Solicitud>();
}
