namespace SisTicket.Core.Application.DTOs.Area;

public class AreaResponse
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public DateTime FechaCreacion { get; set; }
    public bool Activo { get; set; }
}
