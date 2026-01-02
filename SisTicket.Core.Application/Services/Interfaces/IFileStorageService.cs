using Microsoft.AspNetCore.Http;

namespace SisTicket.Core.Application.Services.Interfaces;

public interface IFileStorageService
{
    Task<string> GuardarArchivoAsync(IFormFile archivo, int solicitudId);
    Task<Stream> DescargarArchivoAsync(string rutaArchivo);
    Task EliminarArchivoAsync(string rutaArchivo);
    Task<bool> ArchivoExisteAsync(string rutaArchivo);
    bool EsArchivoPermitido(string nombreArchivo);
    bool EsTamanoPermitido(long tamanoBytes);
}
