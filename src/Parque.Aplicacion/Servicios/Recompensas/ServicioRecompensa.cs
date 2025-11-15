using Parque.Aplicacion.DTOs.RecompensasDtos;
using Parque.Dominio.Gamificacion;
using Parque.Dominio.Usuarios;
using Parque.Infraestructura.Repositorios;

namespace Parque.Aplicacion.Servicios.Recompensas;

public class ServicioRecompensa(
    IRepositorio<Recompensa> repoRecompensa,
    IRepositorio<HistorialCanje> repoHistorial,
    IRepositorio<PuntuacionVisitante> repoPuntuacion,
    IRepositorio<Visitante> repoVisitante,
    IServicioFechaHora servicioFechaHora
) : IServicioRecompensa
{
    public Recompensa CrearRecompensa(RecompensaDto dto)
    {
        VerificacionRecompensa(dto);

        var recompensa = new Recompensa
        {
            Id = Guid.NewGuid(),
            Nombre = dto.Nombre,
            Descripcion = dto.Descripcion ?? string.Empty,
            CostoEnPuntos = dto.CostoEnPuntos,
            CantidadDisponible = dto.CantidadDisponible,
            NivelMembresiaRequerido = dto.NivelMembresiaRequerido,
            FechaCreacion = servicioFechaHora.ObtenerFechaActual()
        };

        repoRecompensa.Agregar(recompensa);

        return recompensa;
    }

    private void VerificacionRecompensa(RecompensaDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Nombre))
        {
            throw new InvalidOperationException("El nombre es obligatorio");
        }

        if (dto.Nombre.Length > 100)
        {
            throw new InvalidOperationException("El nombre no puede exceder los 100 caracteres");
        }

        if (dto.CostoEnPuntos <= 0)
        {
            throw new InvalidOperationException("El costo en puntos debe ser mayor a 0");
        }

        if (dto.CantidadDisponible < 0)
        {
            throw new InvalidOperationException("La cantidad disponible no puede ser negativa");
        }

        if (dto.Descripcion != null && dto.Descripcion.Length > 200)
        {
            throw new InvalidOperationException("La descripción no puede exceder 500 caracteres");
        }
    }

    public Recompensa ActualizarRecompensa(Guid id, RecompensaDto dto)
    {
        VerificacionRecompensa(dto);

        var recompensa = repoRecompensa.Encontrar(r => r.Id == id);
        if (recompensa == null)
        {
            throw new InvalidOperationException($"Recompensa con ID {id} no encontrada");
        }

        recompensa.Nombre = dto.Nombre;
        recompensa.Descripcion = dto.Descripcion ?? string.Empty;
        recompensa.CostoEnPuntos = dto.CostoEnPuntos;
        recompensa.CantidadDisponible = dto.CantidadDisponible;
        recompensa.NivelMembresiaRequerido = dto.NivelMembresiaRequerido;

        repoRecompensa.Editar(recompensa);

        return recompensa;
    }

    public List<Recompensa> ObtenerRecompensas()
    {
        return repoRecompensa.ObtenerTodos();
    }

    public Recompensa ObtenerRecompensaPorId(Guid id)
    {
        var recompensa = repoRecompensa.Encontrar(r => r.Id == id);

        if (recompensa == null)
        {
            throw new InvalidOperationException($"Recompensa con ID {id} no encontrada");
        }

        return recompensa;
    }

    public HistorialCanje CanjearRecompensa(CanjearRecompensaRequest request)
    {
        var visitante = repoVisitante.Encontrar(r => r.Id == request.VisitanteId);
        if(visitante == null)
        {
            throw new InvalidOperationException("Usuario no encontrado");
        }

        var recompensa = repoRecompensa.Encontrar(r => r.Id == request.RecompensaId);
        if (recompensa == null)
        {
            throw new InvalidOperationException($"Recompensa con ID {request.RecompensaId} no encontrada");
        }

        if (recompensa.NivelMembresiaRequerido.HasValue &&
            visitante.NivelMembresia < recompensa.NivelMembresiaRequerido.Value)
        {
            throw new InvalidOperationException("Nivel de membresía insuficiente para canjear esta recompensa");
        }

        var puntuaciones = repoPuntuacion.ObtenerTodos();
        var puntuacionVisitante = puntuaciones.FirstOrDefault(p => p.VisitanteId == request.VisitanteId);
        if (puntuacionVisitante == null || puntuacionVisitante.PuntosTotales < recompensa.CostoEnPuntos)
        {
            throw new InvalidOperationException("Puntos insuficientes para canjear esta recompensa");
        }

        recompensa.ReducirStock();
        puntuacionVisitante.PuntosTotales -= recompensa.CostoEnPuntos;

        var historial = new HistorialCanje
        {
            Id = Guid.NewGuid(),
            VisitanteId = request.VisitanteId,
            RecompensaId = request.RecompensaId,
            PuntosCanjeados = recompensa.CostoEnPuntos,
            FechaCanje = servicioFechaHora.ObtenerFechaActual()
        };
        repoHistorial.Agregar(historial);
        repoPuntuacion.Editar(puntuacionVisitante);
        return historial;
    }

    public List<HistorialCanjeDto> ObtenerHistorialCanjes(Guid visitanteId)
    {
        var historial = repoHistorial.ObtenerTodos()
            .Where(h => h.VisitanteId == visitanteId)
            .ToList();
        if(historial == null)
        {
            throw new InvalidOperationException("No hay transacciones registradas");
        }

        var dtos = new List<HistorialCanjeDto>();

        foreach (var canje in historial)
        {
            var recompensa = repoRecompensa.Encontrar(r => r.Id == canje.RecompensaId);

            dtos.Add(new HistorialCanjeDto
            {
                Id = canje.Id,
                VisitanteId = canje.VisitanteId,
                RecompensaId = canje.RecompensaId,
                NombreRecompensa = recompensa?.Nombre ?? "Recompensa no encontrada",
                PuntosCanjeados = canje.PuntosCanjeados,
                FechaCanje = canje.FechaCanje
            });
        }

        return dtos;
    }

    public void EliminarRecompensa(Guid id)
    {
        var recompesa = repoRecompensa.Encontrar(r => r.Id == id);
        if (recompesa == null)
        {
            throw new InvalidOperationException($"Recompensa con ID {id} no encontrada");
        }

        repoRecompensa.Eliminar(r => r.Id == id);
    }
}
