namespace SisTicket.Core.Application.DTOs.Comentario;

public class ComentarioRequest
{
    public string Texto { get; set; } = string.Empty;
    public int SolicitudId { get; set; }
}
