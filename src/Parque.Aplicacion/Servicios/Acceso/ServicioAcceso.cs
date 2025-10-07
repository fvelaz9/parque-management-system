using Parque.Aplicacion.DTOS;
using Parque.Aplicacion.Servicios.Incidencias;
using Parque.Dominio;
using Parque.Dominio.Atracciones;
using Parque.Infraestructura.Repositorios;

namespace Parque.Aplicacion.Servicios.Acceso;

public class ServicioAcceso(IRepositorio<AtraccionParque> repoAtracciones, IRepositorio<Dominio.Ticket> repoTickets, 
    IRepositorio<RegistroVisita> repoRegistros, IRepositorio<Incidencia> repoIncidencias) : IServicioAcceso
{
    public ValidarAccesoRespuesta ValidarAcceso(ValidarAccesoRequest request)
    {
        var ticket = repoTickets.Encontrar(d => d.Codigo == request.CodigoTicket);
        if(ticket == null)
        {
            return new ValidarAccesoRespuesta { AccesoPermitido = false, Mensaje = "El ticket no fue encontrado" };
        }

        var atraccion = repoAtracciones.Encontrar(y => y.Id == request.AtraccionId);
        if(atraccion == null)
        {
            return new ValidarAccesoRespuesta { AccesoPermitido = false, Mensaje = "Atracción no encontrada" };
        }

        if(ticket.FechaVisita.Date != DateTime.Today)
        {
            return new ValidarAccesoRespuesta
            {
                AccesoPermitido = false,
                Mensaje = $"Ticket válido solo para {ticket.FechaVisita:dd/MM/yyyy}"
            };
        }
        
        // ACA SE MANEJARIA LA SITUACION LIMITE DE ATRACCION EN EVENTO ESPECIAL
        if (ticket.TipoEntrada == TipoTicket.EventoEspecial)
        {
            return new ValidarAccesoRespuesta
            {
                AccesoPermitido = false,
                Mensaje = "Ticket de evento especial no válido para esta atracción",
                NombreAtraccion = atraccion.Nombre
            };
        }
        
        if (request.EdadVisitante < atraccion.EdadMinima)
        {
            return new ValidarAccesoRespuesta
            {
                AccesoPermitido = false,
                Mensaje = $"Edad mínima requerida: {atraccion.EdadMinima} años",
                NombreAtraccion = atraccion.Nombre
            };
        }

        var incidencias = repoIncidencias.Encontrar(d => d.AtraccionId == request.AtraccionId);
        if (incidencias != null && !incidencias.EstaDisponible())
        {
            return new ValidarAccesoRespuesta
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
            return new ValidarAccesoRespuesta
            {
                AccesoPermitido = false,
                Mensaje = $"Aforo completo ({atraccion.Capacidad} personas)",
                NombreAtraccion = atraccion.Nombre
            };
        }
        
        // ACCESO PERMITIDO, falta traer al usuario
        return new ValidarAccesoRespuesta
        {
            AccesoPermitido = true,
            Mensaje = "Acceso permitido",
            NombreAtraccion = atraccion.Nombre,
            NombreVisitante = "Visitante"  
        };
    }

    public void RegistrarIngreso(Guid codigoTicket, int atraccionId)
    {
        var ticket = repoTickets.Encontrar(t => t.Codigo == codigoTicket);
        if (ticket == null)
        {
            throw new ArgumentException("Ticket no encontrado");
        }

        var registro = new RegistroVisita
        {
            AtraccionId = atraccionId,
            Identificador = codigoTicket,
            FechaIngreso = DateTime.Today
        };

        repoRegistros.Agregar(registro);
    }
}
