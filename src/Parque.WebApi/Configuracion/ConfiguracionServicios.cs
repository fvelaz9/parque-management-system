using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Parque.Aplicacion.Servicios;
using Parque.Aplicacion.Servicios.Acceso;
using Parque.Aplicacion.Servicios.Atracciones;
using Parque.Aplicacion.Servicios.Gamificacion;
using Parque.Aplicacion.Servicios.Incidencias;
using Parque.Aplicacion.Servicios.Mantenimiento;
using Parque.Aplicacion.Servicios.Recompensas;
using Parque.Aplicacion.Servicios.Ticket;
using Parque.Dominio.Gamificacion;
using Parque.Infraestructura;
using Parque.Infraestructura.Repositorios;
using Parque.WebApi.Filtros;

namespace Parque.WebApi.Configuracion;
[ExcludeFromCodeCoverage]
public static class ConfiguracionServicios
{
    public static IServiceCollection AgregarServicios(this IServiceCollection services)
    {
        // Repositorio
        services.AddScoped(typeof(IRepositorio<>), typeof(Repositorio<>));

        // Servicios de Aplicacion
        services.AddScoped<IServicioFechaHora, ServicioFechaHora>();
        services.AddScoped<IServicioCuenta, ServicioCuenta>();
        services.AddScoped<IServicioAtracciones, ServicioAtracciones>();
        services.AddScoped<IServicioTicket, ServicioTicket>();
        services.AddScoped<IServicioEvento, ServicioEvento>();
        services.AddScoped<IServicioSesion, ServicioSesion>();
        services.AddScoped<IServicioAcceso, ServicioAcceso>();
        services.AddScoped<IServicioIncidencia, ServicioIncidencia>();
        services.AddScoped<IServicioMantenimiento, ServicioMantenimiento>();
        services.AddScoped<IServicioRecompensa, ServicioRecompensa>();

        // Estrategias de puntuacion
        services.AddScoped<IEstrategiaPuntuacion, PuntuacionPorAtraccion>(sp =>
            new PuntuacionPorAtraccion());

        services.AddScoped<IEstrategiaPuntuacion, PuntuacionCombo>(sp =>
            new PuntuacionCombo(
                minutosVentana: 10,
                atraccionesMinimasCombo: 3,
                puntosBase: 8,
                puntosCombo: 25));

        services.AddScoped<IEstrategiaPuntuacion, PuntuacionPorEvento>(sp =>
            new PuntuacionPorEvento());

        services.AddScoped<IServicioPuntuacion, ServicioPuntuacion>();

        return services;
    }

    public static IServiceCollection AgregarBaseDatos(this IServiceCollection services)
    {
        services.AddDbContext<AppContexto>(options =>
            options.UseSqlServer("name=ConnectionStrings:DefaultConnection"));

        return services;
    }

    public static IServiceCollection AgregarCors(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("AllowFrontend", policy =>
            {
                policy.WithOrigins("http://localhost:4200", "https://localhost:4200")
                      .AllowAnyMethod()
                      .AllowAnyHeader()
                      .AllowCredentials();
            });
        });

        return services;
    }

    public static IServiceCollection AgregarControladores(this IServiceCollection services)
    {
        services.AddControllers(options =>
        {
            options.Filters.Add<ExceptionFilter>();
        });

        return services;
    }
}
