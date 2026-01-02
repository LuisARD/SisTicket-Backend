using SisTicket.Core.Domain.Common;

namespace SisTicket.Core.Domain.Entities;

public class Adjunto : BaseEntity
{
    public int SolicitudId { get; set; }
    public string NombreArchivo { get; set; } = string.Empty;
    public string NombreArchivoOriginal { get; set; } = string.Empty;
    public string RutaArchivo { get; set; } = string.Empty;
    public string TipoContenido { get; set; } = string.Empty;
    public long TamanoBytes { get; set; }
    public int CargadoPorId { get; set; }
    
    public Solicitud Solicitud { get; set; } = null!;
    public Usuario CargadoPor { get; set; } = null!;
}
