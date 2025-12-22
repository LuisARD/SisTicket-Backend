using Microsoft.AspNetCore.Mvc;
using SisTicket.Core.Application.DTOs.Area;
using SisTicket.Core.Application.Services.Interfaces;

namespace WebApp.SisTicket.Controllers;

/// <summary>
/// Gestión de Áreas
/// </summary>
public class AreasController : BaseApiController
{
    private readonly IAreaService _areaService;

    public AreasController(IAreaService areaService)
    {
        _areaService = areaService;
    }

    /// <summary>
    /// Obtiene todas las áreas activas
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<AreaResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var areas = await _areaService.GetAllAsync();
        return Ok(areas);
    }

    /// <summary>
    /// Obtiene un área por ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(AreaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var area = await _areaService.GetByIdAsync(id);
        return Ok(area);
    }

    /// <summary>
    /// Crea una nueva área (Solo Admin/SuperAdmin)
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(AreaResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] AreaRequest request)
    {
        var area = await _areaService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = area.Id }, area);
    }

    /// <summary>
    /// Actualiza un área existente (Solo Admin/SuperAdmin)
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(AreaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(int id, [FromBody] AreaRequest request)
    {
        var area = await _areaService.UpdateAsync(id, request);
        return Ok(area);
    }

    /// <summary>
    /// Elimina un área (Soft delete) (Solo Admin/SuperAdmin)
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        await _areaService.DeleteAsync(id);
        return NoContent();
    }
}
