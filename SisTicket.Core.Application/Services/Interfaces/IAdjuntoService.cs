using SisTicket.Core.Application.DTOs.Adjunto;
using Microsoft.AspNetCore.Http;

namespace SisTicket.Core.Application.Services.Interfaces;

public interface IAdjuntoService
{
    Task<IEnumerable<AdjuntoResponse>> GetBySolicitudIdAsync(int solicitudId);
    Task<AdjuntoResponse> GetByIdAsync(int id);
    Task<AdjuntoResponse> SubirArchivoAsync(int solicitudId, IFormFile archivo, int usuarioId);
    Task<(Stream stream, string nombreArchivo, string tipoContenido)> DescargarArchivoAsync(int solicitudId, int usuarioId);
    Task EliminarAsync(int adjuntoId, int solicitudId, int usuarioId);
    Task<bool> UsuarioPuedeAccederAsync(int solicitudId, int usuarioId, string rol);
}
