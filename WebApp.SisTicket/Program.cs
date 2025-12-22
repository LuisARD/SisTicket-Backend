using SisTicket.Core.Application;
using SisTicket.Infrastructure.Persistence;
using WebApp.SisTicket.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Configuración de CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Controllers
builder.Services.AddControllers();

// Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "SisTicket API",
        Version = "v1",
        Description = "API del Sistema de Solicitudes Internas - Mesa de Servicios",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "SisTicket Team",
            Email = "support@sisticket.com"
        }
    });
});

// Capa de Application (Servicios, AutoMapper, FluentValidation)
builder.Services.AddApplication();

// Capa de Infrastructure (DbContext, Repositorios, UnitOfWork)
builder.Services.AddInfrastructurePersistence(builder.Configuration);

var app = builder.Build();

// Middleware de excepciones global (debe ir primero)
app.UseMiddleware<ExceptionHandlerMiddleware>();

// Swagger (solo en desarrollo)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "SisTicket API v1");
        options.RoutePrefix = string.Empty; // Swagger en la raíz (http://localhost:5000)
    });
}

// CORS
app.UseCors("AllowAll");

// HTTPS Redirection
app.UseHttpsRedirection();

// Authorization (se configurará en la parte 2)
app.UseAuthorization();

// Map Controllers
app.MapControllers();

app.Run();
