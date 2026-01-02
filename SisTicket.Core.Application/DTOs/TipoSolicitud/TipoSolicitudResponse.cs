namespace SisTicket.Core.Application.DTOs.TipoSolicitud;

public class TipoSolicitudResponse
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public int AreaId { get; set; }
    public string AreaNombre { get; set; } = string.Empty;
    public DateTime FechaCreacion { get; set; }
    public bool Activo { get; set; }
}
