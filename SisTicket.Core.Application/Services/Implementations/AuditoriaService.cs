using System.Text.Json;
using AutoMapper;
using SisTicket.Core.Application.DTOs.Auditoria;
using SisTicket.Core.Application.Services.Interfaces;
using SisTicket.Core.Domain.Entities;
using SisTicket.Core.Domain.Enums;
using SisTicket.Core.Domain.Interfaces;

namespace SisTicket.Core.Application.Services.Implementations;

public class AuditoriaService : IAuditoriaService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public AuditoriaService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task RegistrarAsync(
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
        bool exitoso = true,
        string? mensajeError = null)
    {
        try
        {
            var log = new AuditoriaLog
            {
                UsuarioId = usuarioId,
                NombreUsuario = nombreUsuario ?? "Anónimo",
                Rol = rol ?? "Sin rol",
                TipoAccion = tipoAccion,
                Entidad = entidad,
                EntidadId = entidadId,
                Accion = accion,
                Descripcion = descripcion,
                ValoresAntiguos = valoresAntiguos != null 
                    ? JsonSerializer.Serialize(valoresAntiguos, new JsonSerializerOptions { WriteIndented = false })
                    : null,
                ValoresNuevos = valoresNuevos != null 
                    ? JsonSerializer.Serialize(valoresNuevos, new JsonSerializerOptions { WriteIndented = false })
                    : null,
                IpAddress = ipAddress,
                Exitoso = exitoso,
                MensajeError = mensajeError,
                FechaHora = DateTime.UtcNow
            };

            await _unitOfWork.AuditoriaLogs.AddAsync(log);
            await _unitOfWork.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            // Si falla el registro de auditoría, no queremos que falle la operación principal
            Console.WriteLine($"Error al registrar auditoría: {ex.Message}");
        }
    }

    public async Task<IEnumerable<AuditoriaLogResponse>> GetLogsAsync(
        int? usuarioId = null,
        TipoAccion? tipoAccion = null,
        string? entidad = null,
        DateTime? fechaDesde = null,
        DateTime? fechaHasta = null,
        int pageNumber = 1,
        int pageSize = 50)
    {
        var logs = await _unitOfWork.AuditoriaLogs.GetLogsAsync(
            usuarioId, tipoAccion, entidad, fechaDesde, fechaHasta, pageNumber, pageSize);

        return logs.Select(log => new AuditoriaLogResponse
        {
            Id = log.Id,
            UsuarioId = log.UsuarioId,
            NombreUsuario = log.NombreUsuario,
            Rol = log.Rol,
            TipoAccion = log.TipoAccion.ToString(),
            Entidad = log.Entidad,
            EntidadId = log.EntidadId,
            Accion = log.Accion,
            Descripcion = log.Descripcion,
            ValoresAntiguos = log.ValoresAntiguos,
            ValoresNuevos = log.ValoresNuevos,
            IpAddress = log.IpAddress,
            Exitoso = log.Exitoso,
            MensajeError = log.MensajeError,
            FechaHora = log.FechaHora
        });
    }

    public async Task<int> GetTotalLogsAsync(
        int? usuarioId = null,
        TipoAccion? tipoAccion = null,
        string? entidad = null,
        DateTime? fechaDesde = null,
        DateTime? fechaHasta = null)
    {
        return await _unitOfWork.AuditoriaLogs.GetTotalLogsAsync(
            usuarioId, tipoAccion, entidad, fechaDesde, fechaHasta);
    }
}
