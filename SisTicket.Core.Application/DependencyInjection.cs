using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SisTicket.Core.Application.Services.Implementations;
using SisTicket.Core.Application.Services.Interfaces;
using System.Reflection;

namespace SisTicket.Core.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // AutoMapper - Registra todos los perfiles de mapeo del assembly
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        
        // FluentValidation - Registra todos los validadores del assembly
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        
        // Servicios de Aplicación
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUsuarioService, UsuarioService>();
        services.AddScoped<ISolicitudService, SolicitudService>();
        services.AddScoped<IComentarioService, ComentarioService>();
        services.AddScoped<IAreaService, AreaService>();
        services.AddScoped<IPrioridadService, PrioridadService>();
        services.AddScoped<ITipoSolicitudService, TipoSolicitudService>();
        services.AddScoped<IAdjuntoService, AdjuntoService>();
        
        // NOTA: IPasswordHasher e IFileStorageService se registrarán en la capa de Presentation
        // porque dependen de librerías específicas de infraestructura (BCrypt, IWebHostEnvironment)
        
        return services;
    }
}
