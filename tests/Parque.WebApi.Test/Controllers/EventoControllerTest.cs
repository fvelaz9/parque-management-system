using Microsoft.AspNetCore.Mvc;
using Moq;
using Parque.Aplicacion.Servicios;
using Parque.Aplicacion.Servicios.Atracciones;
using Parque.Dominio;
using Parque.Dominio.Atracciones;
using Parque.WebApi.Controllers.Evento;
using Parque.WebApi.Controllers.Evento.Modelos;

namespace Parque.WebApi.Test.Controllers;

[TestClass]
public class EventoControllerTest
{
    private Mock<IServicioEvento>? _servicioEventoMock;
    private Mock<IServicioAtracciones>? _servicioAtraccionesMock;
    private EventoController? _controller;
    private List<AtraccionParque>? _atraccionesTest;

    [TestInitialize]
    public void Initialize()
    {
        _servicioEventoMock = new Mock<IServicioEvento>(MockBehavior.Strict);
        _servicioAtraccionesMock = new Mock<IServicioAtracciones>(MockBehavior.Strict);
        _controller = new EventoController(_servicioEventoMock.Object, _servicioAtraccionesMock.Object);
        _atraccionesTest =
        [
            new AtraccionParque(
                "Montaña Rusa Extrema",
                TipoAtraccion.MontañaRusa,
                12,
                50,
                "Una emocionante montaña rusa con caídas vertiginosas")
            { Id = 1 },
            new AtraccionParque(
                "Carrusel Familiar",
                TipoAtraccion.Espectaculo,
                3,
                80,
                "Un carrusel clásico para toda la familia")
            { Id = 2 },
            new AtraccionParque(
                "Simulador del Terror",
                TipoAtraccion.Simulador,
                16,
                30,
                "Un Simulador oscuro lleno de sustos y sorpresas")
            { Id = 3 }
        ];
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CrearConTituloNull()
    {
        var request = new CreateEventoRequest
        {
            Titulo = string.Empty,
            Descripcion = "Test",
            Inicio = DateTime.Now.AddDays(1),
            Fin = DateTime.Now.AddDays(1).AddHours(2),
            AforoMaximo = 10,
            CostoAdicional = 0,
            Estado = EstadoEvento.Programado,
            AtraccionIds = [1]
        };

        _servicioAtraccionesMock!.Setup(s => s.ObtenerPorIds(It.IsAny<List<int>>()))
            .Returns(_atraccionesTest!.Take(1));

        _servicioEventoMock!.Setup(s => s.AgregarEvento(It.IsAny<Dominio.Evento>()))
            .Throws(new ArgumentException("El título no puede estar vacío"));

        _controller!.Crear(request);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CrearConAforoMaximo0()
    {
        var request = new CreateEventoRequest
        {
            Titulo = "Test Event",
            Descripcion = "Test",
            Inicio = DateTime.Now.AddDays(1),
            Fin = DateTime.Now.AddDays(1).AddHours(2),
            AforoMaximo = 0,
            CostoAdicional = 0,
            Estado = EstadoEvento.Programado,
            AtraccionIds = [1]
        };

        _servicioAtraccionesMock!.Setup(s => s.ObtenerPorIds(It.IsAny<List<int>>()))
            .Returns(_atraccionesTest!.Take(1));

        _servicioEventoMock!.Setup(s => s.AgregarEvento(It.IsAny<Dominio.Evento>()))
            .Throws(new ArgumentException("El aforo máximo debe ser mayor que 0"));

        _controller!.Crear(request);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CrearAforoNegativo()
    {
        var request = new CreateEventoRequest
        {
            Titulo = "Test Event",
            Descripcion = "Test",
            Inicio = DateTime.Now.AddDays(1),
            Fin = DateTime.Now.AddDays(1).AddHours(2),
            AforoMaximo = -5,
            CostoAdicional = 0,
            Estado = EstadoEvento.Programado,
            AtraccionIds = [1]
        };

        _servicioAtraccionesMock!.Setup(s => s.ObtenerPorIds(It.IsAny<List<int>>()))
            .Returns(_atraccionesTest!.Take(1));

        _servicioEventoMock!.Setup(s => s.AgregarEvento(It.IsAny<Dominio.Evento>()))
            .Throws(new ArgumentException("El aforo máximo debe ser mayor que 0"));

        _controller!.Crear(request);
    }

    [TestMethod]
    public void CrearConRequestCorrecto()
    {
        var request = new CreateEventoRequest
        {
            Titulo = "Festival de Aventura",
            Descripcion = "Un evento especial con las mejores atracciones",
            Inicio = DateTime.Now.AddDays(7),
            Fin = DateTime.Now.AddDays(7).AddHours(6),
            AforoMaximo = 500,
            CostoAdicional = 75,
            Estado = EstadoEvento.Programado,
            AtraccionIds = [1, 2, 3]
        };

        var expectedEvento = new Dominio.Evento(
            request.Titulo,
            request.Descripcion,
            request.Inicio,
            request.Fin,
            request.AforoMaximo,
            request.CostoAdicional,
            request.Estado)
        { Id = 1 };
        expectedEvento.Atracciones = _atraccionesTest!;

        _servicioAtraccionesMock!.Setup(s => s.ObtenerPorIds(request.AtraccionIds))
            .Returns(_atraccionesTest!);

        _servicioEventoMock!.Setup(s => s.AgregarEvento(It.IsAny<Dominio.Evento>()))
            .Returns(expectedEvento);

        var result = _controller!.Crear(request);

        _servicioEventoMock.VerifyAll();
        _servicioAtraccionesMock.VerifyAll();
        Assert.IsNotNull(result);
        var createdResult = result as CreatedAtActionResult;
        Assert.IsNotNull(createdResult);
        Assert.AreEqual(201, createdResult.StatusCode);
        Assert.IsInstanceOfType(createdResult.Value, typeof(EventoOutDto));
    }

    [TestMethod]
    public void ListarEventos()
    {
        var eventos = new List<Dominio.Evento>
        {
            new Dominio.Evento("Event 1", "Dojpug", DateTime.Now, DateTime.Now.AddHours(1), 50, 0, EstadoEvento.Programado),
            new Dominio.Evento("Event 2", "ghuogpi", DateTime.Now, DateTime.Now.AddHours(2), 100, 10, EstadoEvento.Cancelado)
        };

        _servicioEventoMock!.Setup(s => s.ListarEventos()).Returns(eventos);

        var result = _controller!.Listar();

        _servicioEventoMock.VerifyAll();
        Assert.IsNotNull(result);
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        var eventosDto = okResult.Value as List<EventoOutDto>;
        Assert.IsNotNull(eventosDto);
        Assert.AreEqual(2, eventosDto.Count);
    }

    [TestMethod]
    public void ListaSinEventos()
    {
        var eventos = new List<Dominio.Evento>();

        _servicioEventoMock!.Setup(s => s.ListarEventos()).Returns(eventos);

        var result = _controller!.Listar();

        _servicioEventoMock.VerifyAll();
        Assert.IsNotNull(result);
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        var eventosDto = okResult.Value as List<EventoOutDto>;
        Assert.IsNotNull(eventosDto);
        Assert.AreEqual(0, eventosDto.Count);
    }

    [TestMethod]
    public void EliminarEvento()
    {
        var eventoId = 1;

        _servicioEventoMock!.Setup(s => s.EliminarEventoPorId(eventoId));

        var result = _controller!.Eliminar(eventoId);

        _servicioEventoMock.VerifyAll();
        Assert.IsNotNull(result);
        var noContentResult = result as NoContentResult;
        Assert.IsNotNull(noContentResult);
        Assert.AreEqual(204, noContentResult.StatusCode);
    }

    [TestMethod]
    [ExpectedException(typeof(Exception))]
    public void EliminarEventoNoExistente()
    {
        var eventoId = 999;

        _servicioEventoMock!.Setup(s => s.EliminarEventoPorId(eventoId))
            .Throws(new Exception($"No se encontró un evento con ID {eventoId}."));

        _controller!.Eliminar(eventoId);
    }

    [TestMethod]
    public void ObtenerPorIdEvento()
    {
        var eventoId = 1;
        var expectedEvento = new Dominio.Evento("Evento", "dsfsa",
            DateTime.Now, DateTime.Now.AddHours(1), 100, 0, EstadoEvento.Programado);

        _servicioEventoMock!.Setup(s => s.ObtenerEventoPorId(eventoId)).Returns(expectedEvento);

        var result = _controller!.ObtenerPorId(eventoId);

        _servicioEventoMock.VerifyAll();
        Assert.IsNotNull(result);
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.IsInstanceOfType(okResult.Value, typeof(EventoOutDto));
    }

    [TestMethod]
    [ExpectedException(typeof(Exception))]
    public void ObtenerPorIdNoExistente()
    {
        var eventoId = 999;

        _servicioEventoMock!.Setup(s => s.ObtenerEventoPorId(eventoId))
            .Throws(new Exception($"No se encontró un evento con ID {eventoId}."));

        _controller!.ObtenerPorId(eventoId);
    }
}
