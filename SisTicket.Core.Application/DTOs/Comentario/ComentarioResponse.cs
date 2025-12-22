namespace SisTicket.Core.Application.DTOs.Comentario;

public class ComentarioResponse
{
    public int Id { get; set; }
    public string Texto { get; set; } = string.Empty;
    public int SolicitudId { get; set; }
    public int UsuarioId { get; set; }
    public string UsuarioNombre { get; set; } = string.Empty;
    public DateTime FechaCreacion { get; set; }
}
