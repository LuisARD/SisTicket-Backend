namespace SisTicket.Core.Domain.Common;

public abstract class BaseEntity
{
    public int Id { get; set; }
    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaModificacion { get; set; }
    public bool Activo { get; set; } = true;
    public bool Eliminado { get; set; } = false;
    public DateTime? FechaEliminacion { get; set; }
}
