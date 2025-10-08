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
    private readonly DateTime _fechaActual = new(2025, 10, 8, 12, 0, 0);

    [TestInitialize]
    public void Initialize()
    {
        _servicioMock = new Mock<IServicioTicket>(MockBehavior.Strict);
        _controller = new TicketController(_servicioMock.Object);
    }

    [TestMethod]
    public void GetAllValido()
    {
        var cuentaId1 = Guid.NewGuid();
        var cuentaId2 = Guid.NewGuid();
        var fechaVisita1 = new DateTime(2025, 10, 10, 14, 0, 0);
        var fechaVisita2 = new DateTime(2025, 10, 11, 14, 0, 0);

        List<Dominio.Ticket> tickets =
        [
            new Dominio.Ticket(cuentaId1, fechaVisita1, 1, TipoTicket.General, _fechaActual),
            new Dominio.Ticket(cuentaId2, fechaVisita2, 2, TipoTicket.General, _fechaActual)
        ];

        _servicioMock!.Setup(s => s.ListarTickets()).Returns(tickets);

        var result = _controller!.GetAll();

        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);
        var returnedTickets = okResult.Value as IEnumerable<Dominio.Ticket>;
        Assert.AreEqual(2, returnedTickets!.Count());
        _servicioMock.VerifyAll();
    }

    [TestMethod]
    public void GetByIdconTicketExistente()
    {
        var cuentaId = Guid.NewGuid();
        var fechaVisita = new DateTime(2025, 10, 10, 14, 0, 0);
        var ticket = new Dominio.Ticket(cuentaId, fechaVisita, 1, TipoTicket.General, _fechaActual);
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
        _servicioMock!.Setup(s => s.BuscarTicket(1)).Returns((Dominio.Ticket?)null);

        var result = _controller!.GetById(1);

        Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        _servicioMock.VerifyAll();
    }

    [TestMethod]
    public void GetByCodigoConTicketExistente()
    {
        var cuentaId = Guid.NewGuid();
        var codigo = Guid.NewGuid();
        var fechaVisita = new DateTime(2025, 10, 10, 14, 0, 0);
        var ticket = new Dominio.Ticket(cuentaId, fechaVisita, 1, TipoTicket.General, _fechaActual)
        {
            Codigo = codigo
        };

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
        _servicioMock!.Setup(s => s.BuscarTicketPorCodigo(codigo)).Returns((Dominio.Ticket?)null);

        var result = _controller!.GetByCodigo(codigo);

        Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        _servicioMock.VerifyAll();
    }

    [TestMethod]
    public void CreateConRequestNull()
    {
        var result = _controller!.Create(null!);
        var badRequest = result as BadRequestObjectResult;
        Assert.IsNotNull(badRequest);
        Assert.AreEqual(400, badRequest.StatusCode);
    }

    [TestMethod]
    public void CreateGeneralTicketValido()
    {
        var cuentaId = Guid.NewGuid();
        var fechaVisita = new DateTime(2025, 10, 10, 14, 0, 0);
        var request = new CrearTicketDto
        {
            CuentaId = cuentaId,
            FechaVisita = fechaVisita,
            TipoEntrada = TipoTicket.General
        };

        var expectedTicket = new Dominio.Ticket(cuentaId, request.FechaVisita, 0, TipoTicket.General, _fechaActual);
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
        var cuentaId = Guid.NewGuid();
        var fechaVisita = new DateTime(2025, 10, 9, 14, 0, 0);
        var request = new CrearTicketDto
        {
            CuentaId = cuentaId,
            FechaVisita = fechaVisita,
            TipoEntrada = TipoTicket.EventoEspecial,
            EventoId = null
        };

        var result = _controller!.Create(request);
        Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
    }

    [TestMethod]
    public void CreateTicketEspecialValido()
    {
        var cuentaId = Guid.NewGuid();
        var fechaVisita = new DateTime(2025, 10, 11, 14, 0, 0);
        var request = new CrearTicketDto
        {
            CuentaId = cuentaId,
            FechaVisita = fechaVisita,
            TipoEntrada = TipoTicket.EventoEspecial,
            EventoId = 5
        };

        var expectedTicket = new Dominio.Ticket(cuentaId, request.FechaVisita, 5, TipoTicket.EventoEspecial, _fechaActual);
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
        var cuentaId = Guid.NewGuid();
        var fechaVisita = new DateTime(2025, 10, 9, 14, 0, 0);
        var request = new CrearTicketDto
        {
            CuentaId = cuentaId,
            FechaVisita = fechaVisita,
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
        var cuentaId = Guid.NewGuid();
        var fechaVisita = new DateTime(2025, 10, 9, 14, 0, 0);
        var request = new CrearTicketDto
        {
            CuentaId = cuentaId,
            FechaVisita = fechaVisita,
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
        var result = _controller!.Update(1, null!);
        var badRequest = result as BadRequestObjectResult;
        Assert.IsNotNull(badRequest);
        Assert.AreEqual(400, badRequest.StatusCode);
    }

    [TestMethod]
    public void UpdateConValidRequest()
    {
        var cuentaId = Guid.NewGuid();
        var fechaVisita = new DateTime(2025, 10, 11, 14, 0, 0);
        var request = new UpdateTicketDto
        {
            CuentaId = cuentaId,
            FechaVisita = fechaVisita,
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
        var cuentaId = Guid.NewGuid();
        var fechaVisita = new DateTime(2025, 10, 11, 14, 0, 0);
        var request = new UpdateTicketDto
        {
            CuentaId = cuentaId,
            FechaVisita = fechaVisita,
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
