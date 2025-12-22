using Microsoft.AspNetCore.Mvc;
using SisTicket.Core.Application.DTOs.TipoSolicitud;
using SisTicket.Core.Application.Services.Interfaces;

namespace WebApp.SisTicket.Controllers;

/// <summary>
/// Gestión de Tipos de Solicitud
/// </summary>
public class TiposSolicitudController : BaseApiController
{
    private readonly ITipoSolicitudService _tipoSolicitudService;

    public TiposSolicitudController(ITipoSolicitudService tipoSolicitudService)
    {
        _tipoSolicitudService = tipoSolicitudService;
    }

    /// <summary>
    /// Obtiene todos los tipos de solicitud
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<TipoSolicitudResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var tipos = await _tipoSolicitudService.GetAllAsync();
        return Ok(tipos);
    }

    /// <summary>
    /// Obtiene un tipo de solicitud por ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(TipoSolicitudResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var tipo = await _tipoSolicitudService.GetByIdAsync(id);
        return Ok(tipo);
    }

    /// <summary>
    /// Crea un nuevo tipo de solicitud (Solo Admin/SuperAdmin)
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(TipoSolicitudResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] TipoSolicitudRequest request)
    {
        var tipo = await _tipoSolicitudService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = tipo.Id }, tipo);
    }

    /// <summary>
    /// Actualiza un tipo de solicitud existente (Solo Admin/SuperAdmin)
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(TipoSolicitudResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(int id, [FromBody] TipoSolicitudRequest request)
    {
        var tipo = await _tipoSolicitudService.UpdateAsync(id, request);
        return Ok(tipo);
    }

    /// <summary>
    /// Elimina un tipo de solicitud (Soft delete) (Solo Admin/SuperAdmin)
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        await _tipoSolicitudService.DeleteAsync(id);
        return NoContent();
    }
}
