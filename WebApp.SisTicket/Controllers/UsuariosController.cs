using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SisTicket.Core.Application.DTOs.Usuario;
using SisTicket.Core.Application.Services.Interfaces;

namespace WebApp.SisTicket.Controllers;

/// <summary>
/// Gestión de Usuarios
/// Lectura: Todos los roles autenticados
/// Escritura: Solo SuperAdmin
/// </summary>
[Authorize]
public class UsuariosController : BaseApiController
{
    private readonly IUsuarioService _usuarioService;

    public UsuariosController(IUsuarioService usuarioService)
    {
        _usuarioService = usuarioService;
    }

    /// <summary>
    /// Obtiene todos los usuarios
    /// Accesible por: Todos los roles autenticados
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
    /// Accesible por: Todos los roles autenticados
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
    /// Accesible por: Todos los roles autenticados
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
    /// Accesible por: Todos los roles autenticados
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
    [Authorize(Roles = "SuperAdmin")]
    [ProducesResponseType(typeof(UsuarioResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
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
    [Authorize(Roles = "SuperAdmin")]
    [ProducesResponseType(typeof(UsuarioResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Update(int id, [FromBody] UsuarioRequest request)
    {
        var usuarioActualId = GetCurrentUserId();
        var usuario = await _usuarioService.UpdateAsync(id, request, usuarioActualId);
        return Ok(usuario);
    }

    /// <summary>
    /// Elimina un usuario (Solo SuperAdmin)
    /// El usuario debe estar inactivo para poder eliminarlo
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Delete(int id)
    {
        var usuarioActualId = GetCurrentUserId();
        await _usuarioService.DeleteAsync(id, usuarioActualId);
        return NoContent();
    }

    /// <summary>
    /// Activa o desactiva un usuario (Solo SuperAdmin)
    /// Los usuarios inactivos no pueden iniciar sesión ni ser asignados
    /// Para eliminar un usuario primero debe estar inactivo
    /// </summary>
    [HttpPatch("{id}/estado")]
    [Authorize(Roles = "SuperAdmin")]
    [ProducesResponseType(typeof(UsuarioResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CambiarEstado(int id, [FromBody] CambiarEstadoUsuarioRequest request)
    {
        var usuarioActualId = GetCurrentUserId();
        var usuario = await _usuarioService.CambiarEstadoAsync(id, request.Activo, usuarioActualId);
        return Ok(usuario);
    }

    /// <summary>
    /// Permite a CUALQUIER usuario autenticado cambiar su propia contraseña
    /// Requisitos de seguridad:
    /// - Mínimo 8 caracteres
    /// - Al menos 1 letra mayúscula
    /// - Al menos 1 número
    /// - Al menos 1 símbolo (-, *, @, !, #, $, %, etc.)
    /// </summary>
    [HttpPost("cambiar-mi-password")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CambiarMiPassword([FromBody] CambiarPasswordRequest request)
    {
        var usuarioActualId = GetCurrentUserId();
        await _usuarioService.CambiarMiPasswordAsync(usuarioActualId, request);
        
        return Ok(new 
        { 
            message = "Contraseña cambiada exitosamente",
            success = true
        });
    }
}
