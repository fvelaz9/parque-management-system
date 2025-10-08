using Microsoft.EntityFrameworkCore;
using Parque.Dominio;
using Parque.Dominio.Atracciones;
using Parque.Dominio.Gamificacion;
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
    public DbSet<PuntuacionVisitante> PuntuacionesVisitantes { get; set; }
    public DbSet<ConfiguracionEstrategia> ConfiguracionesEstrategia { get; set; }

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
        modelBuilder.Entity<PuntuacionVisitante>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.VisitanteId).IsRequired();
            entity.Property(e => e.Fecha).IsRequired();
            entity.HasIndex(e => new { e.VisitanteId, e.Fecha }).IsUnique();
        });

        modelBuilder.Entity<ConfiguracionEstrategia>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.EstrategiaActiva).IsRequired().HasMaxLength(50);
        });
    }
}
