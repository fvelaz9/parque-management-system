using Microsoft.EntityFrameworkCore;
using Parque.Dominio;
using Parque.Dominio.Atracciones;
using Parque.Dominio.Usuarios;

namespace DefaultNamespace;

public class AppContexto : DbContext
{
    public DbSet<Cuenta> Cuentas { get; set; }
    public DbSet<Visitante> Visitantes { get; set; }
    public DbSet<Evento> Eventos { get; set; }
    public DbSet<Ticket> Tickets { get; set; }
    public DbSet<AtraccionParque> Atracciones { get; set; }
    public AppContexto(DbContextOptions options)
        : base(options) { }
}
