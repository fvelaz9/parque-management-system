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
    public DbSet<RegistroVisita> RegistrosVisitas { get; set; }
    public DbSet<Sesion> Sesiones { get; set; }

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

        modelBuilder.Entity<Evento>()
            .HasMany(e => e.Atracciones)
            .WithMany()
            .UsingEntity(j => j.ToTable("EventoAtracciones"));

        modelBuilder.Entity<Sesion>(builder =>
        {
            builder.HasKey(s => s.Id);
            builder.Property(s => s.Token)
                .IsRequired()
                .HasMaxLength(200);
            builder.HasIndex(s => s.Token)
                .IsUnique();
            builder.Property(s => s.UsuarioId)
                .IsRequired();
        });
    }
}
