using SisTicket.Core.Application.Services.Interfaces;

namespace WebApp.SisTicket.Services;

/// <summary>
/// Configuración de opciones para el servicio de almacenamiento de archivos.
/// Útil para externalizar configuraciones y facilitar testing.
/// </summary>
public class FileStorageOptions
{
    public const string SectionName = "FileStorage";

    /// <summary>
    /// Extensiones de archivo permitidas.
    /// </summary>
    public string[] AllowedExtensions { get; set; } = 
    { 
        ".pdf", ".png", ".jpg", ".jpeg", ".gif",
        ".doc", ".docx", ".xls", ".xlsx", ".txt",
        ".zip", ".rar"
    };

    /// <summary>
    /// Tamaño máximo de archivo en bytes. Por defecto: 10MB
    /// </summary>
    public long MaxFileSizeBytes { get; set; } = 10_485_760; // 10MB

    /// <summary>
    /// Número máximo de archivos por solicitud.
    /// </summary>
    public int MaxFilesPerSolicitud { get; set; } = 5;

    /// <summary>
    /// Ruta relativa desde wwwroot para almacenar archivos.
    /// </summary>
    public string UploadPath { get; set; } = "uploads/solicitudes";

    /// <summary>
    /// Indica si se deben eliminar automáticamente los directorios vacíos.
    /// </summary>
    public bool DeleteEmptyDirectories { get; set; } = true;
}

/// <summary>
/// Extensión para validar la configuración de FileStorageOptions.
/// </summary>
public static class FileStorageOptionsExtensions
{
    public static void Validate(this FileStorageOptions options)
    {
        if (options == null)
            throw new ArgumentNullException(nameof(options));

        if (options.AllowedExtensions == null || options.AllowedExtensions.Length == 0)
            throw new InvalidOperationException("AllowedExtensions cannot be empty");

        if (options.MaxFileSizeBytes <= 0)
            throw new InvalidOperationException("MaxFileSizeBytes must be greater than 0");

        if (options.MaxFilesPerSolicitud <= 0)
            throw new InvalidOperationException("MaxFilesPerSolicitud must be greater than 0");

        if (string.IsNullOrWhiteSpace(options.UploadPath))
            throw new InvalidOperationException("UploadPath cannot be empty");
    }

    public static string GetMaxFileSizeDescription(this FileStorageOptions options)
    {
        var sizeMB = options.MaxFileSizeBytes / 1024.0 / 1024.0;
        return $"{sizeMB:F2} MB";
    }

    public static string GetAllowedExtensionsDescription(this FileStorageOptions options)
    {
        return string.Join(", ", options.AllowedExtensions);
    }
}
