using System.Linq.Expressions;
using Moq;
using Parque.Aplicacion.Servicios.Ticket;
using Parque.Dominio;
using Parque.Infraestructura.Repositorios;

namespace Parque.Aplicacion.Test.Servicios;

[TestClass]
public class ServicioTicketsTest
{
    private Mock<IRepositorio<Ticket>> _repositorioMock;
    private ServicioTicket _servicio;

    public ServicioTicketsTest()
    {
        _repositorioMock = new Mock<IRepositorio<Ticket>>();
        _servicio = new ServicioTicket(_repositorioMock.Object);
    }

    [TestMethod]
    public void CrearTicket_ConDatosValidos_DeberiaCrearTicket()
    {
        var cuentaId = 1;
        var fechaVisita = DateTime.Now.AddDays(1);
        var eventoId = 1;
        var ticket = _servicio.CrearTicket(cuentaId, fechaVisita, eventoId);
        Assert.IsNotNull(ticket);
        Assert.AreEqual(cuentaId, ticket.CuentaId);
        Assert.AreEqual(fechaVisita.Date, ticket.FechaVisita.Date);
        Assert.AreEqual(eventoId, ticket.EventoId);
        Assert.AreNotEqual(Guid.Empty, ticket.Codigo);
        _repositorioMock.Verify(r => r.Agregar(It.IsAny<Ticket>()), Times.Once);
    }
}
