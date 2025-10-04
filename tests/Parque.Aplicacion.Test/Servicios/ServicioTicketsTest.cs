using System.Linq.Expressions;
using Moq;
using Parque.Aplicacion.Servicios.Ticket;
using Parque.Dominio;
using Parque.Infraestructura.Repositorios;

namespace Parque.Aplicacion.Test.Servicios;

[TestClass]
public class ServicioTicketsTest
{
    private Mock<IRepositorio<Ticket>> _repositorioMock; // error
    private ServicioTicket _servicio; // error

    [TestInitialize]
    public void Setup()
    {
        _repositorioMock = new Mock<IRepositorio<Ticket>>();
        _servicio = new ServicioTicket(_repositorioMock.Object); // error
    }
    
    [TestMethod]
    public void CrearTicket_ConDatosValidos_DeberiaCrearTicket()
    {
        // Arrange
        var cuentaId = 1;
        var fechaVisita = DateTime.Now.AddDays(1);
        var eventoId = 1;

        // Act
        var ticket = _servicio.CrearTicket(cuentaId, fechaVisita, eventoId);

        // Assert
        Assert.IsNotNull(ticket);
        Assert.AreEqual(cuentaId, ticket.CuentaId); // error
        Assert.AreEqual(fechaVisita.Date, ticket.FechaVisita.Date); // error
        Assert.AreEqual(eventoId, ticket.EventoId); // error
        Assert.AreNotEqual(Guid.Empty, ticket.Codigo); 
        _repositorioMock.Verify(r => r.Agregar(It.IsAny<Ticket>()), Times.Once);
    }
}
