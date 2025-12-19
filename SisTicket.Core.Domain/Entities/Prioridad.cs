using SisTicket.Core.Domain.Common;

namespace SisTicket.Core.Domain.Entities;

public class Prioridad : BaseEntity
{
    public string Nombre { get; set; } = string.Empty;
    public int Nivel { get; set; }
    public string? Descripcion { get; set; }
    
    public ICollection<Solicitud> Solicitudes { get; set; } = new List<Solicitud>();
}
