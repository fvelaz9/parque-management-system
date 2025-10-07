using Parque.Aplicacion.DTOS;
using Parque.Aplicacion.Servicios.Incidencias;
using Parque.Dominio;
using Parque.Dominio.Atracciones;
using Parque.Dominio.Usuarios;
using Parque.Infraestructura.Repositorios;

namespace Parque.Aplicacion.Servicios.Acceso;

public class ServicioAcceso(IRepositorio<AtraccionParque> repoAtracciones, IRepositorio<Dominio.Ticket> repoTickets,
    IRepositorio<RegistroVisita> repoRegistros, IRepositorio<Incidencia> repoIncidencias, IRepositorio<Cuenta> repoCuentas, IRepositorio<Evento> repoEvento) : IServicioAcceso
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

        if(ticket.FechaVisita.Date != DateTime.Today)
        {
            return new ValidarAccesoResponse
            {
                AccesoPermitido = false,
                Mensaje = $"Ticket válido solo para {ticket.FechaVisita:dd/MM/yyyy}"
            };
        }

        // ACA SE MANEJARIA LA SITUACION LIMITE DE ATRACCION EN EVENTO ESPECIAL APARITR DE ACA AGREGAR COSA DE EVENTO
        if (ticket.TipoEntrada == TipoTicket.EventoEspecial)
        {
            var errorEvento = VerificarEvento(ticket, atraccion, request.AtraccionId);
            if (errorEvento != null)
            {
                return errorEvento;
            }
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

        if (request.CuentaVisitante.ObtenerEdadVisitante() < atraccion.EdadMinima)
        {
            return new ValidarAccesoResponse
            {
                AccesoPermitido = false,
                Mensaje = $"Edad mínima requerida: {atraccion.EdadMinima} años",
                NombreAtraccion = atraccion.Nombre
            };
        }

        var incidencias = repoIncidencias.Encontrar(d => d.AtraccionId == request.AtraccionId);
        if (incidencias != null && !incidencias.EstaDisponible())
        {
            return new ValidarAccesoResponse
            {
                AccesoPermitido = false,
                Mensaje = "Atracción temporalmente fuera de servicio",
                NombreAtraccion = atraccion.Nombre
            };
        }

        // 7. Validar aforo (visitantes dentro actualmente)
        var visitantesActuales = repoRegistros.ObtenerTodos()
            .Count(r => r.AtraccionId == request.AtraccionId && r.FechaEgreso == null);

        if (visitantesActuales >= atraccion.Capacidad)
        {
            return new ValidarAccesoResponse
            {
                AccesoPermitido = false,
                Mensaje = $"Aforo completo ({atraccion.Capacidad} personas)",
                NombreAtraccion = atraccion.Nombre
            };
        }

        if(ticket.CuentaId != request.CuentaVisitante.Id)
        {
            return new ValidarAccesoResponse
            {
                AccesoPermitido = false,
                Mensaje = "El cuenta no fue encontrada",
                NombreAtraccion = atraccion.Nombre
            };
        }

        // ACCESO PERMITIDO
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
        // 1. Verificar que el ticket tenga un EventoId
        if (ticket.EventoId == null)
        {
            return new ValidarAccesoResponse
            {
                AccesoPermitido = false,
                Mensaje = "Ticket de evento especial sin evento asociado",
                NombreAtraccion = atraccion.Nombre
            };
        }

        // 2. Buscar el evento asociado al ticket
        var evento = repoEvento.Encontrar(e => e.Id == ticket.EventoId.Value);
        
        if (evento == null)
        {
            return new ValidarAccesoResponse
            {
                AccesoPermitido = false,
                Mensaje = "No se encontró el evento asociado al ticket",
                NombreAtraccion = atraccion.Nombre
            };
        }

        // 3. Validar que la atracción esté incluida en el evento
        var atraccionIncluidaEnEvento = evento.Atracciones.Any(a => a.Id == atraccionId);

        if (!atraccionIncluidaEnEvento)
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
        if (DateTime.Today < evento.Inicio.Date || DateTime.Today > evento.Fin.Date)
        {
            return new ValidarAccesoResponse
            {
                AccesoPermitido = false,
                Mensaje = $"Evento '{evento.Titulo}' no está activo. " +
                         $"Válido desde {evento.Inicio:dd/MM/yyyy} hasta {evento.Fin:dd/MM/yyyy}",
                NombreAtraccion = atraccion.Nombre
            };
        }

        // 5. Validar que la fecha del ticket coincida con el rango del evento
        if (ticket.FechaVisita.Date < evento.Inicio.Date || ticket.FechaVisita.Date > evento.Fin.Date)
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

    public RegistroVisita RegistrarIngreso(Guid codigoTicket, int atraccionId, Cuenta cuentaVisitante)
    {
        // Primero validar acceso
        var validacion = ValidarAcceso(new ValidarAccesoRequest
        {
            CodigoTicket = codigoTicket,
            AtraccionId = atraccionId,
            CuentaVisitante = cuentaVisitante
        });

        if (!validacion.AccesoPermitido)
        {
            throw new ArgumentException(validacion.Mensaje);
        }

        var ticket = repoTickets.Encontrar(t => t.Codigo == codigoTicket);

        var registro = new RegistroVisita
        {
            AtraccionId = atraccionId,
            Identificador = codigoTicket,
            FechaIngreso = DateTime.Now
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

        registro.FechaEgreso = DateTime.Now;
        repoRegistros.Editar(registro);
        return registro;
    }
}
