using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SisTicket.Core.Application.DTOs.Auditoria;
using SisTicket.Core.Application.Services.Interfaces;
using SisTicket.Core.Domain.Enums;

namespace WebApp.SisTicket.Controllers;

/// <summary>
/// Gestión de Logs de Auditoría (Solo SuperAdmin)
/// </summary>
[Authorize(Roles = "SuperAdmin")]
public class AuditoriaController : BaseApiController
{
    private readonly IAuditoriaService _auditoriaService;

    public AuditoriaController(IAuditoriaService auditoriaService)
    {
        _auditoriaService = auditoriaService;
    }

    /// <summary>
    /// Obtiene logs de auditoría con filtros opcionales
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<AuditoriaLogResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetLogs(
        [FromQuery] int? usuarioId = null,
        [FromQuery] TipoAccion? tipoAccion = null,
        [FromQuery] string? entidad = null,
        [FromQuery] DateTime? fechaDesde = null,
        [FromQuery] DateTime? fechaHasta = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 50)
    {
        var logs = await _auditoriaService.GetLogsAsync(
            usuarioId, tipoAccion, entidad, fechaDesde, fechaHasta, pageNumber, pageSize);

        var total = await _auditoriaService.GetTotalLogsAsync(
            usuarioId, tipoAccion, entidad, fechaDesde, fechaHasta);

        Response.Headers.Append("X-Total-Count", total.ToString());
        Response.Headers.Append("X-Page-Number", pageNumber.ToString());
        Response.Headers.Append("X-Page-Size", pageSize.ToString());

        return Ok(logs);
    }

    /// <summary>
    /// Obtiene logs de un usuario específico
    /// </summary>
    [HttpGet("usuario/{usuarioId}")]
    [ProducesResponseType(typeof(IEnumerable<AuditoriaLogResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetLogsByUsuario(
        int usuarioId,
        [FromQuery] DateTime? fechaDesde = null,
        [FromQuery] DateTime? fechaHasta = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 50)
    {
        var logs = await _auditoriaService.GetLogsAsync(
            usuarioId, null, null, fechaDesde, fechaHasta, pageNumber, pageSize);

        return Ok(logs);
    }

    /// <summary>
    /// Obtiene logs por entidad (ej: Solicitudes, Usuarios, etc.)
    /// </summary>
    [HttpGet("entidad/{entidad}")]
    [ProducesResponseType(typeof(IEnumerable<AuditoriaLogResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetLogsByEntidad(
        string entidad,
        [FromQuery] int? entidadId = null,
        [FromQuery] DateTime? fechaDesde = null,
        [FromQuery] DateTime? fechaHasta = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 50)
    {
        var logs = await _auditoriaService.GetLogsAsync(
            null, null, entidad, fechaDesde, fechaHasta, pageNumber, pageSize);

        // Si se proporciona entidadId, filtrar en memoria (o agregar al servicio)
        if (entidadId.HasValue)
        {
            logs = logs.Where(l => l.EntidadId == entidadId.Value);
        }

        return Ok(logs);
    }

    /// <summary>
    /// Obtiene logs de acciones fallidas y accesos denegados
    /// </summary>
    [HttpGet("errores")]
    [ProducesResponseType(typeof(IEnumerable<AuditoriaLogResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetErrorLogs(
        [FromQuery] DateTime? fechaDesde = null,
        [FromQuery] DateTime? fechaHasta = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 50)
    {
        var logs = await _auditoriaService.GetLogsAsync(
            null, TipoAccion.Error, null, fechaDesde, fechaHasta, pageNumber, pageSize);

        // También incluir accesos denegados
        var accesosDenegados = await _auditoriaService.GetLogsAsync(
            null, TipoAccion.AccesoDenegado, null, fechaDesde, fechaHasta, pageNumber, pageSize);

        var todosErrores = logs.Concat(accesosDenegados)
            .OrderByDescending(l => l.FechaHora)
            .Take(pageSize);

        return Ok(todosErrores);
    }
}
