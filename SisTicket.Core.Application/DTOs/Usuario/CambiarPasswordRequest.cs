namespace SisTicket.Core.Application.DTOs.Usuario;

public class CambiarPasswordRequest
{
    public string PasswordActual { get; set; } = string.Empty;
    public string PasswordNueva { get; set; } = string.Empty;
    public string ConfirmarPassword { get; set; } = string.Empty;
}
