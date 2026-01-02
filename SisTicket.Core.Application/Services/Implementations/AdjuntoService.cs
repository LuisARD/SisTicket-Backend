using AutoMapper;
using Microsoft.AspNetCore.Http;
using SisTicket.Core.Application.DTOs.Adjunto;
using SisTicket.Core.Application.Exceptions;
using SisTicket.Core.Application.Services.Interfaces;
using SisTicket.Core.Domain.Entities;
using SisTicket.Core.Domain.Interfaces;

namespace SisTicket.Core.Application.Services.Implementations;

public class AdjuntoService : IAdjuntoService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileStorageService _fileStorageService;
    private readonly IMapper _mapper;
    private const int MaxArchivosPermitidos = 5;

    public AdjuntoService(
        IUnitOfWork unitOfWork,
        IFileStorageService fileStorageService,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _fileStorageService = fileStorageService;
        _mapper = mapper;
    }

    public async Task<IEnumerable<AdjuntoResponse>> GetBySolicitudIdAsync(int solicitudId)
    {
        var solicitud = await _unitOfWork.Solicitudes.GetByIdAsync(solicitudId);
        if (solicitud == null)
            throw new NotFoundException(nameof(Solicitud), solicitudId);

        var adjuntos = await _unitOfWork.Adjuntos.GetBySolicitudIdAsync(solicitudId);
        return _mapper.Map<IEnumerable<AdjuntoResponse>>(adjuntos);
    }

    public async Task<AdjuntoResponse> GetByIdAsync(int id)
    {
        var adjunto = await _unitOfWork.Adjuntos.GetByIdWithRelacionesAsync(id);
        if (adjunto == null)
            throw new NotFoundException(nameof(Adjunto), id);

        return _mapper.Map<AdjuntoResponse>(adjunto);
    }

    public async Task<AdjuntoResponse> SubirArchivoAsync(int solicitudId, IFormFile archivo, int usuarioId)
    {
        var solicitud = await _unitOfWork.Solicitudes.GetByIdAsync(solicitudId);
        if (solicitud == null)
            throw new NotFoundException(nameof(Solicitud), solicitudId);

        var cantidadAdjuntos = await _unitOfWork.Adjuntos.ContarPorSolicitudAsync(solicitudId);
        if (cantidadAdjuntos >= MaxArchivosPermitidos)
            throw new ValidationException($"La solicitud no puede tener más de {MaxArchivosPermitidos} archivos adjuntos");

        var rutaArchivo = await _fileStorageService.GuardarArchivoAsync(archivo, solicitudId);

        var adjunto = new Adjunto
        {
            SolicitudId = solicitudId,
            NombreArchivo = Path.GetFileName(rutaArchivo),
            NombreArchivoOriginal = archivo.FileName,
            RutaArchivo = rutaArchivo,
            TipoContenido = archivo.ContentType,
            TamanoBytes = archivo.Length,
            CargadoPorId = usuarioId
        };

        await _unitOfWork.Adjuntos.AddAsync(adjunto);
        await _unitOfWork.SaveChangesAsync();

        return await GetByIdAsync(adjunto.Id);
    }

    public async Task<(Stream stream, string nombreArchivo, string tipoContenido)> DescargarArchivoAsync(
        int solicitudId, 
        int usuarioId)
    {
        var solicitud = await _unitOfWork.Solicitudes.GetByIdAsync(solicitudId);
        if (solicitud == null)
            throw new NotFoundException(nameof(Solicitud), solicitudId);

        var adjuntos = await _unitOfWork.Adjuntos.GetBySolicitudIdAsync(solicitudId);
        var adjunto = adjuntos.FirstOrDefault();

        if (adjunto == null)
            throw new ValidationException("Esta solicitud no contiene ningún archivo adjunto");

        var stream = await _fileStorageService.DescargarArchivoAsync(adjunto.RutaArchivo);
        return (stream, adjunto.NombreArchivoOriginal, adjunto.TipoContenido);
    }

    public async Task EliminarAsync(int adjuntoId, int solicitudId, int usuarioId)
    {
        var adjunto = await _unitOfWork.Adjuntos.GetByIdWithRelacionesAsync(adjuntoId);
        if (adjunto == null)
            throw new NotFoundException(nameof(Adjunto), adjuntoId);

        if (adjunto.SolicitudId != solicitudId)
            throw new ValidationException("El adjunto no pertenece a la solicitud especificada");

        await _fileStorageService.EliminarArchivoAsync(adjunto.RutaArchivo);
        
        await _unitOfWork.Adjuntos.DeleteAsync(adjuntoId);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<bool> UsuarioPuedeAccederAsync(int solicitudId, int usuarioId, string rol)
    {
        var solicitud = await _unitOfWork.Solicitudes.GetByIdAsync(solicitudId);
        if (solicitud == null)
            return false;

        if (rol == "SuperAdmin" || rol == "Admin")
            return true;

        if (solicitud.SolicitanteId == usuarioId)
            return true;

        if (solicitud.GestorAsignadoId == usuarioId)
            return true;

        return false;
    }
}
