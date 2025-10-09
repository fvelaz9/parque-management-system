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

        var atraccion = repoAtracciones.Encontrar(y => y.Id == request.AtraccionId);
        if(atraccion == null)
        {
            return new ValidarAccesoResponse { AccesoPermitido = false, Mensaje = "Atracción no encontrada" };
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

        if(ticket.TipoEntrada == TipoTicket.EventoEspecial)
        {
            var errorEvento = VerificarEvento(ticket, atraccion, request.AtraccionId);
            if(errorEvento != null)
            {
                return errorEvento;
            }
        }

        var validacion = ValidarReglasAccesoUsuario(atraccion, ticket, request);
        if(validacion != null)
        {
            return validacion;
        }

        var incidencias = ValidarIncidencias(request, atraccion);
        if(incidencias != null)
        {
            return incidencias;
        }

        var visitantesActuales = ValidarCapacidad(request, atraccion);

        if(visitantesActuales != null)
        {
            return visitantesActuales;
        }

        return new ValidarAccesoResponse
        {
            AccesoPermitido = true,
            Mensaje = "Acceso permitido",
            NombreAtraccion = atraccion.Nombre,
            NombreVisitante = "Visitante"
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

        var evento = repoEvento.Encontrar(e => e.Id == ticket.EventoId.Value);

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

        // 4. Validar que el evento esté activo (dentro del rango de fechas)
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

    private ValidarAccesoResponse? ValidarReglasAccesoUsuario(AtraccionParque atraccion, Dominio.Ticket ticket, ValidarAccesoRequest request)
    {
        if(request.CuentaVisitante?.Id == null)
        {
            return new ValidarAccesoResponse
            {
                AccesoPermitido = false,
                Mensaje = "Cuenta no encontrada",
                NombreAtraccion = atraccion.Nombre
            };
        }

        var cuenta = repoCuentas.Encontrar(d => d.Id == request.CuentaVisitante.Id);
        if(cuenta == null)
        {
            return new ValidarAccesoResponse
            {
                AccesoPermitido = false,
                Mensaje = "Cuenta no encontrada",
                NombreAtraccion = atraccion.Nombre
            };
        }

        if(request.CuentaVisitante.ObtenerEdadVisitante() < atraccion.EdadMinima)
        {
            return new ValidarAccesoResponse
            {
                AccesoPermitido = false,
                Mensaje = $"Edad mínima requerida: {atraccion.EdadMinima} años",
                NombreAtraccion = atraccion.Nombre
            };
        }

        if(ticket.CuentaId != request.CuentaVisitante.Id)
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

    private ValidarAccesoResponse? ValidarIncidencias(ValidarAccesoRequest request, AtraccionParque atraccion)
    {
        var incidencias = repoIncidencias.Encontrar(d => d.AtraccionId == request.AtraccionId);
        var fechaActual = servicioFechaHora.ObtenerFechaActual();
        if(incidencias != null && !incidencias.EstaDisponible(fechaActual))
        {
            return new ValidarAccesoResponse
            {
                AccesoPermitido = false,
                Mensaje = "Atracción temporalmente fuera de servicio",
                NombreAtraccion = atraccion.Nombre
            };
        }

        return null;
    }

    private ValidarAccesoResponse? ValidarCapacidad(ValidarAccesoRequest request, AtraccionParque atraccion)
    {
        var visitantesActuales = repoRegistros.ObtenerTodos()
            .Count(r => r.AtraccionId == request.AtraccionId && r.FechaEgreso == null);

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

    public RegistroVisita RegistrarIngreso(Guid codigoTicket, int atraccionId, Cuenta cuentaVisitante)
    {
        var validacion = ValidarAcceso(new ValidarAccesoRequest
        {
            CodigoTicket = codigoTicket,
            AtraccionId = atraccionId,
            CuentaVisitante = cuentaVisitante
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
}
