using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SisTicket.Core.Application.DTOs.Adjunto;
using SisTicket.Core.Application.DTOs.Comentario;
using SisTicket.Core.Application.DTOs.Solicitud;
using SisTicket.Core.Application.Services.Interfaces;
using SisTicket.Core.Domain.Enums;

namespace WebApp.SisTicket.Controllers;

/// <summary>
/// Gestión de Solicitudes y Comentarios
/// </summary>
[Authorize]
public class SolicitudesController : BaseApiController
{
    private readonly ISolicitudService _solicitudService;
    private readonly IComentarioService _comentarioService;
    private readonly IAdjuntoService _adjuntoService;

    public SolicitudesController(
        ISolicitudService solicitudService,
        IComentarioService comentarioService,
        IAdjuntoService adjuntoService)
    {
        _solicitudService = solicitudService;
        _comentarioService = comentarioService;
        _adjuntoService = adjuntoService;
    }

    #region Solicitudes

    /// <summary>
    /// Obtiene todas las solicitudes
    /// SuperAdmin/Admin: Todas
    /// Gestor: Asignadas a él + Sin asignar de su área
    /// Solicitante: Solo las propias
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<SolicitudResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var usuarioActualId = GetCurrentUserId();
        var rol = GetCurrentUserRole();

        // SuperAdmin y Admin ven todas
        if (rol == "SuperAdmin" || rol == "Admin")
        {
            var solicitudes = await _solicitudService.GetAllAsync();
            return Ok(solicitudes);
        }

        // Gestor ve las asignadas a él + sin asignar de su área
        if (rol == "Gestor")
        {
            var solicitudes = await _solicitudService.GetSolicitudesGestorAreaAsync(usuarioActualId);
            return Ok(solicitudes);
        }

        // Solicitante ve solo las propias
        var solicitudesPropias = await _solicitudService.GetBySolicitanteIdAsync(usuarioActualId);
        return Ok(solicitudesPropias);
    }

    /// <summary>
    /// Obtiene una solicitud por ID con validación de permisos
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(SolicitudResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetById(int id)
    {
        var solicitud = await _solicitudService.GetByIdAsync(id);
        
        // Validar permisos: SuperAdmin/Admin ven todas
        var rol = GetCurrentUserRole();
        if (rol == "SuperAdmin" || rol == "Admin")
        {
            return Ok(solicitud);
        }

        var usuarioActualId = GetCurrentUserId();

        // Gestor solo ve las de su área
        if (rol == "Gestor")
        {
            // TODO: Validar que la solicitud es del área del gestor
            return Ok(solicitud);
        }

        // Solicitante solo ve las propias
        if (solicitud.SolicitanteId != usuarioActualId)
        {
            return Forbid();
        }

        return Ok(solicitud);
    }

    /// <summary>
    /// Obtiene solicitudes por solicitante
    /// </summary>
    [HttpGet("solicitante/{solicitanteId}")]
    [Authorize(Roles = "Admin,SuperAdmin")]
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
    [Authorize(Roles = "Gestor,Admin,SuperAdmin")]
    [ProducesResponseType(typeof(IEnumerable<SolicitudResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByGestor(int gestorId)
    {
        var solicitudes = await _solicitudService.GetByGestorIdAsync(gestorId);
        return Ok(solicitudes);
    }

    /// <summary>
    /// Obtiene solicitudes con filtros (Solo Admin/SuperAdmin)
    /// </summary>
    [HttpGet("filtrar")]
    [Authorize(Roles = "Admin,SuperAdmin")]
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
    /// Crea una nueva solicitud (Todos los usuarios autenticados)
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
    /// Actualiza una solicitud existente
    /// Solo el solicitante puede editar si está en estado Nueva y sin gestor
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
    [Authorize(Roles = "Admin,SuperAdmin")]
    [ProducesResponseType(typeof(SolicitudResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> AsignarGestor(int id, [FromBody] AsignarGestorRequest request)
    {
        var usuarioActualId = GetCurrentUserId();
        var solicitud = await _solicitudService.AsignarGestorAsync(id, request.GestorId, usuarioActualId);
        return Ok(solicitud);
    }

    /// <summary>
    /// Permite a un gestor auto-asignarse una solicitud de su área (Solo Gestores)
    /// La solicitud debe estar en estado Nueva y sin gestor asignado
    /// El gestor debe pertenecer al área de la solicitud
    /// </summary>
    [HttpPost("{id}/tomar-solicitud")]
    [Authorize(Roles = "Gestor")]
    [ProducesResponseType(typeof(SolicitudResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> TomarSolicitud(int id)
    {
        var usuarioActualId = GetCurrentUserId();
        var solicitud = await _solicitudService.TomarSolicitudAsync(id, usuarioActualId);
        return Ok(solicitud);
    }

    /// <summary>
    /// Cambia el estado de una solicitud
    /// Admin/SuperAdmin: Cualquier solicitud
    /// Gestor: Solo las asignadas a él
    /// </summary>
    [HttpPost("{id}/cambiar-estado")]
    [Authorize(Roles = "Gestor,Admin,SuperAdmin")]
    [ProducesResponseType(typeof(SolicitudResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CambiarEstado(int id, [FromBody] CambiarEstadoSolicitudRequest request)
    {
        var usuarioActualId = GetCurrentUserId();
        var estado = (EstadoSolicitud)request.Estado;
        var solicitud = await _solicitudService.CambiarEstadoAsync(id, estado, usuarioActualId);
        return Ok(solicitud);
    }

    /// <summary>
    /// Elimina una solicitud (Solo Admin/SuperAdmin)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Delete(int id)
    {
        await _solicitudService.DeleteAsync(id);
        return NoContent();
    }

    #endregion

    #region Comentarios

    /// <summary>
    /// Obtiene todos los comentarios de una solicitud
    /// Todos los usuarios autenticados pueden ver comentarios
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
    /// SOLO Gestores del área, Admin y SuperAdmin pueden comentar
    /// Solicitantes solo pueden VER comentarios, NO crearlos
    /// </summary>
    [HttpPost("{solicitudId}/comentarios")]
    [ProducesResponseType(typeof(ComentarioResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
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
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeleteComentario(int solicitudId, int comentarioId)
    {
        var usuarioActualId = GetCurrentUserId();
        await _comentarioService.DeleteAsync(comentarioId, usuarioActualId);
        return NoContent();
    }

    #endregion

    #region Adjuntos

    /// <summary>
    /// Obtiene todos los adjuntos de una solicitud
    /// </summary>
    [HttpGet("{solicitudId}/adjuntos")]
    [ProducesResponseType(typeof(IEnumerable<AdjuntoResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetAdjuntos(int solicitudId)
    {
        var usuarioActualId = GetCurrentUserId();
        var rol = GetCurrentUserRole();

        // Validar permisos de acceso
        if (!await _adjuntoService.UsuarioPuedeAccederAsync(solicitudId, usuarioActualId, rol))
        {
            return Forbid();
        }

        var adjuntos = await _adjuntoService.GetBySolicitudIdAsync(solicitudId);
        return Ok(adjuntos);
    }

    /// <summary>
    /// Sube un archivo adjunto a una solicitud
    /// Máximo 5 archivos por solicitud, 10MB por archivo
    /// Formatos permitidos: PDF, PNG, JPG, JPEG, GIF, DOC, DOCX, XLS, XLSX, TXT, ZIP, RAR
    /// </summary>
    [HttpPost("{solicitudId}/adjuntos")]
    [RequestSizeLimit(10_485_760)] // 10MB
    [ProducesResponseType(typeof(AdjuntoResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> SubirAdjunto(int solicitudId, IFormFile archivo)
    {
        var usuarioActualId = GetCurrentUserId();
        var rol = GetCurrentUserRole();

        // Validar permisos de acceso
        if (!await _adjuntoService.UsuarioPuedeAccederAsync(solicitudId, usuarioActualId, rol))
        {
            return Forbid();
        }

        var adjunto = await _adjuntoService.SubirArchivoAsync(solicitudId, archivo, usuarioActualId);
        return CreatedAtAction(nameof(GetAdjuntos), new { solicitudId }, adjunto);
    }

    /// <summary>
    /// Descarga el archivo adjunto de una solicitud
    /// Si la solicitud no tiene adjuntos, devuelve un mensaje indicándolo
    /// </summary>
    [HttpGet("{solicitudId}/adjuntos/descargar")]
    [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DescargarAdjunto(int solicitudId)
    {
        var usuarioActualId = GetCurrentUserId();
        var rol = GetCurrentUserRole();

        // Validar permisos de acceso
        if (!await _adjuntoService.UsuarioPuedeAccederAsync(solicitudId, usuarioActualId, rol))
        {
            return Forbid();
        }

        var (stream, nombreArchivo, tipoContenido) = await _adjuntoService.DescargarArchivoAsync(
            solicitudId, 
            usuarioActualId
        );

        return File(stream, tipoContenido, nombreArchivo);
    }

    /// <summary>
    /// Elimina un archivo adjunto
    /// Solo puede eliminar: el usuario que lo subió, el gestor asignado, Admin o SuperAdmin
    /// </summary>
    [HttpDelete("{solicitudId}/adjuntos/{adjuntoId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> EliminarAdjunto(int solicitudId, int adjuntoId)
    {
        var usuarioActualId = GetCurrentUserId();
        
        await _adjuntoService.EliminarAsync(adjuntoId, solicitudId, usuarioActualId);
        return NoContent();
    }

    #endregion
}
