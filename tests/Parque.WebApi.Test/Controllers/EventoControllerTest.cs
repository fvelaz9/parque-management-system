using Moq;
using Parque.Aplicacion.Servicios;
using Parque.Dominio;
using Parque.Dominio.Atracciones;
using Parque.WebApi.Controllers.Evento;
using Parque.WebApi.Controllers.Evento.Modelos;

namespace Parque.WebApi.Test.Controllers;

[TestClass]
public class EventoControllerTest
{
    private Mock<IServicioEvento>? _servicioEventoMock;
    private EventoController? _controller;
    private List<AtraccionParque>? _atraccionesTest;
    [TestInitialize]
    public void Initialize()
    {
        _servicioEventoMock = new Mock<IServicioEvento>(MockBehavior.Strict);
        _controller = new EventoController(_servicioEventoMock.Object);
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
    [ExpectedException(typeof(Exception))]
    public void CrearConRequestNull()
    {
        _controller!.Crear(null!);
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
            Estado = EstadoEvento.Cancelado,
            Atracciones = _atraccionesTest!.ToList()
        };

        var expectedEvento = new Dominio.Evento(
            request.Titulo,
            request.Descripcion,
            request.Inicio,
            request.Fin,
            request.AforoMaximo,
            request.CostoAdicional,
            request.Estado);
        expectedEvento.Atracciones = _atraccionesTest!;

        _servicioEventoMock!.Setup(s => s.AgregarEvento(It.IsAny<Dominio.Evento>()))
            .Returns(expectedEvento);

        var response = _controller!.Crear(request);

        _servicioEventoMock.VerifyAll();
        Assert.IsNotNull(response);
        Assert.IsInstanceOfType(response, typeof(CreateEventoResponse));
    }

    [TestMethod]
    [ExpectedException(typeof(Exception))]
    public void CrearConTituloNull()
    {
        var request = new CreateEventoRequest
        {
            Titulo = string.Empty,
            AforoMaximo = 10
        };

        _controller!.Crear(request);
    }

    [TestMethod]
    [ExpectedException(typeof(Exception))]
    public void CrearConAforoMaximo0()
    {
        var request = new CreateEventoRequest
        {
            Titulo = "Test Event",
            AforoMaximo = 0
        };

        _controller!.Crear(request);
    }

    [TestMethod]
    [ExpectedException(typeof(Exception))]
    public void CrearAforoNegativo()
    {
        var request = new CreateEventoRequest
        {
            Titulo = "Test Event",
            AforoMaximo = -5
        };

        _controller!.Crear(request);
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
        Assert.AreEqual(2, result.Count);
        Assert.IsInstanceOfType(result, typeof(List<EventoOutDto>));
    }

    [TestMethod]
    public void ListaSinEventos()
    {
        var eventos = new List<Dominio.Evento>();

        _servicioEventoMock!.Setup(s => s.ListarEventos()).Returns(eventos);

        var result = _controller!.Listar();

        _servicioEventoMock.VerifyAll();
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public void EliminarEvento()
    {
        var eventoId = 1;
        var eventoExistente = new Dominio.Evento(
            "Evento a eliminar",
            "Descripción",
            DateTime.Now,
            DateTime.Now.AddHours(2),
            100,
            10,
            EstadoEvento.Programado);

        _servicioEventoMock!.Setup(s => s.ObtenerEventoPorId(eventoId))
            .Returns(eventoExistente);
        _servicioEventoMock.Setup(s => s.EliminarEventoPorId(eventoId));

        _controller!.Eliminar(eventoId);

        _servicioEventoMock.VerifyAll();
    }

    [TestMethod]
    [ExpectedException(typeof(Exception))]
    public void EliminarEventoNoExistente()
    {
        var eventoId = 999;

        _servicioEventoMock!.Setup(s => s.ObtenerEventoPorId(eventoId))
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
        Assert.IsInstanceOfType(result, typeof(EventoOutDto));
    }

    [TestMethod]
    [ExpectedException(typeof(Exception))]
    public void ObtenerPorIdNoExistente()
    {
        var eventoId = 999;
        Dominio.Evento? nullEvento = null;

        _servicioEventoMock!.Setup(s => s.ObtenerEventoPorId(eventoId))
            .Throws(new Exception($"No se encontró un evento con ID {eventoId}."));

        _controller!.ObtenerPorId(eventoId);
    }

    [TestMethod]
    [ExpectedException(typeof(Exception))]
    public void ActualizarNull()
    {
        var eventoId = 1;

        _controller!.Actualizar(eventoId, null!);
    }

    [TestMethod]
    [ExpectedException(typeof(Exception))]
    public void ActualizarNoExistente()
    {
        var eventoId = 999;
        var request = new UpdateEventoRequest
        {
            Titulo = "Usdfadale"
        };

        _servicioEventoMock!.Setup(s => s.ObtenerEventoPorId(eventoId))
                           .Throws(new Exception($"No se encontró un evento con ID {eventoId}."));

        _controller!.Actualizar(eventoId, request);
    }

    [TestMethod]
    public void ActualizarEvento()
    {
        var eventoId = 1;
        var request = new UpdateEventoRequest
        {
            Titulo = "hkslha",
            Descripcion = "adfads",
            Inicio = DateTime.Now.AddDays(1),
            Fin = DateTime.Now.AddDays(1).AddHours(3),
            AforoMaximo = 200,
            CostoAdicional = 25,
            Estado = EstadoEvento.Cancelado
        };

        var existingEvento = new Dominio.Evento(
            "adfs",
            "dfgs",
            DateTime.Now,
            DateTime.Now.AddHours(1),
            100,
            0,
            EstadoEvento.Programado);

        _servicioEventoMock!.Setup(s => s.ObtenerEventoPorId(eventoId))
                           .Returns(existingEvento);
        _servicioEventoMock.Setup(s => s.ActualizarEvento(It.IsAny<Dominio.Evento>()));

        _controller!.Actualizar(eventoId, request);

        _servicioEventoMock.VerifyAll();
        Assert.AreEqual(request.Titulo, existingEvento.Titulo);
        Assert.AreEqual(request.Descripcion, existingEvento.Descripcion);
        Assert.AreEqual(request.Inicio.Value, existingEvento.Inicio);
        Assert.AreEqual(request.Fin.Value, existingEvento.Fin);
        Assert.AreEqual(request.AforoMaximo.Value, existingEvento.AforoMaximo);
        Assert.AreEqual(request.CostoAdicional.Value, existingEvento.CostoAdicional);
        Assert.AreEqual(request.Estado.Value, existingEvento.Estado);
    }
}
