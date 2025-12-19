using SisTicket.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Agregar servicios de Infrastructure (DbContext, Repositories, UnitOfWork)
builder.Services.AddInfrastructurePersistence(builder.Configuration);

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
