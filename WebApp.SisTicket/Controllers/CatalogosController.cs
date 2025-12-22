using Microsoft.AspNetCore.Mvc;
using SisTicket.Core.Application.DTOs.Area;
using SisTicket.Core.Application.DTOs.Prioridad;
using SisTicket.Core.Application.DTOs.TipoSolicitud;
using SisTicket.Core.Application.Services.Interfaces;

namespace WebApp.SisTicket.Controllers;

/// <summary>
/// Gestión de Catálogos del Sistema (Áreas, Prioridades, Tipos de Solicitud)
/// </summary>
public class CatalogosController : BaseApiController
{
    private readonly IAreaService _areaService;
    private readonly IPrioridadService _prioridadService;
    private readonly ITipoSolicitudService _tipoSolicitudService;

    public CatalogosController(
        IAreaService areaService,
        IPrioridadService prioridadService,
        ITipoSolicitudService tipoSolicitudService)
    {
        _areaService = areaService;
        _prioridadService = prioridadService;
        _tipoSolicitudService = tipoSolicitudService;
    }

    #region Áreas

    /// <summary>
    /// Obtiene todas las áreas
    /// </summary>
    [HttpGet("areas")]
    [ProducesResponseType(typeof(IEnumerable<AreaResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllAreas()
    {
        var areas = await _areaService.GetAllAsync();
        return Ok(areas);
    }

    /// <summary>
    /// Obtiene un área por ID
    /// </summary>
    [HttpGet("areas/{id}")]
    [ProducesResponseType(typeof(AreaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAreaById(int id)
    {
        var area = await _areaService.GetByIdAsync(id);
        return Ok(area);
    }

    /// <summary>
    /// Crea una nueva área (Solo Admin/SuperAdmin)
    /// </summary>
    [HttpPost("areas")]
    [ProducesResponseType(typeof(AreaResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateArea([FromBody] AreaRequest request)
    {
        var area = await _areaService.CreateAsync(request);
        return CreatedAtAction(nameof(GetAreaById), new { id = area.Id }, area);
    }

    /// <summary>
    /// Actualiza un área (Solo Admin/SuperAdmin)
    /// </summary>
    [HttpPut("areas/{id}")]
    [ProducesResponseType(typeof(AreaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateArea(int id, [FromBody] AreaRequest request)
    {
        var area = await _areaService.UpdateAsync(id, request);
        return Ok(area);
    }

    /// <summary>
    /// Elimina un área (Solo Admin/SuperAdmin)
    /// </summary>
    [HttpDelete("areas/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteArea(int id)
    {
        await _areaService.DeleteAsync(id);
        return NoContent();
    }

    #endregion

    #region Prioridades

    /// <summary>
    /// Obtiene todas las prioridades ordenadas por nivel
    /// </summary>
    [HttpGet("prioridades")]
    [ProducesResponseType(typeof(IEnumerable<PrioridadResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllPrioridades()
    {
        var prioridades = await _prioridadService.GetAllAsync();
        return Ok(prioridades);
    }

    /// <summary>
    /// Obtiene una prioridad por ID
    /// </summary>
    [HttpGet("prioridades/{id}")]
    [ProducesResponseType(typeof(PrioridadResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPrioridadById(int id)
    {
        var prioridad = await _prioridadService.GetByIdAsync(id);
        return Ok(prioridad);
    }

    /// <summary>
    /// Crea una nueva prioridad (Solo Admin/SuperAdmin)
    /// </summary>
    [HttpPost("prioridades")]
    [ProducesResponseType(typeof(PrioridadResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreatePrioridad([FromBody] PrioridadRequest request)
    {
        var prioridad = await _prioridadService.CreateAsync(request);
        return CreatedAtAction(nameof(GetPrioridadById), new { id = prioridad.Id }, prioridad);
    }

    /// <summary>
    /// Actualiza una prioridad (Solo Admin/SuperAdmin)
    /// </summary>
    [HttpPut("prioridades/{id}")]
    [ProducesResponseType(typeof(PrioridadResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdatePrioridad(int id, [FromBody] PrioridadRequest request)
    {
        var prioridad = await _prioridadService.UpdateAsync(id, request);
        return Ok(prioridad);
    }

    /// <summary>
    /// Elimina una prioridad (Solo Admin/SuperAdmin)
    /// </summary>
    [HttpDelete("prioridades/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeletePrioridad(int id)
    {
        await _prioridadService.DeleteAsync(id);
        return NoContent();
    }

    #endregion

    #region Tipos de Solicitud

    /// <summary>
    /// Obtiene todos los tipos de solicitud
    /// </summary>
    [HttpGet("tipos-solicitud")]
    [ProducesResponseType(typeof(IEnumerable<TipoSolicitudResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllTiposSolicitud()
    {
        var tipos = await _tipoSolicitudService.GetAllAsync();
        return Ok(tipos);
    }

    /// <summary>
    /// Obtiene un tipo de solicitud por ID
    /// </summary>
    [HttpGet("tipos-solicitud/{id}")]
    [ProducesResponseType(typeof(TipoSolicitudResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTipoSolicitudById(int id)
    {
        var tipo = await _tipoSolicitudService.GetByIdAsync(id);
        return Ok(tipo);
    }

    /// <summary>
    /// Crea un nuevo tipo de solicitud (Solo Admin/SuperAdmin)
    /// </summary>
    [HttpPost("tipos-solicitud")]
    [ProducesResponseType(typeof(TipoSolicitudResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateTipoSolicitud([FromBody] TipoSolicitudRequest request)
    {
        var tipo = await _tipoSolicitudService.CreateAsync(request);
        return CreatedAtAction(nameof(GetTipoSolicitudById), new { id = tipo.Id }, tipo);
    }

    /// <summary>
    /// Actualiza un tipo de solicitud (Solo Admin/SuperAdmin)
    /// </summary>
    [HttpPut("tipos-solicitud/{id}")]
    [ProducesResponseType(typeof(TipoSolicitudResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateTipoSolicitud(int id, [FromBody] TipoSolicitudRequest request)
    {
        var tipo = await _tipoSolicitudService.UpdateAsync(id, request);
        return Ok(tipo);
    }

    /// <summary>
    /// Elimina un tipo de solicitud (Solo Admin/SuperAdmin)
    /// </summary>
    [HttpDelete("tipos-solicitud/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteTipoSolicitud(int id)
    {
        await _tipoSolicitudService.DeleteAsync(id);
        return NoContent();
    }

    #endregion
}
