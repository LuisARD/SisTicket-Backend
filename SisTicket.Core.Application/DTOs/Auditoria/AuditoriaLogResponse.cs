namespace SisTicket.Core.Application.DTOs.Auditoria;

public class AuditoriaLogResponse
{
    public int Id { get; set; }
    public int? UsuarioId { get; set; }
    public string? NombreUsuario { get; set; }
    public string? Rol { get; set; }
    public string TipoAccion { get; set; } = string.Empty;
    public string Entidad { get; set; } = string.Empty;
    public int? EntidadId { get; set; }
    public string Accion { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public string? ValoresAntiguos { get; set; }
    public string? ValoresNuevos { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public bool Exitoso { get; set; }
    public string? MensajeError { get; set; }
    public DateTime FechaHoraUtc { get; set; }
}
