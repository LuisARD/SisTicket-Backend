namespace SisTicket.Core.Application.DTOs.Prioridad;

public class PrioridadResponse
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public int Nivel { get; set; }
    public string? Descripcion { get; set; }
    public DateTime FechaCreacion { get; set; }
    public bool Activo { get; set; }
}
