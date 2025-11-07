using Microsoft.EntityFrameworkCore;
using Parque.Dominio.Usuarios;
using Parque.Infraestructura.Repositorios;
using Parque.WebApi.Configuracion;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AgregarControladores();
builder.Services.AgregarCors();
builder.Services.AgregarServicios();
builder.Services.AgregarBaseDatos();

var app = builder.Build();

// Crear un admin inicial si no existe
using(var scope = app.Services.CreateScope())
{
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
app.UseCors("AllowFrontend");
app.UseAuthorization();
app.MapControllers();

app.Run();
