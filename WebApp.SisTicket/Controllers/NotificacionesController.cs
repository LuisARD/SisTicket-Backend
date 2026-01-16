using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SisTicket.Core.Application.DTOs.Notificacion;
using SisTicket.Core.Application.Services.Interfaces;
using System.Security.Claims;

namespace WebApp.SisTicket.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotificacionesController(INotificacionService notificacionService) : ControllerBase
{
    /// <summary>
    /// Obtiene las notificaciones de las últimas 24h del usuario autenticado
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<NotificacionDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<NotificacionDto>>> GetMisNotificaciones()
    {
        var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var notificaciones = await notificacionService.ObtenerNotificacionesUsuarioAsync(usuarioId);
        
        return Ok(notificaciones);
    }
}
