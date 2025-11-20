using System.Linq.Expressions;
using Moq;
using Parque.Aplicacion.DTOs;
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

    [TestMethod]
    public void CrearTicketEventoEspecial_Valido_RetornaTicket()
    {
        var cuentaId = Guid.NewGuid();
        var fechaVisita = new DateTime(2025, 10, 15, 10, 0, 0);
        var eventoId = 111;

        var evento = new Evento("Fiesta", "Gran evento", fechaVisita.AddDays(-1), fechaVisita.AddDays(2), 100, 30, EstadoEvento.Programado)
        { Id = eventoId };

        _repositorioEventoMock.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Evento, bool>>>()))
            .Returns(evento);
        _repositorioMock.Setup(r => r.ObtenerTodos()).Returns(new List<Ticket>());
        _repositorioMock.Setup(r => r.Agregar(It.IsAny<Ticket>()));

        var ticket = _servicio.CrearTicketEventoEspecial(cuentaId, fechaVisita, eventoId);

        Assert.IsNotNull(ticket);
        Assert.AreEqual(TipoTicket.EventoEspecial, ticket.TipoEntrada);
        Assert.AreEqual(cuentaId, ticket.CuentaId);
        Assert.AreEqual(eventoId, ticket.EventoId);
        _repositorioMock.Verify(r => r.Agregar(It.IsAny<Ticket>()), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(ExcepcionEntidadNoEncontrada))]
    public void CrearTicketEventoEspecial_EventoNoExiste_LanzaExcepcion()
    {
        var cuentaId = Guid.NewGuid();
        var fechaVisita = new DateTime(2025, 10, 16, 12, 0, 0);
        var eventoId = 999;

        _repositorioEventoMock.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Evento, bool>>>()))
            .Returns((Evento?)null);

        _servicio.CrearTicketEventoEspecial(cuentaId, fechaVisita, eventoId);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void CrearTicketEventoEspecial_AforoCompleto_LanzaExcepcion()
    {
        var cuentaId = Guid.NewGuid();
        var fechaVisita = new DateTime(2025, 10, 15, 10, 0, 0);
        var eventoId = 222;

        var evento = new Evento("Fiesta", "Gran evento", fechaVisita.AddDays(-2), fechaVisita.AddDays(2), 1, 30, EstadoEvento.Programado)
        { Id = eventoId };

        _repositorioEventoMock.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Evento, bool>>>()))
            .Returns(evento);

        _repositorioMock.Setup(r => r.ObtenerTodos()).Returns(new List<Ticket>
        {
            new Ticket { EventoId = eventoId, EsValido = true }
        });

        _servicio.CrearTicketEventoEspecial(cuentaId, fechaVisita, eventoId);
    }

    [TestMethod]
public void CrearTicket_ConTipoGeneral_RetornaTicketGeneral()
{
    var cuentaId = Guid.NewGuid();
    var ticketDto = new CrearTicketDto
    {
        FechaVisita = _fechaActual.AddDays(2),
        TipoEntrada = TipoTicket.General
    };
    _repositorioMock.Setup(r => r.Agregar(It.IsAny<Ticket>()));

    var ticket = _servicio.CrearTicket(cuentaId, ticketDto);

    Assert.IsNotNull(ticket);
    Assert.AreEqual(TipoTicket.General, ticket.TipoEntrada);
    _repositorioMock.Verify(r => r.Agregar(It.IsAny<Ticket>()), Times.Once);
}

[TestMethod]
public void CrearTicket_ConTipoEventoEspecial_RetornaTicketEventoEspecial()
{
    var cuentaId = Guid.NewGuid();
    var eventoId = 15;
    var ticketDto = new CrearTicketDto
    {
        FechaVisita = _fechaActual.AddDays(3),
        TipoEntrada = TipoTicket.EventoEspecial,
        EventoId = eventoId
    };

    var evento = new Evento("Fiesta", "Evento", _fechaActual, _fechaActual.AddDays(10), 100, 30, EstadoEvento.Programado)
    { Id = eventoId };

    _repositorioEventoMock.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Evento, bool>>>()))
        .Returns(evento);
    _repositorioMock.Setup(r => r.ObtenerTodos()).Returns(new List<Ticket>());
    _repositorioMock.Setup(r => r.Agregar(It.IsAny<Ticket>()));

    var ticket = _servicio.CrearTicket(cuentaId, ticketDto);

    Assert.IsNotNull(ticket);
    Assert.AreEqual(TipoTicket.EventoEspecial, ticket.TipoEntrada);
    Assert.AreEqual(eventoId, ticket.EventoId);
}

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CrearTicket_EventoEspecialSinEventoId_LanzaArgumentException()
    {
        var cuentaId = Guid.NewGuid();
        var ticketDto = new CrearTicketDto
        {
            FechaVisita = _fechaActual.AddDays(5),
            TipoEntrada = TipoTicket.EventoEspecial,
            EventoId = null
        };

        _servicio.CrearTicket(cuentaId, ticketDto);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CrearTicket_TipoNoValido_LanzaArgumentException()
    {
        var cuentaId = Guid.NewGuid();
        var ticketDto = new CrearTicketDto
        {
            FechaVisita = _fechaActual.AddDays(5),
            TipoEntrada = (TipoTicket)999,
        };

        _servicio.CrearTicket(cuentaId, ticketDto);
    }

    [TestMethod]
    public void ObtenerTicketsPorUsuarioYEvento_Valido_RetornaListaTickets()
    {
        var usuarioId = Guid.NewGuid();
        var eventoId = 7;
        var evento = new Evento("Evento", "Evento test", _fechaActual, _fechaActual.AddDays(1), 50, 10, EstadoEvento.Programado) { Id = eventoId };

        _repositorioEventoMock.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Evento, bool>>>()))
            .Returns(evento);

        var tickets = new List<Ticket>
        {
            new Ticket { CuentaId = usuarioId, EventoId = eventoId, EsValido = true, TipoEntrada = TipoTicket.EventoEspecial, FechaVisita = _fechaActual.AddDays(1) }
        };

        _repositorioMock.Setup(r => r.ObtenerTodos()).Returns(tickets);

        var result = _servicio.ObtenerTicketsPorUsuarioYEvento(usuarioId, eventoId);

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual(usuarioId, result.First().CuentaId);
        Assert.AreEqual(eventoId, result.First().EventoId);
    }

    [TestMethod]
    [ExpectedException(typeof(ExcepcionEntidadNoEncontrada))]
    public void ObtenerTicketsPorUsuarioYEvento_EventoNoExiste_LanzaExcepcion()
    {
        var usuarioId = Guid.NewGuid();
        var eventoId = 88;

        _repositorioEventoMock.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Evento, bool>>>()))
            .Returns((Evento?)null);

        _servicio.ObtenerTicketsPorUsuarioYEvento(usuarioId, eventoId);
    }

    [TestMethod]
    public void ListarTicketsValidosGeneral_DevuelveSoloTicketsValidosGeneral()
    {
        var usuarioId = Guid.NewGuid();
        var tickets = new List<Ticket>
        {
            new Ticket { CuentaId = usuarioId, EsValido = true, TipoEntrada = TipoTicket.General, FechaVisita = _fechaActual.AddDays(2) },
            new Ticket { CuentaId = usuarioId, EsValido = false, TipoEntrada = TipoTicket.General, FechaVisita = _fechaActual.AddDays(3) },
            new Ticket { CuentaId = usuarioId, EsValido = true, TipoEntrada = TipoTicket.EventoEspecial, FechaVisita = _fechaActual.AddDays(1) },
            new Ticket { CuentaId = usuarioId, EsValido = true, TipoEntrada = TipoTicket.General, FechaVisita = _fechaActual.AddDays(-1) }
        };
        _repositorioMock.Setup(r => r.ObtenerTodos()).Returns(tickets);

        var result = _servicio.ListarTicketsValidosGeneral(usuarioId);

        Assert.AreEqual(1, result.Count); // Solo el primero cumple toda la condición
        Assert.AreEqual(TipoTicket.General, result.First().TipoEntrada);
        Assert.IsTrue(result.First().EsValido);
        Assert.AreEqual(usuarioId, result.First().CuentaId);
    }
}
