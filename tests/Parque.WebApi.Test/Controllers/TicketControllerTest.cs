using Microsoft.AspNetCore.Mvc;
using Moq;
using Parque.Aplicacion.DTOS;
using Parque.Aplicacion.Servicios.Ticket;
using Parque.Dominio;
using Parque.WebApi.Controllers;

namespace Parque.WebApi.Test.Controllers;
[TestClass]
public class TicketControllerTest
{
    private Mock<IServicioTicket>? _servicioMock;
    private TicketController? _controller;

    [TestInitialize]
    public void Initialize()
    {
        _servicioMock = new Mock<IServicioTicket>(MockBehavior.Strict);
        _controller = new TicketController(_servicioMock.Object);
    }

    [TestMethod]
    public void GetAllValido()
    {
        List<Ticket> tickets =
        [
            new Ticket { Id = 1, CuentaId = 1 },
            new Ticket { Id = 2, CuentaId = 2 }
        ];

        _servicioMock!.Setup(s => s.ListarTickets()).Returns(tickets);

        var result = _controller!.GetAll();

        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);
        var returnedTickets = okResult.Value as IEnumerable<Ticket>;
        Assert.AreEqual(2, returnedTickets!.Count());
        _servicioMock.VerifyAll();
    }

    [TestMethod]
    public void GetByIdconTicketExistente()
    {
        var ticket = new Ticket { Id = 1 };
        _servicioMock!.Setup(s => s.BuscarTicket(1)).Returns(ticket);

        var result = _controller!.GetById(1);

        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(ticket, okResult.Value);
        _servicioMock.VerifyAll();
    }

    [TestMethod]
    public void GetByIdConTicketDoesNoExistente()
    {
        _servicioMock!.Setup(s => s.BuscarTicket(1)).Returns((Ticket)null);

        var result = _controller!.GetById(1);

        Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        _servicioMock.VerifyAll();
    }

    [TestMethod]
    public void GetByCodigoConTicketExistente()
    {
        var codigo = Guid.NewGuid();
        var ticket = new Ticket { Codigo = codigo };

        _servicioMock!.Setup(s => s.BuscarTicketPorCodigo(codigo)).Returns(ticket);

        var result = _controller!.GetByCodigo(codigo);

        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(ticket, okResult.Value);
        _servicioMock.VerifyAll();
    }

    [TestMethod]
    public void GetByCodigoConTicketNotExistente()
    {
        var codigo = Guid.NewGuid();
        _servicioMock!.Setup(s => s.BuscarTicketPorCodigo(codigo)).Returns((Ticket)null);

        var result = _controller!.GetByCodigo(codigo);

        Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        _servicioMock.VerifyAll();
    }

    [TestMethod]
    public void CreateConRequestNull()
    {
        var result = _controller!.Create(null);
        var badRequest = result as BadRequestObjectResult;
        Assert.IsNotNull(badRequest);
        Assert.AreEqual(400, badRequest.StatusCode);
    }

    [TestMethod]
    public void CreateGeneralTicketValido()
    {
        var request = new CrearTicketDto
        {
            CuentaId = 1,
            FechaVisita = DateTime.Now.AddDays(2),
            TipoEntrada = TipoTicket.General
        };

        var expectedTicket = new Ticket { Id = 10, CuentaId = 1 };
        _servicioMock!.Setup(s => s.CrearTicketGeneral(request.CuentaId, request.FechaVisita))
            .Returns(expectedTicket);

        var result = _controller!.Create(request);

        var created = result as CreatedAtActionResult;
        Assert.IsNotNull(created);
        Assert.AreEqual(nameof(_controller.GetById), created.ActionName);
        Assert.AreEqual(expectedTicket, created.Value);
        _servicioMock.VerifyAll();
    }

    [TestMethod]
    public void CreateConTicketEspecialNoValido()
    {
        var request = new CrearTicketDto
        {
            CuentaId = 1,
            FechaVisita = DateTime.Now.AddDays(1),
            TipoEntrada = TipoTicket.EventoEspecial,
            EventoId = null
        };

        var result = _controller!.Create(request);
        Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
    }

    [TestMethod]
    public void CreateTicketEspecialValido()
    {
        var request = new CrearTicketDto
        {
            CuentaId = 2,
            FechaVisita = DateTime.Now.AddDays(3),
            TipoEntrada = TipoTicket.EventoEspecial,
            EventoId = 5
        };

        var expectedTicket = new Ticket { Id = 20, CuentaId = 2, EventoId = 5 };
        _servicioMock!.Setup(s => s.CrearTicketEventoEspecial(request.CuentaId, request.FechaVisita, request.EventoId.Value))
            .Returns(expectedTicket);

        var result = _controller!.Create(request);

        var created = result as CreatedAtActionResult;
        Assert.IsNotNull(created);
        Assert.AreEqual(expectedTicket, created.Value);
        _servicioMock.VerifyAll();
    }

    [TestMethod]
    public void CreateConExcepcionDelServicio()
    {
        var request = new CrearTicketDto
        {
            CuentaId = 1,
            FechaVisita = DateTime.Now.AddDays(1),
            TipoEntrada = TipoTicket.General
        };

        _servicioMock!.Setup(s => s.CrearTicketGeneral(request.CuentaId, request.FechaVisita))
            .Throws(new ArgumentException("Fecha inválida"));

        var result = _controller!.Create(request);

        var badRequest = result as BadRequestObjectResult;
        Assert.IsNotNull(badRequest);
        Assert.AreEqual(400, badRequest.StatusCode);
        _servicioMock.VerifyAll();
    }

    [TestMethod]
    public void CreateConServiceThrowsInvalidOperationException()
    {
        var request = new CrearTicketDto
        {
            CuentaId = 1,
            FechaVisita = DateTime.Now.AddDays(1),
            TipoEntrada = TipoTicket.EventoEspecial,
            EventoId = 3
        };

        _servicioMock!.Setup(s => s.CrearTicketEventoEspecial(request.CuentaId, request.FechaVisita, request.EventoId.Value))
            .Throws(new InvalidOperationException("Aforo completo"));

        var result = _controller!.Create(request);

        var conflict = result as ConflictObjectResult;
        Assert.IsNotNull(conflict);
        Assert.AreEqual(409, conflict.StatusCode);
        _servicioMock.VerifyAll();
    }

    [TestMethod]
    public void UpdateConRequestNull()
    {
        var result = _controller!.Update(1, null);
        var badRequest = result as BadRequestObjectResult;
        Assert.IsNotNull(badRequest);
        Assert.AreEqual(400, badRequest.StatusCode);
    }

    [TestMethod]
    public void UpdateConValidRequest()
    {
        var request = new UpdateTicketDto
        {
            CuentaId = 1,
            FechaVisita = DateTime.Now.AddDays(3),
            EventoId = 5,
            TipoEntrada = TipoTicket.General
        };

        _servicioMock!.Setup(s => s.ModificarTicket(
            1,
            request.CuentaId,
            request.FechaVisita,
            request.EventoId,
            request.TipoEntrada));

        var result = _controller!.Update(1, request);

        Assert.IsInstanceOfType(result, typeof(NoContentResult));
        _servicioMock.VerifyAll();
    }

    [TestMethod]
    public void UpdateExcepcionDelServicio()
    {
        var request = new UpdateTicketDto
        {
            CuentaId = 1,
            FechaVisita = DateTime.Now.AddDays(3),
            EventoId = 5,
            TipoEntrada = TipoTicket.General
        };

        _servicioMock!.Setup(s => s.ModificarTicket(
                1,
                request.CuentaId,
                request.FechaVisita,
                request.EventoId,
                request.TipoEntrada))
            .Throws(new ArgumentException("Ticket no encontrado"));

        var result = _controller!.Update(1, request);

        var badRequest = result as BadRequestObjectResult;
        Assert.IsNotNull(badRequest);
        Assert.AreEqual(400, badRequest.StatusCode);
        _servicioMock.VerifyAll();
    }

    [TestMethod]
    public void DeleteValido()
    {
        _servicioMock!.Setup(s => s.EliminarTicket(1));

        var result = _controller!.Delete(1);

        Assert.IsInstanceOfType(result, typeof(NoContentResult));
        _servicioMock.VerifyAll();
    }
}
