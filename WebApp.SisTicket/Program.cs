using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SisTicket.Core.Application;
using SisTicket.Core.Application.Services.Interfaces;
using SisTicket.Infrastructure.Persistence;
using System.Text;
using WebApp.SisTicket.Configuration;
using WebApp.SisTicket.Middleware;
using WebApp.SisTicket.Services;

var builder = WebApplication.CreateBuilder(args);

// Configurar JwtSettings desde appsettings.json
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
builder.Services.Configure<JwtSettings>(jwtSettings);

var jwtConfig = jwtSettings.Get<JwtSettings>();
var key = Encoding.UTF8.GetBytes(jwtConfig!.SecretKey);

// Configuración de JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; // En producción cambiar a true
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtConfig.Issuer,
        ValidAudience = jwtConfig.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ClockSkew = TimeSpan.Zero
    };

    // Leer el token desde la cookie HTTP-Only
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            // Primero intenta leer desde el header Authorization
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            
            // Si no está en el header, busca en la cookie
            if (string.IsNullOrEmpty(token))
            {
                token = context.Request.Cookies["AuthToken"];
            }

            if (!string.IsNullOrEmpty(token))
            {
                context.Token = token;
            }

            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization();

// Configuración de CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:4200") // Frontend URLs
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials(); // Importante para cookies
    });
});

// Controllers
builder.Services.AddControllers();

// Swagger/OpenAPI con soporte JWT
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "SisTicket API",
        Version = "v1",
        Description = "API del Sistema de Solicitudes Internas - Mesa de Servicios con JWT Authentication",
        Contact = new OpenApiContact
        {
            Name = "SisTicket Team",
            Email = "support@sisticket.com"
        }
    });

    // Configurar JWT en Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Servicios de Infraestructura
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();

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

// Authentication & Authorization (orden importante)
app.UseAuthentication();
app.UseAuthorization();

// Map Controllers
app.MapControllers();

app.Run();
