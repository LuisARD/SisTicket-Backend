using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using SisTicket.Core.Application.Exceptions;
using SisTicket.Core.Application.Services.Interfaces;

namespace WebApp.SisTicket.Services;

public class LocalFileStorageService : IFileStorageService
{
    private readonly string _uploadsBasePath;
    private readonly ILogger<LocalFileStorageService> _logger;

    private const string UploadsFolder = "uploads";
    private const string SolicitudesFolder = "solicitudes";
    private const long MaxFileSizeBytes = 10_485_760; // 10MB
    
    private static readonly string[] AllowedExtensions = 
    { 
        ".pdf", ".png", ".jpg", ".jpeg", ".gif",
        ".doc", ".docx", ".xls", ".xlsx", ".txt",
        ".zip", ".rar"
    };

    public LocalFileStorageService(
        IWebHostEnvironment env,
        ILogger<LocalFileStorageService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        
        var webRootPath = env.WebRootPath ?? throw new InvalidOperationException("WebRootPath is not configured");
        _uploadsBasePath = Path.Combine(webRootPath, UploadsFolder, SolicitudesFolder);
        
        EnsureDirectoryExists(_uploadsBasePath);
        _logger.LogInformation("LocalFileStorageService initialized. Base path: {BasePath}", _uploadsBasePath);
    }

    public async Task<string> GuardarArchivoAsync(IFormFile archivo, int solicitudId)
    {
        ValidateFile(archivo);
        ValidateSolicitudId(solicitudId);

        var extension = Path.GetExtension(archivo.FileName).ToLowerInvariant();
        var uniqueFileName = GenerateUniqueFileName(extension);
        var solicitudDirectory = GetSolicitudDirectory(solicitudId);
        var fullPath = Path.Combine(solicitudDirectory, uniqueFileName);
        
        EnsureDirectoryExists(solicitudDirectory);

        try
        {
            await SaveFileToPathAsync(archivo, fullPath);
            _logger.LogInformation("File saved successfully: {FileName} for Solicitud {SolicitudId}", uniqueFileName, solicitudId);
            
            return Path.Combine(solicitudId.ToString(), uniqueFileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving file {FileName} for Solicitud {SolicitudId}", archivo.FileName, solicitudId);
            throw new ValidationException("Error al guardar el archivo. Por favor, inténtelo nuevamente.");
        }
    }

    public async Task<Stream> DescargarArchivoAsync(string rutaArchivo)
    {
        ValidateFilePath(rutaArchivo);

        var fullPath = Path.Combine(_uploadsBasePath, rutaArchivo);
        
        if (!File.Exists(fullPath))
        {
            _logger.LogWarning("File not found: {FilePath}", fullPath);
            throw new NotFoundException("Archivo", rutaArchivo);
        }

        try
        {
            var memoryStream = new MemoryStream();
            using (var fileStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                await fileStream.CopyToAsync(memoryStream);
            }
            
            memoryStream.Position = 0;
            _logger.LogInformation("File downloaded successfully: {FilePath}", rutaArchivo);
            return memoryStream;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error downloading file: {FilePath}", rutaArchivo);
            throw new ValidationException("Error al descargar el archivo. Por favor, inténtelo nuevamente.");
        }
    }

    public Task EliminarArchivoAsync(string rutaArchivo)
    {
        ValidateFilePath(rutaArchivo);

        var fullPath = Path.Combine(_uploadsBasePath, rutaArchivo);
        
        if (!File.Exists(fullPath))
        {
            _logger.LogWarning("Attempted to delete non-existent file: {FilePath}", fullPath);
            return Task.CompletedTask;
        }

        try
        {
            File.Delete(fullPath);
            _logger.LogInformation("File deleted successfully: {FilePath}", rutaArchivo);
            
            DeleteEmptyDirectory(Path.GetDirectoryName(fullPath));
            
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting file: {FilePath}", rutaArchivo);
            throw new ValidationException("Error al eliminar el archivo. Por favor, inténtelo nuevamente.");
        }
    }

    public Task<bool> ArchivoExisteAsync(string rutaArchivo)
    {
        if (string.IsNullOrWhiteSpace(rutaArchivo))
            return Task.FromResult(false);

        var fullPath = Path.Combine(_uploadsBasePath, rutaArchivo);
        return Task.FromResult(File.Exists(fullPath));
    }

    public bool EsArchivoPermitido(string nombreArchivo)
    {
        if (string.IsNullOrWhiteSpace(nombreArchivo))
            return false;

        var extension = Path.GetExtension(nombreArchivo).ToLowerInvariant();
        return AllowedExtensions.Contains(extension);
    }

    public bool EsTamanoPermitido(long tamanoBytes)
    {
        return tamanoBytes > 0 && tamanoBytes <= MaxFileSizeBytes;
    }

    #region Private Helper Methods

    private void ValidateFile(IFormFile archivo)
    {
        if (archivo == null || archivo.Length == 0)
            throw new ValidationException("El archivo está vacío");

        if (!EsArchivoPermitido(archivo.FileName))
        {
            var allowedExtensionsString = string.Join(", ", AllowedExtensions);
            throw new ValidationException($"Tipo de archivo no permitido. Solo se permiten: {allowedExtensionsString}");
        }

        if (!EsTamanoPermitido(archivo.Length))
        {
            var maxSizeMB = MaxFileSizeBytes / 1024 / 1024;
            throw new ValidationException($"El archivo excede el tamaño máximo permitido de {maxSizeMB}MB");
        }
    }

    private static void ValidateSolicitudId(int solicitudId)
    {
        if (solicitudId <= 0)
            throw new ValidationException("El ID de solicitud no es válido");
    }

    private static void ValidateFilePath(string rutaArchivo)
    {
        if (string.IsNullOrWhiteSpace(rutaArchivo))
            throw new ValidationException("La ruta del archivo no es válida");
        
        if (rutaArchivo.Contains("..") || Path.IsPathRooted(rutaArchivo))
            throw new ValidationException("La ruta del archivo contiene caracteres no permitidos");
    }

    private string GetSolicitudDirectory(int solicitudId)
    {
        return Path.Combine(_uploadsBasePath, solicitudId.ToString());
    }

    private static string GenerateUniqueFileName(string extension)
    {
        return $"{Guid.NewGuid()}{extension}";
    }

    private void EnsureDirectoryExists(string directoryPath)
    {
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
            _logger.LogDebug("Directory created: {DirectoryPath}", directoryPath);
        }
    }

    private static async Task SaveFileToPathAsync(IFormFile archivo, string fullPath)
    {
        using var fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.None);
        await archivo.CopyToAsync(fileStream);
    }

    private void DeleteEmptyDirectory(string? directoryPath)
    {
        if (string.IsNullOrWhiteSpace(directoryPath) || !Directory.Exists(directoryPath))
            return;

        try
        {
            if (Directory.GetFiles(directoryPath).Length == 0 && 
                Directory.GetDirectories(directoryPath).Length == 0)
            {
                Directory.Delete(directoryPath);
                _logger.LogDebug("Empty directory deleted: {DirectoryPath}", directoryPath);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Could not delete empty directory: {DirectoryPath}", directoryPath);
        }
    }

    #endregion
}
