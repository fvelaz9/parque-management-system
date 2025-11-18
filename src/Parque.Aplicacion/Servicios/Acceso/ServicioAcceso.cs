using Parque.Aplicacion.DTOs;
using Parque.Aplicacion.Servicios.Gamificacion;
using Parque.Dominio;
using Parque.Dominio.Atracciones;
using Parque.Dominio.Excepciones;
using Parque.Dominio.Usuarios;
using Parque.Infraestructura.Repositorios;

namespace Parque.Aplicacion.Servicios.Acceso;

public class ServicioAcceso(IRepositorio<AtraccionParque> repoAtracciones, IRepositorio<Dominio.Ticket> repoTickets,
    IRepositorio<RegistroVisita> repoRegistros, IRepositorio<Incidencia> repoIncidencias, IRepositorio<Cuenta> repoCuentas,
    IRepositorio<Evento> repoEvento, IServicioFechaHora servicioFechaHora, IServicioPuntuacion servicioPuntuacion) : IServicioAcceso
{
    public ValidarAccesoResponse ValidarAcceso(ValidarAccesoRequest request)
    {
        var ticket = repoTickets.Encontrar(d => d.Codigo == request.CodigoTicket);

        if(ticket == null)
        {
            return new ValidarAccesoResponse { AccesoPermitido = false, Mensaje = "El ticket no fue encontrado" };
        }

        if(!ticket.EsValido)
        {
            return new ValidarAccesoResponse { AccesoPermitido = false, Mensaje = "El ticket no es válido" };
        }

        var atraccion = repoAtracciones.Encontrar(y => y.Id == request.AtraccionId);
        if(atraccion == null)
        {
            return new ValidarAccesoResponse { AccesoPermitido = false, Mensaje = "Atracción no encontrada" };
        }

        var cuentaVisitante = repoCuentas.Encontrar(c => c.Id == request.CuentaVisitanteId);
        if(cuentaVisitante == null)
        {
            return new ValidarAccesoResponse { AccesoPermitido = false, Mensaje = "Cuenta no encontrada" };
        }

        var fechaActual = servicioFechaHora.ObtenerFechaActual().Date;
        if(ticket.FechaVisita.Date != fechaActual)
        {
            return new ValidarAccesoResponse
            {
                AccesoPermitido = false,
                Mensaje = $"Ticket válido solo para {ticket.FechaVisita:dd/MM/yyyy}"
            };
        }

        if(ticket.TipoEntrada == TipoTicket.General)
        {
            var validacionGeneral = ValidarAtraccionParaTicketGeneral(request.AtraccionId, atraccion);
            if(validacionGeneral != null)
            {
                return validacionGeneral;
            }
        }

        if(ticket.TipoEntrada == TipoTicket.EventoEspecial)
        {
            var errorEvento = VerificarEvento(ticket, atraccion, request.AtraccionId);
            if(errorEvento != null)
            {
                return errorEvento;
            }
        }

        var validacion = ValidarReglasAccesoUsuario(atraccion, ticket, cuentaVisitante);
        if(validacion != null)
        {
            return validacion;
        }

        var incidencias = ValidarIncidencias(request.AtraccionId, atraccion);
        if(incidencias != null)
        {
            return incidencias;
        }

        var visitantesActuales = ValidarCapacidad(request.AtraccionId, atraccion);
        if(visitantesActuales != null)
        {
            return visitantesActuales;
        }

        return new ValidarAccesoResponse
        {
            AccesoPermitido = true,
            Mensaje = "Acceso permitido",
            NombreAtraccion = atraccion.Nombre,
            NombreVisitante = $"{cuentaVisitante.Nombre} {cuentaVisitante.Apellido}"
        };
    }

    private ValidarAccesoResponse? VerificarEvento(Dominio.Ticket ticket, AtraccionParque atraccion, int atraccionId)
    {
        if(ticket.EventoId == null)
        {
            return new ValidarAccesoResponse
            {
                AccesoPermitido = false,
                Mensaje = "Ticket de evento especial sin evento asociado",
                NombreAtraccion = atraccion.Nombre
            };
        }

        var evento = repoEvento.EncontrarConRelaciones(e => e.Id == ticket.EventoId.Value, "Atracciones");

        if(evento == null)
        {
            return new ValidarAccesoResponse
            {
                AccesoPermitido = false,
                Mensaje = "No se encontró el evento asociado al ticket",
                NombreAtraccion = atraccion.Nombre
            };
        }

        var atraccionIncluidaEnEvento = evento.Atracciones.Any(a => a.Id == atraccionId);

        if(!atraccionIncluidaEnEvento)
        {
            var atraccionesPermitidas = string.Join(", ", evento.Atracciones.Select(a => a.Nombre));
            return new ValidarAccesoResponse
            {
                AccesoPermitido = false,
                Mensaje = $"Esta atracción no forma parte del evento '{evento.Titulo}'. " +
                         $"Atracciones incluidas: {atraccionesPermitidas}",
                NombreAtraccion = atraccion.Nombre
            };
        }

        var fechaActual = servicioFechaHora.ObtenerFechaActual().Date;
        if(fechaActual < evento.Inicio.Date || fechaActual > evento.Fin.Date)
        {
            return new ValidarAccesoResponse
            {
                AccesoPermitido = false,
                Mensaje = $"Evento '{evento.Titulo}' no está activo. " +
                         $"Válido desde {evento.Inicio:dd/MM/yyyy} hasta {evento.Fin:dd/MM/yyyy}",
                NombreAtraccion = atraccion.Nombre
            };
        }

        if(ticket.FechaVisita.Date < evento.Inicio.Date || ticket.FechaVisita.Date > evento.Fin.Date)
        {
            return new ValidarAccesoResponse
            {
                AccesoPermitido = false,
                Mensaje = $"Fecha del ticket fuera del rango del evento. " +
                         $"Evento válido desde {evento.Inicio:dd/MM/yyyy} hasta {evento.Fin:dd/MM/yyyy}",
                NombreAtraccion = atraccion.Nombre
            };
        }

        return null;
    }

    private ValidarAccesoResponse? ValidarReglasAccesoUsuario(AtraccionParque atraccion, Dominio.Ticket ticket, Cuenta cuentaVisitante)
    {
        if(cuentaVisitante.ObtenerEdadVisitante() < atraccion.EdadMinima)
        {
            return new ValidarAccesoResponse
            {
                AccesoPermitido = false,
                Mensaje = $"Edad mínima requerida: {atraccion.EdadMinima} años",
                NombreAtraccion = atraccion.Nombre
            };
        }

        if(ticket.CuentaId != cuentaVisitante.Id)
        {
            return new ValidarAccesoResponse
            {
                AccesoPermitido = false,
                Mensaje = "El ticket no pertenece a esta cuenta",
                NombreAtraccion = atraccion.Nombre
            };
        }

        return null;
    }

    private ValidarAccesoResponse? ValidarIncidencias(int atraccionId, AtraccionParque atraccion)
    {
        var incidencias = repoIncidencias.ObtenerTodos()
            .Where(i => i.AtraccionId == atraccionId && i.EstaActiva(servicioFechaHora.ObtenerFechaActual()))
            .ToList();

        if(incidencias.Any())
        {
            var incidencia = incidencias.First();
            return new ValidarAccesoResponse
            {
                AccesoPermitido = false,
                Mensaje = $"Atracción fuera de servicio. {incidencia.Descripcion}. " +
                         $"Resolución estimada: {incidencia.FechaResolucionEstimada:dd/MM/yyyy HH:mm}",
                NombreAtraccion = atraccion.Nombre
            };
        }

        return null;
    }

    private ValidarAccesoResponse? ValidarCapacidad(int atraccionId, AtraccionParque atraccion)
    {
        var visitantesActuales = repoRegistros.ObtenerTodos()
            .Count(r => r.AtraccionId == atraccionId && r.FechaEgreso == null);

        if(visitantesActuales >= atraccion.Capacidad)
        {
            return new ValidarAccesoResponse
            {
                AccesoPermitido = false,
                Mensaje = $"Aforo completo ({atraccion.Capacidad} personas)",
                NombreAtraccion = atraccion.Nombre
            };
        }

        return null;
    }

    private ValidarAccesoResponse? ValidarAtraccionParaTicketGeneral(int atraccionId, AtraccionParque atraccion)
    {
        var fechaActual = servicioFechaHora.ObtenerFechaActual().Date;
        var eventosActivos = repoEvento.ObtenerConRelaciones(
            e => fechaActual >= e.Inicio.Date && fechaActual <= e.Fin.Date,
            "Atracciones");

        var eventoConAtraccion = eventosActivos
            .FirstOrDefault(e => e.Atracciones.Any(a => a.Id == atraccionId));

        if(eventoConAtraccion != null)
        {
            return new ValidarAccesoResponse
            {
                AccesoPermitido = false,
                Mensaje = $"Esta atracción forma parte del evento '{eventoConAtraccion.Titulo}'. " +
                          $"Se requiere ticket de Evento Especial. " +
                          $"Evento activo hasta {eventoConAtraccion.Fin:dd/MM/yyyy}",
                NombreAtraccion = atraccion.Nombre
            };
        }

        return null;
    }

    public RegistroVisita RegistrarIngreso(Guid codigoTicket, int atraccionId, Cuenta cuentaVisitante)
    {
        var validacion = ValidarAcceso(new ValidarAccesoRequest
        {
            CodigoTicket = codigoTicket,
            AtraccionId = atraccionId,
            CuentaVisitanteId = cuentaVisitante.Id
        });

        if(!validacion.AccesoPermitido)
        {
            throw new ArgumentException(validacion.Mensaje);
        }

        var registro = new RegistroVisita
        {
            AtraccionId = atraccionId,
            Identificador = codigoTicket,
            FechaIngreso = servicioFechaHora.ObtenerFechaActual()
        };
        var ticket = repoTickets.Encontrar(t => t.Codigo == codigoTicket);
        if(ticket != null)
        {
            ticket.MarcarComoUsado();
            repoTickets.Editar(ticket);
        }

        repoRegistros.Agregar(registro);
        return registro;
    }

    public RegistroVisita RegistrarEgreso(Guid codigoTicket, int atraccionId)
    {
        var registro = repoRegistros.Encontrar(a => a.Identificador == codigoTicket && a.AtraccionId == atraccionId && a.FechaEgreso == null);

        if(registro == null)
        {
            throw new ArgumentException("No hay ingreso registrado o ya se registró el egreso");
        }

        registro.FechaEgreso = servicioFechaHora.ObtenerFechaActual();
        repoRegistros.Editar(registro);

        servicioPuntuacion.CalcularYRegistrarPuntos(registro.Id);

        return registro;
    }

    public AforoResponse ObtenerAforoAtraccion(int atraccionId)
    {
        var atraccion = repoAtracciones.Encontrar(a => a.Id == atraccionId);
        if(atraccion == null)
        {
            throw new ExcepcionEntidadNoEncontrada("Atracción no encontrada");
        }

        var visitantesActuales = repoRegistros.ObtenerTodos()
            .Count(r => r.AtraccionId == atraccionId && r.FechaEgreso == null);

        var capacidadRestante = atraccion.Capacidad - visitantesActuales;
        var porcentajeOcupacion = atraccion.Capacidad > 0
            ? (visitantesActuales * 100.0) / atraccion.Capacidad
            : 0;

        return new AforoResponse
        {
            AtraccionId = atraccionId,
            NombreAtraccion = atraccion.Nombre,
            CapacidadTotal = atraccion.Capacidad,
            VisitantesActuales = visitantesActuales,
            CapacidadRestante = Math.Max(0, capacidadRestante),
            PorcentajeOcupacion = Math.Round(porcentajeOcupacion, 2),
            AforoCompleto = visitantesActuales >= atraccion.Capacidad
        };
    }

    public List<RegistroVisitaDto> ObtenerRegistrosActivosPorUsuario(Guid usuarioId, int atraccionId)
    {
        var registrosActivos = repoRegistros.ObtenerTodos()
            .Where(r => r.FechaEgreso == null && r.AtraccionId == atraccionId)
            .ToList();

        var ids = registrosActivos.Select(r => r.Identificador).ToList();

        var tickets = repoTickets.Obtener(t => ids.Contains(t.Codigo) && t.CuentaId == usuarioId);

        return registrosActivos
            .Where(r => tickets.Any(t => t.Codigo == r.Identificador))
            .Select(r => new RegistroVisitaDto
            {
                Id = r.Id,
                Identificador = r.Identificador,
                FechaIngreso = r.FechaIngreso,
                AtraccionId = r.AtraccionId
            })
            .ToList();
    }
}
