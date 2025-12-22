namespace SisTicket.Core.Application.DTOs.Prioridad;

public class PrioridadRequest
{
    public string Nombre { get; set; } = string.Empty;
    public int Nivel { get; set; }
    public string? Descripcion { get; set; }
}
