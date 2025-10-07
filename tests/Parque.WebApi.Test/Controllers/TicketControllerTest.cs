using Microsoft.AspNetCore.Mvc;
using Moq;
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
        List<Ticket> tickets = new List<Ticket>
        {
            new Ticket { Id = 1, CuentaId = 1 },
            new Ticket { Id = 2, CuentaId = 2 }
        };

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
        Ticket ticket = new Ticket { Codigo = codigo };

        _servicioMock!.Setup(s => s.BuscarTicketPorCodigo(codigo)).Returns(ticket);

        var result = _controller!.GetByCodigo(codigo);

        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(ticket, okResult.Value);
        _servicioMock.VerifyAll();
    }
}
