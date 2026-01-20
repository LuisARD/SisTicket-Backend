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
    /// Admin/SuperAdmin obtienen TODAS las notificaciones del sistema
    /// Acceso: Solo Gestor, Admin y SuperAdmin
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Gestor,Admin,SuperAdmin")]
    [ProducesResponseType(typeof(IEnumerable<NotificacionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IEnumerable<NotificacionDto>>> GetMisNotificaciones()
    {
        var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var notificaciones = await notificacionService.ObtenerNotificacionesUsuarioAsync(usuarioId);
        
        return Ok(notificaciones);
    }

    /// <summary>
    /// Elimina una notificación específica (Solo Admin/SuperAdmin)
    /// </summary>
    [HttpDelete("{notificacionId:guid}")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult> EliminarNotificacion(Guid notificacionId)
    {
        var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var eliminada = await notificacionService.EliminarNotificacionAsync(notificacionId, usuarioId);

        if (!eliminada)
        {
            return NotFound(new { mensaje = "Notificación no encontrada" });
        }

        return NoContent();
    }

    /// <summary>
    /// Limpia todas las notificaciones de un usuario (Solo Admin/SuperAdmin)
    /// </summary>
    [HttpDelete("usuario/{usuarioId:int}")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult> LimpiarNotificacionesUsuario(int usuarioId)
    {
        var adminId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        await notificacionService.LimpiarNotificacionesUsuarioAsync(usuarioId, adminId);

        return NoContent();
    }

    /// <summary>
    /// Obtiene todas las notificaciones del sistema (Solo Admin/SuperAdmin)
    /// </summary>
    [HttpGet("todas")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    [ProducesResponseType(typeof(IEnumerable<NotificacionDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<NotificacionDto>>> GetTodasLasNotificaciones()
    {
        var notificaciones = await notificacionService.ObtenerTodasLasNotificacionesAsync();
        return Ok(notificaciones);
    }
}
