using Microsoft.EntityFrameworkCore;
using Parque.Dominio;
using Parque.Dominio.Atracciones;
using Parque.Dominio.Usuarios;

namespace Parque.Infraestructura;

public class AppContexto(DbContextOptions options) : DbContext(options)
{
    public DbSet<Cuenta> Cuentas { get; set; }
    public DbSet<Visitante> Visitantes { get; set; }
    public DbSet<Evento> Eventos { get; set; }
    public DbSet<Ticket> Tickets { get; set; }
    public DbSet<AtraccionParque> Atracciones { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Cuenta>(builder =>
        {
            builder.OwnsOne(u => u.Email, email =>
            {
                email.Property(e => e.Valor).HasColumnName("Email").IsRequired();
            });
            builder.Property(u => u.Password)
                .HasColumnName("Password")
                .IsRequired()
                .HasMaxLength(100);
        });
    }
}
