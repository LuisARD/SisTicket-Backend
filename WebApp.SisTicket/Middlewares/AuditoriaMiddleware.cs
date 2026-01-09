using System.Security.Claims;
using SisTicket.Core.Application.Services.Interfaces;
using SisTicket.Core.Domain.Enums;

namespace WebApp.SisTicket.Middlewares;

public class AuditoriaMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<AuditoriaMiddleware> _logger;

    public AuditoriaMiddleware(RequestDelegate next, ILogger<AuditoriaMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, IAuditoriaService auditoriaService)
    {
        // Solo auditar endpoints de API
        if (!context.Request.Path.StartsWithSegments("/api"))
        {
            await _next(context);
            return;
        }

        var method = context.Request.Method;
        var path = context.Request.Path.Value ?? "";
        var usuarioId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var nombreUsuario = context.User.FindFirst(ClaimTypes.Name)?.Value;
        var rol = context.User.FindFirst(ClaimTypes.Role)?.Value;
        var ipAddress = context.Connection.RemoteIpAddress?.ToString();
        var userAgent = context.Request.Headers["User-Agent"].ToString();

        try
        {
            await _next(context);

            // Registrar acción exitosa (solo si no es Swagger ni archivos estáticos)
            if (!DebeIgnorarRuta(path))
            {
                var tipoAccion = DeterminarTipoAccion(method, path, context.Response.StatusCode);
                var entidad = ExtraerEntidad(path);
                var entidadId = ExtraerEntidadId(path);

                if (tipoAccion.HasValue)
                {
                    await auditoriaService.RegistrarAsync(
                        usuarioId != null ? int.Parse(usuarioId) : null,
                        nombreUsuario,
                        rol,
                        tipoAccion.Value,
                        entidad,
                        entidadId,
                        $"{method} {path}",
                        $"Status: {context.Response.StatusCode}",
                        ipAddress: ipAddress,
                        userAgent: userAgent,
                        exitoso: context.Response.StatusCode < 400
                    );
                }
            }
        }
        catch (Exception ex)
        {
            // Registrar error
            await auditoriaService.RegistrarAsync(
                usuarioId != null ? int.Parse(usuarioId) : null,
                nombreUsuario,
                rol,
                TipoAccion.Error,
                ExtraerEntidad(path),
                null,
                $"{method} {path}",
                "Error en la ejecución",
                ipAddress: ipAddress,
                userAgent: userAgent,
                exitoso: false,
                mensajeError: ex.Message
            );

            throw;
        }
    }

    private TipoAccion? DeterminarTipoAccion(string method, string path, int statusCode)
    {
        // Acciones específicas
        if (path.Contains("/login", StringComparison.OrdinalIgnoreCase)) 
            return TipoAccion.Login;
        
        if (path.Contains("/logout", StringComparison.OrdinalIgnoreCase)) 
            return TipoAccion.Logout;
        
        if (path.Contains("/tomar-solicitud", StringComparison.OrdinalIgnoreCase)) 
            return TipoAccion.TomarSolicitud;
        
        if (path.Contains("/asignar-gestor", StringComparison.OrdinalIgnoreCase)) 
            return TipoAccion.AsignarGestor;
        
        if (path.Contains("/cambiar-estado", StringComparison.OrdinalIgnoreCase)) 
            return TipoAccion.CambiarEstado;
        
        if (path.Contains("/adjuntos", StringComparison.OrdinalIgnoreCase) && method == "POST") 
            return TipoAccion.SubirAdjunto;
        
        if (path.Contains("/adjuntos", StringComparison.OrdinalIgnoreCase) && method == "GET") 
            return TipoAccion.DescargarAdjunto;
        
        if (path.Contains("/comentarios", StringComparison.OrdinalIgnoreCase) && method == "POST") 
            return TipoAccion.CrearComentario;
        
        if (path.Contains("/comentarios", StringComparison.OrdinalIgnoreCase) && method == "DELETE") 
            return TipoAccion.EliminarComentario;
        
        if (path.Contains("/estado", StringComparison.OrdinalIgnoreCase) && method == "PATCH")
        {
            // Determinar si es activar o desactivar leyendo el body (simplificado)
            return TipoAccion.ActivarUsuario;
        }
        
        // Acceso denegado
        if (statusCode == 403 || statusCode == 401) 
            return TipoAccion.AccesoDenegado;

        // Acciones genéricas por método HTTP
        return method switch
        {
            "GET" => TipoAccion.Leer,
            "POST" => TipoAccion.Crear,
            "PUT" => TipoAccion.Actualizar,
            "PATCH" => TipoAccion.Actualizar,
            "DELETE" => TipoAccion.Eliminar,
            _ => null
        };
    }

    private string ExtraerEntidad(string path)
    {
        var segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
        
        if (segments.Length > 1)
        {
            var entidad = segments[1];
            
            // Mapear nombres amigables
            return entidad switch
            {
                "solicitudes" => "Solicitudes",
                "usuarios" => "Usuarios",
                "areas" => "Areas",
                "prioridades" => "Prioridades",
                "tipossolicitud" => "TiposSolicitud",
                "auth" => "Autenticacion",
                "comentarios" => "Comentarios",
                "adjuntos" => "Adjuntos",
                _ => char.ToUpper(entidad[0]) + entidad.Substring(1)
            };
        }
        
        return "Desconocido";
    }

    private int? ExtraerEntidadId(string path)
    {
        var segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
        
        // Buscar el primer segmento numérico después de /api/entidad
        if (segments.Length > 2 && int.TryParse(segments[2], out var id))
        {
            return id;
        }
        
        return null;
    }

    private bool DebeIgnorarRuta(string path)
    {
        // Rutas que no queremos auditar
        var rutasIgnoradas = new[]
        {
            "/swagger",
            "/health",
            "/favicon.ico",
            "/_framework",
            "/.well-known"
        };
        
        return rutasIgnoradas.Any(r => path.Contains(r, StringComparison.OrdinalIgnoreCase));
    }
}
