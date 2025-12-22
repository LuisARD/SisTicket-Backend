using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace WebApp.SisTicket.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public abstract class BaseApiController : ControllerBase
{
    // Método para obtener el ID del usuario actual desde el JWT
    protected int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        
        if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
        {
            return userId;
        }

        // Si no hay token JWT válido, retornar 0 (causará UnauthorizedException en los servicios)
        return 0;
    }

    // Método para obtener el rol del usuario actual
    protected string GetCurrentUserRole()
    {
        return User.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;
    }

    // Método para verificar si el usuario está autenticado
    protected bool IsAuthenticated()
    {
        return User.Identity?.IsAuthenticated ?? false;
    }
}
