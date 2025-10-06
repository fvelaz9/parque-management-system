using System.Linq.Expressions;
using Moq;
using Parque.Aplicacion.Servicios.Ticket;
using Parque.Dominio;
using Parque.Infraestructura.Repositorios;

namespace Parque.Aplicacion.Test.Servicios;

[TestClass]
public class ServicioTicketsTest
{
    private readonly Mock<IRepositorio<Ticket>> _repositorioMock;
    private readonly Mock<IRepositorio<Evento>> _repositorioEventoMock;
    private readonly ServicioTicket _servicio;

    public ServicioTicketsTest()
    {
        _repositorioMock = new Mock<IRepositorio<Ticket>>();
        _repositorioEventoMock = new Mock<IRepositorio<Evento>>();
        _servicio = new ServicioTicket(_repositorioMock.Object, _repositorioEventoMock.Object);
    }

    [TestMethod]
    public void CrearTicketGeneralValido()
    {
        var cuentaId = 123;
        DateTime fechaVisita = DateTime.Now.AddDays(1);

        _repositorioMock.Setup(r => r.Agregar(It.IsAny<Dominio.Ticket>()));
        var ticket = _servicio.CrearTicketGeneral(cuentaId, fechaVisita);

        _repositorioMock.Verify(r => r.Agregar(It.IsAny<Dominio.Ticket>()), Times.Once);
        Assert.IsNotNull(ticket);
        Assert.AreEqual(TipoTicket.General, ticket.TipoEntrada);
        Assert.AreEqual(cuentaId, ticket.CuentaId);
        Assert.AreEqual(fechaVisita, ticket.FechaVisita);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CrearTicketGeneralConFechaInvalida()
    {
        var cuentaId = 123;
        DateTime fechaVisita = DateTime.Now.AddMinutes(-5);

        _servicio.CrearTicketGeneral(cuentaId, fechaVisita);
    }

    [TestMethod]
    public void ListarTickets_DeberiaRetornarTodosLosTickets()
    {
        // Arrange
        var tickets = new List<Ticket>
        {
            new Ticket { Id = 1, CuentaId = 1, EventoId = 1 },
            new Ticket { Id = 2, CuentaId = 2, EventoId = 2 }
        };
        _repositorioMock.Setup(r => r.ObtenerTodos()).Returns(tickets);

        // Act
        var resultado = _servicio.ListarTickets();

        // Assert
        Assert.AreEqual(2, resultado.Count());
        _repositorioMock.Verify(r => r.ObtenerTodos(), Times.Once);
    }

    [TestMethod]
    public void BuscarTicket_ConIdValido_DeberiaRetornarTicket()
    {
        // Arrange
        var ticketEsperado = new Ticket { Id = 1, CuentaId = 1 };
        _repositorioMock.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Ticket, bool>>>()))
            .Returns(ticketEsperado);

        // Act
        var resultado = _servicio.BuscarTicket(1);

        // Assert
        Assert.IsNotNull(resultado);
        Assert.AreEqual(ticketEsperado.Id, resultado.Id);
        _repositorioMock.Verify(r => r.Encontrar(It.IsAny<Expression<Func<Ticket, bool>>>()), Times.Once);
    }

    [TestMethod]
    public void ModificarTicket_ConIdExistente_DeberiaModificarTicket()
    {
        // Arrange
        var ticketExistente = new Ticket { Id = 1, CuentaId = 1, FechaVisita = DateTime.Now };
        _repositorioMock.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Ticket, bool>>>()))
            .Returns(ticketExistente);

        var nuevaCuentaId = 2;
        var nuevaFechaVisita = DateTime.Now.AddDays(2);
        var nuevoEventoId = 2;
        var nuevoEventoTipo = TipoTicket.EventoEspecial;

        // Act
        _servicio.ModificarTicket(1, nuevaCuentaId, nuevaFechaVisita, nuevoEventoId, nuevoEventoTipo);

        // Assert
        Assert.AreEqual(nuevaCuentaId, ticketExistente.CuentaId);
        Assert.AreEqual(nuevaFechaVisita, ticketExistente.FechaVisita);
        Assert.AreEqual(nuevoEventoId, ticketExistente.EventoId);
        Assert.IsNotNull(nuevoEventoTipo);
        _repositorioMock.Verify(r => r.Editar(ticketExistente), Times.Once);
    }

    [TestMethod]
    public void EliminarTicket_ConIdValido_DeberiaEliminarTicket()
    {
        var id = 1;
        _servicio.EliminarTicket(id);
        _repositorioMock.Verify(r => r.Eliminar(It.IsAny<Expression<Func<Ticket, bool>>>()), Times.Once);
    }

    [TestMethod]
    public void BuscarTicketPorCodigo_WhenTicketNotExists_ShouldReturnNull()
    {
        // Arrange
        var codigo = Guid.NewGuid();
        _repositorioMock.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Dominio.Ticket, bool>>>()))
            .Returns((Dominio.Ticket?)null);

        // Act
        var resultado = _servicio.BuscarTicketPorCodigo(codigo);

        // Assert
        Assert.IsNull(resultado);
        _repositorioMock.VerifyAll();
    }
}
