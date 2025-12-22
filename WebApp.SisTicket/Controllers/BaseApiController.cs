using Microsoft.AspNetCore.Mvc;

namespace WebApp.SisTicket.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public abstract class BaseApiController : ControllerBase
{
    // Método helper para obtener el ID del usuario actual
    // Se implementará completamente en la parte 2 (JWT)
    protected int GetCurrentUserId()
    {
        // TODO: Obtener del JWT en la parte 2
        // Por ahora retorna un ID temporal para pruebas
        return 1; // ID del SuperAdmin de los seeds
    }
}
