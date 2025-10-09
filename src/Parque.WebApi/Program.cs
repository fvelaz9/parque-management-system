using Microsoft.EntityFrameworkCore;
using Parque.Aplicacion.Servicios;
using Parque.Aplicacion.Servicios.Acceso;
using Parque.Aplicacion.Servicios.Atracciones;
using Parque.Aplicacion.Servicios.Gamificacion;
using Parque.Aplicacion.Servicios.Incidencias;
using Parque.Aplicacion.Servicios.Ticket;
using Parque.Dominio.Gamificacion;
using Parque.Dominio.Usuarios;
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
builder.Services.AddScoped<IServicioFechaHora, ServicioFechaHora>();
builder.Services.AddScoped<IServicioCuenta, ServicioCuenta>();
builder.Services.AddScoped<IServicioAtracciones, ServicioAtracciones>();
builder.Services.AddScoped<IServicioTicket, ServicioTicket>();
builder.Services.AddScoped<IServicioEvento, ServicioEvento>();
builder.Services.AddScoped<IServicioSesion, ServicioSesion>();
builder.Services.AddScoped<IServicioAcceso, ServicioAcceso>();
builder.Services.AddScoped<IServicioIncidencia, ServicioIncidencia>();

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

// Base de datos
builder.Services.AddDbContext<AppContexto>(options => options.UseSqlServer("name=ConnectionStrings:DefaultConnection"));

var app = builder.Build();

// Crear un admin inicial si no existe
using(var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppContexto>();
    var repo = scope.ServiceProvider.GetRequiredService<IRepositorio<Cuenta>>();

    // Verificar si ya existe un administrador
    var todasLasCuentas = repo.ObtenerTodos();
    var adminExistente = todasLasCuentas.FirstOrDefault(c => c.Roles.Contains(Rol.Administrador));

    if(adminExistente == null)
    {
        var adminEmail = new Email("admin@admin.com");
        var adminInicial = Cuenta.Crear(
            "Administrador",
            "Sistema",
            adminEmail,
            "Admin123!",
            Rol.Administrador);

        repo.Agregar(adminInicial);
        Console.WriteLine("Admin inicial creado: admin@admin.com / Admin123!");
    }
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
