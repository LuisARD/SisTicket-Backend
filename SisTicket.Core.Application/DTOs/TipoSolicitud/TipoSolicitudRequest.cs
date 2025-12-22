namespace SisTicket.Core.Application.DTOs.TipoSolicitud;

public class TipoSolicitudRequest
{
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
}
