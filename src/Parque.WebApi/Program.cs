using Microsoft.EntityFrameworkCore;
using Parque.Aplicacion.Servicios;
using Parque.Aplicacion.Servicios.Atracciones;
using Parque.Aplicacion.Servicios.Gamificacion;
using Parque.Aplicacion.Servicios.Ticket;
using Parque.Dominio.Gamificacion;
using Parque.Infraestructura;
using Parque.Infraestructura.Repositorios;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddScoped(typeof(IRepositorio<>), typeof(Repositorio<>));

builder.Services.AddSingleton<IServicioFechaHora, ServicioFechaHora>();

builder.Services.AddScoped<IServicioCuenta, ServicioCuenta>();

builder.Services.AddScoped<IServicioAtracciones, ServicioAtracciones>();

builder.Services.AddScoped<IServicioTicket, ServicioTicket>();

builder.Services.AddScoped<IServicioEvento, ServicioEvento>();

builder.Services.AddScoped<IEstrategiaPuntuacion, PuntuacionPorAtraccion>(sp =>
    new PuntuacionPorAtraccion());

builder.Services.AddScoped<IEstrategiaPuntuacion, PuntuacionCombo>(sp =>
    new PuntuacionCombo(
        minutosVentana: 10,
        atraccionesMinimasCombo: 3,
        puntosBase: 8,
        puntosCombo: 25));

builder.Services.AddScoped<IEstrategiaPuntuacion, PuntuacionPorEvento>(sp =>
    new PuntuacionPorEvento());

builder.Services.AddScoped<IServicioPuntuacion, ServicioPuntuacion>();

builder.Services.AddDbContext<AppContexto>(options => options.UseSqlServer("name=ConnectionStrings:DefaultConnection"));

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
