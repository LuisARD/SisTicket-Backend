using Microsoft.AspNetCore.Mvc;
using SisTicket.Core.Application.DTOs.Usuario;
using SisTicket.Core.Application.Services.Interfaces;

namespace WebApp.SisTicket.Controllers;

/// <summary>
/// Gestión de Usuarios (Solo SuperAdmin)
/// </summary>
public class UsuariosController : BaseApiController
{
    private readonly IUsuarioService _usuarioService;

    public UsuariosController(IUsuarioService usuarioService)
    {
        _usuarioService = usuarioService;
    }

    /// <summary>
    /// Obtiene todos los usuarios
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<UsuarioResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var usuarios = await _usuarioService.GetAllAsync();
        return Ok(usuarios);
    }

    /// <summary>
    /// Obtiene un usuario por ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(UsuarioResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var usuario = await _usuarioService.GetByIdAsync(id);
        return Ok(usuario);
    }

    /// <summary>
    /// Obtiene usuarios por área
    /// </summary>
    [HttpGet("area/{areaId}")]
    [ProducesResponseType(typeof(IEnumerable<UsuarioResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByArea(int areaId)
    {
        var usuarios = await _usuarioService.GetByAreaIdAsync(areaId);
        return Ok(usuarios);
    }

    /// <summary>
    /// Obtiene gestores por área
    /// </summary>
    [HttpGet("gestores/area/{areaId}")]
    [ProducesResponseType(typeof(IEnumerable<UsuarioResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetGestoresByArea(int areaId)
    {
        var gestores = await _usuarioService.GetGestoresByAreaIdAsync(areaId);
        return Ok(gestores);
    }

    /// <summary>
    /// Crea un nuevo usuario (Solo SuperAdmin)
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(UsuarioResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Create([FromBody] UsuarioRequest request)
    {
        var usuarioActualId = GetCurrentUserId();
        var usuario = await _usuarioService.CreateAsync(request, usuarioActualId);
        return CreatedAtAction(nameof(GetById), new { id = usuario.Id }, usuario);
    }

    /// <summary>
    /// Actualiza un usuario existente (Solo SuperAdmin)
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(UsuarioResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Update(int id, [FromBody] UsuarioRequest request)
    {
        var usuarioActualId = GetCurrentUserId();
        var usuario = await _usuarioService.UpdateAsync(id, request, usuarioActualId);
        return Ok(usuario);
    }

    /// <summary>
    /// Elimina un usuario (Solo SuperAdmin)
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Delete(int id)
    {
        var usuarioActualId = GetCurrentUserId();
        await _usuarioService.DeleteAsync(id, usuarioActualId);
        return NoContent();
    }
}
