using Microsoft.AspNetCore.Mvc;
using SisTicket.Core.Application.DTOs.Comentario;
using SisTicket.Core.Application.Services.Interfaces;

namespace WebApp.SisTicket.Controllers;

/// <summary>
/// Gestión de Comentarios
/// </summary>
public class ComentariosController : BaseApiController
{
    private readonly IComentarioService _comentarioService;

    public ComentariosController(IComentarioService comentarioService)
    {
        _comentarioService = comentarioService;
    }

    /// <summary>
    /// Obtiene todos los comentarios de una solicitud
    /// </summary>
    [HttpGet("solicitud/{solicitudId}")]
    [ProducesResponseType(typeof(IEnumerable<ComentarioResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBySolicitud(int solicitudId)
    {
        var comentarios = await _comentarioService.GetBySolicitudIdAsync(solicitudId);
        return Ok(comentarios);
    }

    /// <summary>
    /// Crea un nuevo comentario en una solicitud
    /// Pueden comentar: Solicitante, Gestores del área, Admin, SuperAdmin
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ComentarioResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create([FromBody] ComentarioRequest request)
    {
        var usuarioActualId = GetCurrentUserId();
        var comentario = await _comentarioService.CreateAsync(request, usuarioActualId);
        return CreatedAtAction(nameof(GetBySolicitud), new { solicitudId = comentario.SolicitudId }, comentario);
    }

    /// <summary>
    /// Elimina un comentario
    /// Pueden eliminar: Autor del comentario, Admin, SuperAdmin
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Delete(int id)
    {
        var usuarioActualId = GetCurrentUserId();
        await _comentarioService.DeleteAsync(id, usuarioActualId);
        return NoContent();
    }
}
