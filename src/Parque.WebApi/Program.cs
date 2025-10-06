using Microsoft.EntityFrameworkCore;
using Parque.Aplicacion.Servicios;
using Parque.Infraestructura;
using Parque.Infraestructura.Repositorios;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddSingleton<IServicioFechaHora, ServicioFechaHora>();

builder.Services.AddScoped<IServicioCuenta, ServicioCuenta>();

builder.Services.AddScoped(typeof(IRepositorio<>), typeof(Repositorio<>));

builder.Services.AddDbContext<AppContexto>(options => options.UseSqlServer("name=ConnectionStrings:DefaultConnection"));

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
