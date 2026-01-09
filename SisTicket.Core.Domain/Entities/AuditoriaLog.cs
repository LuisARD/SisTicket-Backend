using SisTicket.Core.Domain.Common;
using SisTicket.Core.Domain.Enums;

namespace SisTicket.Core.Domain.Entities;

public class AuditoriaLog : BaseEntity
{
    public int? UsuarioId { get; set; }
    public string? NombreUsuario { get; set; }
    public string? Rol { get; set; }
    public TipoAccion TipoAccion { get; set; }
    public string Entidad { get; set; } = string.Empty;
    public int? EntidadId { get; set; }
    public string Accion { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public string? ValoresAntiguos { get; set; }
    public string? ValoresNuevos { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public bool Exitoso { get; set; } = true;
    public string? MensajeError { get; set; }
    public DateTime FechaHoraUtc { get; set; }
    
    // Relación
    public Usuario? Usuario { get; set; }
}
