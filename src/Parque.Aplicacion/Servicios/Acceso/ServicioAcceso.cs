using Parque.Aplicacion.DTOS;
using Parque.Aplicacion.Servicios.Incidencias;
using Parque.Dominio;
using Parque.Dominio.Atracciones;
using Parque.Dominio.Usuarios;
using Parque.Infraestructura.Repositorios;

namespace Parque.Aplicacion.Servicios.Acceso;

public class ServicioAcceso(IRepositorio<AtraccionParque> repoAtracciones, IRepositorio<Dominio.Ticket> repoTickets,
    IRepositorio<RegistroVisita> repoRegistros, IRepositorio<Incidencia> repoIncidencias) : IServicioAcceso
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

        // ACA SE MANEJARIA LA SITUACION LIMITE DE ATRACCION EN EVENTO ESPECIAL
        if (ticket.TipoEntrada == TipoTicket.EventoEspecial)
        {
            return new ValidarAccesoResponse
            {
                AccesoPermitido = false,
                Mensaje = "Ticket de evento especial no válido para esta atracción",
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

        // ACCESO PERMITIDO, falta traer al usuario
        return new ValidarAccesoResponse
        {
            AccesoPermitido = true,
            Mensaje = "Acceso permitido",
            NombreAtraccion = atraccion.Nombre,
            NombreVisitante = "Visitante"
        };
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
