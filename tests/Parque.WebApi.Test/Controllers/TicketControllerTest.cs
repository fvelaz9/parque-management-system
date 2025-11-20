using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Parque.Aplicacion.DTOs;
using Parque.Aplicacion.Servicios.Ticket;
using Parque.Dominio;
using Parque.Dominio.Usuarios;
using Parque.WebApi.Controllers;

namespace Parque.WebApi.Test.Controllers;

[TestClass]
public class TicketControllerTest
{
    private Mock<IServicioTicket>? _servicioMock;
    private TicketController? _controller;
    private Cuenta? _usuarioAutenticado;
    private readonly DateTime _fechaActual = new(2025, 10, 8, 12, 0, 0);

    [TestInitialize]
    public void Initialize()
    {
        _servicioMock = new Mock<IServicioTicket>(MockBehavior.Strict);
        _controller = new TicketController(_servicioMock.Object);

        // Crear usuario autenticado
        _usuarioAutenticado = Cuenta.Crear("Juan", "Perez", new Email("juan@test.com"), "pass123", Rol.Visitante);
        _usuarioAutenticado.AsignarVisitante(new DateTime(2000, 1, 1));

        // Configurar HttpContext con usuario autenticado
        var httpContext = new DefaultHttpContext();
        httpContext.Items["user"] = _usuarioAutenticado;
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };
    }

    [TestMethod]
    public void GetByCodigo_ConTicketExistente_RetornaOk()
    {
        // Arrange
        var codigo = Guid.NewGuid();
        var fechaVisita = new DateTime(2025, 10, 10, 14, 0, 0);
        var ticket = new Dominio.Ticket(_usuarioAutenticado!.Id, fechaVisita, 1, TipoTicket.General, _fechaActual)
        {
            Codigo = codigo
        };

        _servicioMock!.Setup(s => s.BuscarTicketPorCodigo(codigo)).Returns(ticket);

        // Act
        var result = _controller!.GetByCodigo(codigo);

        // Assert
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(ticket, okResult.Value);
        _servicioMock.VerifyAll();
    }

    [TestMethod]
    public void GetByCodigo_ConTicketNoExistente_RetornaNotFound()
    {
        // Arrange
        var codigo = Guid.NewGuid();
        _servicioMock!.Setup(s => s.BuscarTicketPorCodigo(codigo)).Returns((Dominio.Ticket?)null);

        // Act
        var result = _controller!.GetByCodigo(codigo);

        // Assert
        Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
        _servicioMock.VerifyAll();
    }

    [TestMethod]
    public void Create_TicketGeneralValido_RetornaCreated()
    {
        // Arrange
        var fechaVisita = new DateTime(2025, 10, 10, 14, 0, 0);
        var request = new CrearTicketDto
        {
            FechaVisita = fechaVisita,
            TipoEntrada = TipoTicket.General
        };

        var expectedTicket = new Dominio.Ticket(_usuarioAutenticado!.Id, request.FechaVisita, 0, TipoTicket.General, _fechaActual);
        _servicioMock!.Setup(s => s.CrearTicket(_usuarioAutenticado.Id, request))
            .Returns(expectedTicket);

        // Act
        var result = _controller!.Create(request);

        // Assert
        var created = result as CreatedAtActionResult;
        Assert.IsNotNull(created);
        Assert.AreEqual(nameof(_controller.GetByCodigo), created.ActionName);
        Assert.AreEqual(expectedTicket, created.Value);
        _servicioMock.VerifyAll();
    }

    [TestMethod]
    public void Create_TicketEspecialSinEventoId_RetornaBadRequest()
    {
        // Arrange
        var fechaVisita = new DateTime(2025, 10, 9, 14, 0, 0);
        var request = new CrearTicketDto
        {
            FechaVisita = fechaVisita,
            TipoEntrada = TipoTicket.EventoEspecial,
            EventoId = null
        };

        // Act
        var result = _controller!.Create(request);

        // Assert
        Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
    }

    [TestMethod]
    public void Create_TicketEspecialValido_RetornaCreated()
    {
        // Arrange
        var fechaVisita = new DateTime(2025, 10, 11, 14, 0, 0);
        var request = new CrearTicketDto
        {
            FechaVisita = fechaVisita,
            TipoEntrada = TipoTicket.EventoEspecial,
            EventoId = 5
        };

        var expectedTicket = new Dominio.Ticket(_usuarioAutenticado!.Id, request.FechaVisita, 5, TipoTicket.EventoEspecial, _fechaActual);
        _servicioMock!.Setup(s => s.CrearTicket(_usuarioAutenticado.Id, request))
            .Returns(expectedTicket);

        // Act
        var result = _controller!.Create(request);

        // Assert
        var created = result as CreatedAtActionResult;
        Assert.IsNotNull(created);
        Assert.AreEqual(expectedTicket, created.Value);
        _servicioMock.VerifyAll();
    }

    [TestMethod]
    public void Create_ExcepcionDelServicio_NoManejadaPorController()
    {
        // Arrange
        var fechaVisita = new DateTime(2025, 10, 9, 14, 0, 0);
        var request = new CrearTicketDto
        {
            FechaVisita = fechaVisita,
            TipoEntrada = TipoTicket.General
        };

        _servicioMock!.Setup(s => s.CrearTicket(_usuarioAutenticado!.Id, request))
            .Throws(new ArgumentException("Fecha inválida"));

        // Act & Assert
        Assert.ThrowsException<ArgumentException>(() => _controller!.Create(request));
        _servicioMock.VerifyAll();
    }

    [TestMethod]
    public void ObtenerMisTickets_ConUsuarioAutenticado_RetornaTicketsDelUsuario()
    {
        // Arrange
        var fechaVisita1 = new DateTime(2025, 10, 10, 14, 0, 0);
        var fechaVisita2 = new DateTime(2025, 10, 11, 14, 0, 0);
        var otroUsuarioId = Guid.NewGuid();

        List<Dominio.Ticket> todosLosTickets =
        [
            new Dominio.Ticket(_usuarioAutenticado!.Id, fechaVisita1, 1, TipoTicket.General, _fechaActual),
            new Dominio.Ticket(_usuarioAutenticado.Id, fechaVisita2, 2, TipoTicket.General, _fechaActual),
            new Dominio.Ticket(otroUsuarioId, fechaVisita1, 3, TipoTicket.General, _fechaActual)
        ];

        _servicioMock!.Setup(s => s.ListarTickets()).Returns(todosLosTickets);

        // Act
        var result = _controller!.ObtenerMisTickets();

        // Assert
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);
        var returnedTickets = okResult.Value as IEnumerable<Dominio.Ticket>;
        Assert.IsNotNull(returnedTickets);
        Assert.AreEqual(2, returnedTickets.Count());
        Assert.IsTrue(returnedTickets.All(t => t.CuentaId == _usuarioAutenticado.Id));
        _servicioMock.VerifyAll();
    }

    [TestMethod]
    public void ObtenerMisTickets_SinUsuarioAutenticado_RetornaUnauthorized()
    {
        // Arrange
        _controller!.ControllerContext.HttpContext.Items["user"] = null;

        // Act
        var result = _controller.ObtenerMisTickets();

        // Assert
        Assert.IsInstanceOfType(result, typeof(UnauthorizedObjectResult));
    }

    [TestMethod]
    public void GetTicketsPorUsuario_UsuarioConTickets_RetornaOkConTickets()
    {
        // Arrange
        var usuarioId = Guid.NewGuid();
        var tickets = new List<Ticket>
        {
            new Ticket(usuarioId, DateTime.Today, 1, TipoTicket.General, DateTime.Now) { Codigo = Guid.NewGuid() },
            new Ticket(usuarioId, DateTime.Today.AddDays(1), 2, TipoTicket.EventoEspecial, DateTime.Now) { Codigo = Guid.NewGuid() }
        };

        _servicioMock!.Setup(s => s.ListarTicketsValidosGeneral(usuarioId)).Returns(tickets);

        // Act
        var result = _controller!.GetTicketsPorUsuario(usuarioId);

        // Assert
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        var value = okResult.Value as IEnumerable<Ticket>;
        Assert.IsNotNull(value);
        Assert.AreEqual(2, value.Count());
        Assert.IsTrue(value.All(t => t.CuentaId == usuarioId));
    }

    [TestMethod]
    public void GetTicketsPorUsuario_UsuarioSinTickets_RetornaOkConListaVacia()
    {
        var usuarioId = Guid.NewGuid();
        _servicioMock!.Setup(s => s.ListarTicketsValidosGeneral(usuarioId)).Returns([]);
        var result = _controller!.GetTicketsPorUsuario(usuarioId);
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        var value = okResult.Value as IEnumerable<Ticket>;
        Assert.IsNotNull(value);
        Assert.AreEqual(0, value.Count());
    }

    [TestMethod]
    public void GetTicketsPorUsuarioYEvento_ExistenTickets_RetornaOk()
    {
        var usuarioId = Guid.NewGuid();
        var eventoId = 42;
        var tickets = new List<Ticket>
        {
            new Ticket(usuarioId, DateTime.Today, eventoId, TipoTicket.EventoEspecial, DateTime.Now) { Codigo = Guid.NewGuid() }
        };
        _servicioMock!.Setup(s => s.ObtenerTicketsPorUsuarioYEvento(usuarioId, eventoId)).Returns(tickets);
        var result = _controller!.GetTicketsPorUsuarioYEvento(usuarioId, eventoId);
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        var value = okResult.Value as IEnumerable<Ticket>;
        Assert.IsNotNull(value);
        Assert.AreEqual(1, value.Count());
    }
}
