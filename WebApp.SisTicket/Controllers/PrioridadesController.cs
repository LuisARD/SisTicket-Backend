using Microsoft.AspNetCore.Mvc;
using SisTicket.Core.Application.DTOs.Prioridad;
using SisTicket.Core.Application.Services.Interfaces;

namespace WebApp.SisTicket.Controllers;

/// <summary>
/// Gestión de Prioridades
/// </summary>
public class PrioridadesController : BaseApiController
{
    private readonly IPrioridadService _prioridadService;

    public PrioridadesController(IPrioridadService prioridadService)
    {
        _prioridadService = prioridadService;
    }

    /// <summary>
    /// Obtiene todas las prioridades ordenadas por nivel
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<PrioridadResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var prioridades = await _prioridadService.GetAllAsync();
        return Ok(prioridades);
    }

    /// <summary>
    /// Obtiene una prioridad por ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(PrioridadResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var prioridad = await _prioridadService.GetByIdAsync(id);
        return Ok(prioridad);
    }

    /// <summary>
    /// Crea una nueva prioridad (Solo Admin/SuperAdmin)
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(PrioridadResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] PrioridadRequest request)
    {
        var prioridad = await _prioridadService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = prioridad.Id }, prioridad);
    }

    /// <summary>
    /// Actualiza una prioridad existente (Solo Admin/SuperAdmin)
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(PrioridadResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(int id, [FromBody] PrioridadRequest request)
    {
        var prioridad = await _prioridadService.UpdateAsync(id, request);
        return Ok(prioridad);
    }

    /// <summary>
    /// Elimina una prioridad (Soft delete) (Solo Admin/SuperAdmin)
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        await _prioridadService.DeleteAsync(id);
        return NoContent();
    }
}
