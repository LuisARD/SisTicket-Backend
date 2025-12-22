namespace SisTicket.Core.Application.DTOs.Solicitud;

public class SolicitudResponse
{
    public int Id { get; set; }
    public string NumeroSolicitud { get; set; } = string.Empty;
    public string Titulo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    
    public int SolicitanteId { get; set; }
    public string SolicitanteNombre { get; set; } = string.Empty;
    
    public int? GestorAsignadoId { get; set; }
    public string? GestorAsignadoNombre { get; set; }
    
    public int TipoSolicitudId { get; set; }
    public string TipoSolicitudNombre { get; set; } = string.Empty;
    
    public int PrioridadId { get; set; }
    public string PrioridadNombre { get; set; } = string.Empty;
    
    public int AreaId { get; set; }
    public string AreaNombre { get; set; } = string.Empty;
    
    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaModificacion { get; set; }
    public bool Activo { get; set; }
}
