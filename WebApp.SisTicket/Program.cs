using SisTicket.Core.Application;
using SisTicket.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Capa de Application (Servicios, AutoMapper, FluentValidation)
builder.Services.AddApplication();

// Capa de Infrastructure (DbContext, Repositorios, UnitOfWork)
builder.Services.AddInfrastructurePersistence(builder.Configuration);

var app = builder.Build();

app.MapGet("/", () => "SisTicket API - Sistema de Solicitudes Internas");

app.Run();
