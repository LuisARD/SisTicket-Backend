using SisTicket.Core.Domain.Common;

namespace SisTicket.Core.Domain.Entities;

public class Comentario : BaseEntity
{
    public string Texto { get; set; } = string.Empty;
    public int SolicitudId { get; set; }
    public int UsuarioId { get; set; }
    
    public Solicitud Solicitud { get; set; } = null!;
    public Usuario Usuario { get; set; } = null!;
}
