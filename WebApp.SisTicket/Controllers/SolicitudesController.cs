using Microsoft.AspNetCore.Mvc;
using SisTicket.Core.Application.DTOs.Comentario;
using SisTicket.Core.Application.DTOs.Solicitud;
using SisTicket.Core.Application.Services.Interfaces;
using SisTicket.Core.Domain.Enums;

namespace WebApp.SisTicket.Controllers;

/// <summary>
/// Gestión de Solicitudes y Comentarios
/// </summary>
public class SolicitudesController : BaseApiController
{
    private readonly ISolicitudService _solicitudService;
    private readonly IComentarioService _comentarioService;

    public SolicitudesController(
        ISolicitudService solicitudService,
        IComentarioService comentarioService)
    {
        _solicitudService = solicitudService;
        _comentarioService = comentarioService;
    }

    #region Solicitudes

    /// <summary>
    /// Obtiene todas las solicitudes
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<SolicitudResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var solicitudes = await _solicitudService.GetAllAsync();
        return Ok(solicitudes);
    }

    /// <summary>
    /// Obtiene una solicitud por ID con todos sus detalles
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(SolicitudResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var solicitud = await _solicitudService.GetByIdAsync(id);
        return Ok(solicitud);
    }

    /// <summary>
    /// Obtiene solicitudes por solicitante
    /// </summary>
    [HttpGet("solicitante/{solicitanteId}")]
    [ProducesResponseType(typeof(IEnumerable<SolicitudResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetBySolicitante(int solicitanteId)
    {
        var solicitudes = await _solicitudService.GetBySolicitanteIdAsync(solicitanteId);
        return Ok(solicitudes);
    }

    /// <summary>
    /// Obtiene solicitudes por gestor asignado
    /// </summary>
    [HttpGet("gestor/{gestorId}")]
    [ProducesResponseType(typeof(IEnumerable<SolicitudResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByGestor(int gestorId)
    {
        var solicitudes = await _solicitudService.GetByGestorIdAsync(gestorId);
        return Ok(solicitudes);
    }

    /// <summary>
    /// Obtiene solicitudes con filtros
    /// </summary>
    [HttpGet("filtrar")]
    [ProducesResponseType(typeof(IEnumerable<SolicitudResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByFiltros(
        [FromQuery] EstadoSolicitud? estado = null,
        [FromQuery] int? prioridadId = null,
        [FromQuery] DateTime? fechaDesde = null,
        [FromQuery] DateTime? fechaHasta = null)
    {
        var solicitudes = await _solicitudService.GetByFiltrosAsync(estado, prioridadId, fechaDesde, fechaHasta);
        return Ok(solicitudes);
    }

    /// <summary>
    /// Crea una nueva solicitud
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(SolicitudResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] SolicitudRequest request)
    {
        var usuarioActualId = GetCurrentUserId();
        var solicitud = await _solicitudService.CreateAsync(request, usuarioActualId);
        return CreatedAtAction(nameof(GetById), new { id = solicitud.Id }, solicitud);
    }

    /// <summary>
    /// Actualiza una solicitud existente (Solo si está en estado Nueva y sin gestor)
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(SolicitudResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Update(int id, [FromBody] SolicitudRequest request)
    {
        var usuarioActualId = GetCurrentUserId();
        var solicitud = await _solicitudService.UpdateAsync(id, request, usuarioActualId);
        return Ok(solicitud);
    }

    /// <summary>
    /// Asigna un gestor a una solicitud (Solo Admin/SuperAdmin)
    /// </summary>
    [HttpPost("{id}/asignar-gestor")]
    [ProducesResponseType(typeof(SolicitudResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> AsignarGestor(int id, [FromBody] AsignarGestorRequest request)
    {
        var usuarioActualId = GetCurrentUserId();
        var solicitud = await _solicitudService.AsignarGestorAsync(id, request.GestorId, usuarioActualId);
        return Ok(solicitud);
    }

    /// <summary>
    /// Cambia el estado de una solicitud
    /// </summary>
    [HttpPost("{id}/cambiar-estado")]
    [ProducesResponseType(typeof(SolicitudResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CambiarEstado(int id, [FromBody] CambiarEstadoSolicitudRequest request)
    {
        var usuarioActualId = GetCurrentUserId();
        var estado = (EstadoSolicitud)request.Estado;
        var solicitud = await _solicitudService.CambiarEstadoAsync(id, estado, usuarioActualId);
        return Ok(solicitud);
    }

    /// <summary>
    /// Elimina una solicitud (Soft delete)
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        await _solicitudService.DeleteAsync(id);
        return NoContent();
    }

    #endregion

    #region Comentarios

    /// <summary>
    /// Obtiene todos los comentarios de una solicitud
    /// </summary>
    [HttpGet("{solicitudId}/comentarios")]
    [ProducesResponseType(typeof(IEnumerable<ComentarioResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetComentarios(int solicitudId)
    {
        var comentarios = await _comentarioService.GetBySolicitudIdAsync(solicitudId);
        return Ok(comentarios);
    }

    /// <summary>
    /// Crea un nuevo comentario en una solicitud
    /// Pueden comentar: Solicitante, Gestores del área, Admin, SuperAdmin
    /// </summary>
    [HttpPost("{solicitudId}/comentarios")]
    [ProducesResponseType(typeof(ComentarioResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateComentario(int solicitudId, [FromBody] ComentarioRequest request)
    {
        request.SolicitudId = solicitudId;
        var usuarioActualId = GetCurrentUserId();
        var comentario = await _comentarioService.CreateAsync(request, usuarioActualId);
        return CreatedAtAction(nameof(GetComentarios), new { solicitudId }, comentario);
    }

    /// <summary>
    /// Elimina un comentario
    /// Pueden eliminar: Autor del comentario, Admin, SuperAdmin
    /// </summary>
    [HttpDelete("{solicitudId}/comentarios/{comentarioId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DeleteComentario(int solicitudId, int comentarioId)
    {
        var usuarioActualId = GetCurrentUserId();
        await _comentarioService.DeleteAsync(comentarioId, usuarioActualId);
        return NoContent();
    }

    #endregion
}
