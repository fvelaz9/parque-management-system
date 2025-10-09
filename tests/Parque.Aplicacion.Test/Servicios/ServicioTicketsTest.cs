using System.Linq.Expressions;
using Moq;
using Parque.Aplicacion.Servicios;
using Parque.Aplicacion.Servicios.Ticket;
using Parque.Dominio;
using Parque.Dominio.Excepciones;
using Parque.Infraestructura.Repositorios;

namespace Parque.Aplicacion.Test.Servicios;

[TestClass]
public class ServicioTicketsTest
{
    private readonly Mock<IRepositorio<Ticket>> _repositorioMock;
    private readonly Mock<IRepositorio<Evento>> _repositorioEventoMock;
    private readonly Mock<IServicioFechaHora> _servicioFechaHoraMock;
    private readonly ServicioTicket _servicio;
    private readonly DateTime _fechaActual = new(2025, 10, 8, 12, 0, 0);

    public ServicioTicketsTest()
    {
        _repositorioMock = new Mock<IRepositorio<Ticket>>();
        _repositorioEventoMock = new Mock<IRepositorio<Evento>>();
        _servicioFechaHoraMock = new Mock<IServicioFechaHora>();
        _servicioFechaHoraMock.Setup(s => s.ObtenerFechaActual()).Returns(_fechaActual);
        _servicio = new ServicioTicket(
            _repositorioMock.Object,
            _repositorioEventoMock.Object,
            _servicioFechaHoraMock.Object);
    }

    [TestMethod]
    public void CrearTicketGeneralValido()
    {
        var cuentaId = new Guid("12345678-1234-1234-1234-123456789abc");
        var fechaVisita = new DateTime(2025, 10, 10, 14, 0, 0);

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
        var cuentaId = new Guid("12345678-1234-1234-1234-123456789abc");
        var fechaVisita = new DateTime(2025, 10, 7, 12, 0, 0); // Fecha anterior a _fechaActual

        _servicio.CrearTicketGeneral(cuentaId, fechaVisita);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CrearTicketEventoEspecialFechaInvalida()
    {
        var cuentaId = new Guid("12345678-1234-1234-1234-123456789abc");
        var fechaPasada = new DateTime(2025, 10, 7, 12, 0, 0); // Fecha anterior

        _servicio.CrearTicketEventoEspecial(cuentaId, fechaPasada, 1);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void CrearTicketEventoEspecialAforoCompleto()
    {
        var fechaVisita = new DateTime(2025, 10, 10, 14, 0, 0);
        var fechaInicioEvento = new DateTime(2025, 10, 8, 10, 0, 0);
        var fechaFinEvento = new DateTime(2025, 10, 18, 10, 0, 0);
        var evento = new Evento("Show", "Concierto", fechaInicioEvento, fechaFinEvento, 2, 50, EstadoEvento.Programado);
        var cuentaId = new Guid("12345678-1234-1234-1234-123456789abc");

        _repositorioEventoMock.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Evento, bool>>>()))
            .Returns(evento);

        var ticketsVendidos = new List<Ticket>
        {
            new Ticket { EventoId = 1, EsValido = true },
            new Ticket { EventoId = 1, EsValido = true }
        };
        _repositorioMock.Setup(r => r.ObtenerTodos()).Returns(ticketsVendidos);

        _servicio.CrearTicketEventoEspecial(cuentaId, fechaVisita, 1);
    }

    [TestMethod]
    [ExpectedException(typeof(ExcepcionEntidadNoEncontrada))]
    public void CrearTicketEventoEspecialEventoNoEncontrado()
    {
        var cuentaId = new Guid("12345678-1234-1234-1234-123456789abc");
        var fechaVisita = new DateTime(2025, 10, 13, 14, 0, 0);

        _repositorioEventoMock.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Evento, bool>>>()))
            .Returns(default(Evento));

        _servicio.CrearTicketEventoEspecial(cuentaId, fechaVisita, 999);
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
        var fechaActualTicket = new DateTime(2025, 10, 8, 10, 0, 0);
        var ticketExistente = new Ticket { Id = 1, CuentaId = cuentaId, FechaVisita = fechaActualTicket };
        _repositorioMock.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Ticket, bool>>>()))
            .Returns(ticketExistente);

        var fechaInicioEvento = new DateTime(2025, 10, 8, 10, 0, 0);
        var fechaFinEvento = new DateTime(2025, 10, 18, 10, 0, 0);
        var evento = new Evento("Show", "Concierto", fechaInicioEvento, fechaFinEvento, 100, 50, EstadoEvento.Programado) { Id = 2 };
        _repositorioEventoMock.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Evento, bool>>>()))
            .Returns(evento);

        _repositorioMock.Setup(r => r.ObtenerTodos()).Returns([]);

        var cuentaId2 = new Guid("12345678-1234-1234-1234-123456789abc");
        var nuevaFechaVisita = new DateTime(2025, 10, 10, 14, 0, 0);
        var nuevoEventoId = 2;
        var nuevoEventoTipo = TipoTicket.EventoEspecial;

        _servicio.ModificarTicket(1, cuentaId2, nuevaFechaVisita, nuevoEventoId, nuevoEventoTipo);

        Assert.AreEqual(cuentaId2, ticketExistente.CuentaId);
        Assert.AreEqual(nuevaFechaVisita, ticketExistente.FechaVisita);
        Assert.AreEqual(nuevoEventoId, ticketExistente.EventoId);
        Assert.IsNotNull(nuevoEventoTipo);
        _repositorioMock.Verify(r => r.Editar(ticketExistente), Times.Once);
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

    [TestMethod]
    public void CrearBorrarTicker()
    {
        var cuentaId = new Guid("12345678-1234-1234-1234-123456789abc");
        DateTime fechaVisita = DateTime.Now.AddDays(1);

        _repositorioMock.Setup(r => r.Agregar(It.IsAny<Dominio.Ticket>()));

        var ticket = _servicio.CrearTicketGeneral(cuentaId, fechaVisita);

        _repositorioMock.Verify(r => r.Agregar(It.IsAny<Dominio.Ticket>()), Times.Once);

        _repositorioMock.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Ticket, bool>>>()))
            .Returns(ticket);
        _repositorioMock.Setup(r => r.Eliminar(It.IsAny<Expression<Func<Ticket, bool>>>()));

        _servicio.EliminarTicket(ticket.Id);

        Assert.IsNotNull(ticket);
        _repositorioMock.Verify(r => r.Eliminar(It.IsAny<Expression<Func<Ticket, bool>>>()), Times.Once);
    }
}
