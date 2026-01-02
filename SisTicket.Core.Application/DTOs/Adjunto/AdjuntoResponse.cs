namespace SisTicket.Core.Application.DTOs.Adjunto;

public class AdjuntoResponse
{
    public int Id { get; set; }
    public int SolicitudId { get; set; }
    public string NombreArchivo { get; set; } = string.Empty;
    public string NombreArchivoOriginal { get; set; } = string.Empty;
    public string TipoContenido { get; set; } = string.Empty;
    public long TamanoBytes { get; set; }
    public string TamanoLegible { get; set; } = string.Empty;
    public int CargadoPorId { get; set; }
    public string CargadoPorNombre { get; set; } = string.Empty;
    public DateTime FechaCreacion { get; set; }
}
