namespace SisTicket.Core.Application.DTOs.Solicitud;

public class SolicitudRequest
{
    public string Titulo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public int TipoSolicitudId { get; set; }
    public int PrioridadId { get; set; }
    public int AreaId { get; set; }
}
