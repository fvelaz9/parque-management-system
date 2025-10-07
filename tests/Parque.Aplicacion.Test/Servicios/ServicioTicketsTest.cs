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
    public void CrearTicket_ConDatosValidos_DeberiaCrearTicket()
    {
        var cuentaId = new Guid("12345678-1234-1234-1234-123456789abc");
        var fechaVisita = DateTime.Now.AddDays(1);
        var eventoId = 1;
        var tipoticket = TipoTicket.General;
        var ticket = _servicio.CrearTicket(cuentaId, fechaVisita, eventoId, tipoticket);
        Assert.IsNotNull(ticket);
        Assert.AreEqual(cuentaId, ticket.CuentaId);
        Assert.AreEqual(fechaVisita.Date, ticket.FechaVisita.Date);
        Assert.AreEqual(eventoId, ticket.EventoId);
        Assert.AreNotEqual(Guid.Empty, ticket.Codigo);
        Assert.AreEqual(tipoticket, TipoTicket.General);
        _repositorioMock.Verify(r => r.Agregar(It.IsAny<Ticket>()), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CrearTicket_ConFechaIgualAhora_DeberiaLanzarExcepcion()
    {
        var fechaAhora = DateTime.Now;
        var cuentaId = new Guid("12345678-1234-1234-1234-123456789abc");

        _servicio.CrearTicket(cuentaId, fechaAhora, 1, TipoTicket.General);
    }

    [TestMethod]
    public void ListarTickets_DeberiaRetornarTodosLosTickets()
    {
        var cuentaId = new Guid("12345678-1234-1234-1234-123456789abc");
        var tickets = new List<Ticket>
        {
            new Ticket { Id = 1, CuentaId = cuentaId, EventoId = 1 },
            new Ticket { Id = 2, CuentaId = cuentaId, EventoId = 2 }
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
        var cuentaId = new Guid("12345678-1234-1234-1234-123456789abc");
        var ticketEsperado = new Ticket { Id = 1, CuentaId = cuentaId };
        _repositorioMock.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Ticket, bool>>>()))
            .Returns(ticketEsperado);

        var resultado = _servicio.BuscarTicket(1);

        Assert.IsNotNull(resultado);
        Assert.AreEqual(ticketEsperado.Id, resultado.Id);
        _repositorioMock.Verify(r => r.Encontrar(It.IsAny<Expression<Func<Ticket, bool>>>()), Times.Once);
    }

    [TestMethod]
    public void ModificarTicket_ConIdExistente_DeberiaModificarTicket()
    {
        var cuentaId = new Guid("12345678-1234-1234-1234-123456789abc");
        var ticketExistente = new Ticket { Id = 1, CuentaId = cuentaId, FechaVisita = DateTime.Now };
        _repositorioMock.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Ticket, bool>>>()))
            .Returns(ticketExistente);

        var cuentaId2 = new Guid("12345678-1234-1234-1234-123456789ab2c");
        var nuevaFechaVisita = DateTime.Now.AddDays(2);
        var nuevoEventoId = 2;
        var nuevoEventoTipo = TipoTicket.EventoEspecial;

        // Act
        _servicio.ModificarTicket(1, cuentaId2, nuevaFechaVisita, nuevoEventoId, nuevoEventoTipo);

        // Assert
        Assert.AreEqual(cuentaId2, ticketExistente.CuentaId);
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
    [ExpectedException(typeof(ArgumentException))]
    public void ComprarTicket_ConFechaPasada_DeberiaLanzarExcepcion()
    {
        var cuentaId = new Guid("12345678-1234-1234-1234-123456789abc");
        var fechaPasada = DateTime.Now.AddDays(-1);

        _servicio.CrearTicket(cuentaId, fechaPasada, null, TipoTicket.General);
    }

    [TestMethod]
    public void ComprarTicket_General_DeberiaCrearTicketSinEvento()
    {
        var fechaFutura = DateTime.Now.AddDays(5);
        var cuentaId = new Guid("12345678-1234-1234-1234-123456789abc");
        var ticket = _servicio.CrearTicket(cuentaId, fechaFutura, null, TipoTicket.General);

        Assert.IsNotNull(ticket);
        Assert.AreEqual(cuentaId, ticket.CuentaId);
        Assert.AreEqual(TipoTicket.General, ticket.TipoEntrada);
        Assert.IsNull(ticket.EventoId);
        Assert.AreNotEqual(Guid.Empty, ticket.Codigo);
        _repositorioMock.Verify(r => r.Agregar(It.IsAny<Dominio.Ticket>()), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void ComprarTicket_EventoEspecialSinEventoId_DeberiaLanzarExcepcion()
    {
        var fechaFutura = DateTime.Now.AddDays(5);
        var cuentaId = new Guid("12345678-1234-1234-1234-123456789abc");
        _servicio.CrearTicket(cuentaId, fechaFutura, null, TipoTicket.EventoEspecial);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void ComprarTicket_EventoNoEncontrado_DeberiaLanzarExcepcion()
    {
        var cuentaId = new Guid("12345678-1234-1234-1234-123456789abc");
        var fechaFutura = DateTime.Now.AddDays(5);
        _repositorioEventoMock.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Evento, bool>>>()))
            .Returns((Evento)null);

        _servicio.CrearTicket(cuentaId, fechaFutura, 999, TipoTicket.EventoEspecial);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void CrearTicket_AforoCompleto_DeberiaLanzarExcepcion()
    {
        var fechaFutura = DateTime.Now.AddDays(5);
        var cuentaId = new Guid("12345678-1234-1234-1234-123456789abc");
        var evento = new Evento("Concierto", "Descripción", DateTime.Now, DateTime.Now.AddDays(10), 2, 50, EstadoEvento.Programado);
        _repositorioEventoMock.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Evento, bool>>>()))
            .Returns(evento);
        var ticketsExistentes = new List<Ticket>
        {
            new Ticket { Id = 1, EventoId = 1, EsValido = true },
            new Ticket { Id = 2, EventoId = 1, EsValido = true }
        };
        _repositorioMock.Setup(r => r.ObtenerTodos()).Returns(ticketsExistentes);

        // Act
        _servicio.CrearTicket(cuentaId, fechaFutura, 1, TipoTicket.EventoEspecial);
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
