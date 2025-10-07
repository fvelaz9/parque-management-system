using Microsoft.EntityFrameworkCore;
using Parque.Aplicacion.Servicios;
using Parque.Aplicacion.Servicios.Atracciones;
using Parque.Aplicacion.Servicios.Ticket;
using Parque.Infraestructura;
using Parque.Infraestructura.Repositorios;
using Parque.WebApi.Filtros;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Repositorio
builder.Services.AddScoped(typeof(IRepositorio<>), typeof(Repositorio<>));

// Registrar ExceptionFilter globalmente
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ExceptionFilter>();
});

// Servicios de Aplicacion
builder.Services.AddSingleton<IServicioFechaHora, ServicioFechaHora>();
builder.Services.AddScoped<IServicioCuenta, ServicioCuenta>();
builder.Services.AddScoped<IServicioAtracciones, ServicioAtracciones>();
builder.Services.AddScoped<IServicioTicket, ServicioTicket>();
builder.Services.AddScoped<IServicioEvento, ServicioEvento>();
builder.Services.AddScoped<IServicioSesion, ServicioSesion>();

// Base de datos
builder.Services.AddDbContext<AppContexto>(options => options.UseSqlServer("name=ConnectionStrings:DefaultConnection"));

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
