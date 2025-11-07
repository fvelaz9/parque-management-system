using Microsoft.EntityFrameworkCore;
using Parque.Dominio.Usuarios;
using Parque.Infraestructura;
using Parque.Infraestructura.Repositorios;
using Parque.WebApi.Configuracion;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AgregarControladores();
builder.Services.AgregarCors();
builder.Services.AgregarServicios();
builder.Services.AgregarBaseDatos();

var app = builder.Build();

// Asegurar que la base de datos esté creada y migrada
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppContexto>();
    try
    {
        context.Database.Migrate();
        var repo = scope.ServiceProvider.GetRequiredService<IRepositorio<Cuenta>>();
        var todasLasCuentas = repo.ObtenerTodos();
        var adminExistente = todasLasCuentas.FirstOrDefault(c => c.Roles.Contains(Rol.Administrador));

        if (adminExistente == null)
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
        else
        {
            Console.WriteLine("Administrador ya existe en el sistema.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error al inicializar la base de datos: {ex.Message}");
        throw;
    }
}

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.UseAuthorization();
app.MapControllers();

app.Run();
