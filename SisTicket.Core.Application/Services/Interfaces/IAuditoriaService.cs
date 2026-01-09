using SisTicket.Core.Application.DTOs.Auditoria;
using SisTicket.Core.Domain.Enums;

namespace SisTicket.Core.Application.Services.Interfaces;

public interface IAuditoriaService
{
    Task RegistrarAsync(
        int? usuarioId,
        string? nombreUsuario,
        string? rol,
        TipoAccion tipoAccion,
        string entidad,
        int? entidadId,
        string accion,
        string? descripcion = null,
        object? valoresAntiguos = null,
        object? valoresNuevos = null,
        string? ipAddress = null,
        string? userAgent = null,
        bool exitoso = true,
        string? mensajeError = null
    );
    
    Task<IEnumerable<AuditoriaLogResponse>> GetLogsAsync(
        int? usuarioId = null,
        TipoAccion? tipoAccion = null,
        string? entidad = null,
        DateTime? fechaDesde = null,
        DateTime? fechaHasta = null,
        int pageNumber = 1,
        int pageSize = 50
    );
    
    Task<int> GetTotalLogsAsync(
        int? usuarioId = null,
        TipoAccion? tipoAccion = null,
        string? entidad = null,
        DateTime? fechaDesde = null,
        DateTime? fechaHasta = null
    );
}
