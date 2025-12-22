using SisTicket.Core.Application.Exceptions;
using System.Net;
using System.Text.Json;
using AppException = SisTicket.Core.Application.Exceptions.ApplicationException;

namespace WebApp.SisTicket.Middleware;

public class ExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlerMiddleware> _logger;
    private readonly IHostEnvironment _environment;

    public ExceptionHandlerMiddleware(
        RequestDelegate next, 
        ILogger<ExceptionHandlerMiddleware> logger,
        IHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var response = context.Response;
        response.ContentType = "application/json";

        var errorResponse = exception switch
        {
            NotFoundException notFoundEx => new ErrorResponse
            {
                StatusCode = (int)HttpStatusCode.NotFound,
                Message = notFoundEx.Message,
                Details = _environment.IsDevelopment() ? notFoundEx.StackTrace : null
            },

            ValidationException validationEx => new ErrorResponse
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Message = validationEx.Message,
                Errors = validationEx.Errors.Any() ? validationEx.Errors : null,
                Details = _environment.IsDevelopment() ? validationEx.StackTrace : null
            },

            UnauthorizedException unauthorizedEx => new ErrorResponse
            {
                StatusCode = (int)HttpStatusCode.Unauthorized,
                Message = unauthorizedEx.Message,
                Details = _environment.IsDevelopment() ? unauthorizedEx.StackTrace : null
            },

            AppException appEx => new ErrorResponse
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Message = appEx.Message,
                Details = _environment.IsDevelopment() ? appEx.StackTrace : null
            },

            _ => new ErrorResponse
            {
                StatusCode = (int)HttpStatusCode.InternalServerError,
                Message = "Ha ocurrido un error interno en el servidor",
                Details = _environment.IsDevelopment() ? exception.StackTrace : null
            }
        };

        response.StatusCode = errorResponse.StatusCode;

        // Log del error
        if (errorResponse.StatusCode >= 500)
        {
            _logger.LogError(exception, "Error interno del servidor: {Message}", exception.Message);
        }
        else
        {
            _logger.LogWarning(exception, "Error de aplicación: {Message}", exception.Message);
        }

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        var result = JsonSerializer.Serialize(errorResponse, jsonOptions);
        await response.WriteAsync(result);
    }

    private class ErrorResponse
    {
        public int StatusCode { get; set; }
        public string Message { get; set; } = string.Empty;
        public IDictionary<string, string[]>? Errors { get; set; }
        public string? Details { get; set; }
    }
}
