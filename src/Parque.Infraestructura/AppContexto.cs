using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
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
    public DbSet<Sesion> Sesiones { get; set; }
    public DbSet<PuntuacionVisitante> PuntuacionesVisitantes { get; set; }
    public DbSet<ConfiguracionEstrategia> ConfiguracionesEstrategia { get; set; }
    public DbSet<ConfiguracionFechaHora> ConfiguracionFechaHora { get; set; }
    public DbSet<Incidencia> Incidencias { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Cuenta>(static builder =>
        {
            builder.OwnsOne(u => u.Email, email =>
            {
                email.Property(e => e.Valor).HasColumnName("Email").IsRequired();
            });

            builder.Property(u => u.Password)
                .HasColumnName("Password")
                .IsRequired()
                .HasMaxLength(100);

            // Configurar la relación con Visitante
            builder.HasOne(c => c.Visitante)
                .WithOne()
                .HasForeignKey<Cuenta>("VisitanteId")
                .IsRequired(false);

            // Mapear la colección de Roles
            builder.Property<HashSet<Rol>>("_roles")
                .HasColumnName("Roles")
                .HasConversion(
                    roles => string.Join(",", roles.Select(r => ((int)r).ToString())),
                    rolesString => rolesString.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                              .Select(r => (Rol)int.Parse(r))
                                              .ToHashSet())
                .Metadata.SetValueComparer(new ValueComparer<HashSet<Rol>>(
                    (c1, c2) => c1!.SetEquals(c2!),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                    c => c.ToHashSet()));
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

            modelBuilder.Entity<ConfiguracionFechaHora>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FechaHoraConfigurada).IsRequired();
            });
        });
    }
}
