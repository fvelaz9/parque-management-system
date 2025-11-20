using System.Linq.Expressions;
using Moq;
using Parque.Aplicacion.Servicios;
using Parque.Dominio;
using Parque.Dominio.Atracciones;
using Parque.Dominio.Excepciones;
using Parque.Infraestructura.Repositorios;

namespace Parque.Aplicacion.Test.Servicios;

[TestClass]
public class ServicioEventoTest
{
    private Mock<IRepositorio<Evento>>? _mockRepositorio;
    private ServicioEvento? _servicioEvento;

    [TestInitialize]
    public void Inicializar()
    {
        _mockRepositorio = new Mock<IRepositorio<Evento>>();
        _servicioEvento = new ServicioEvento(_mockRepositorio.Object);
    }

    [TestMethod]
    public void AgregarEventoValido()
    {
        var evento = new Evento(
            titulo: "Noche de Dinosaurios",
            descripcion: "Evento temático jurásico",
            inicio: DateTime.Today.AddDays(3).AddHours(20),
            fin: DateTime.Today.AddDays(3).AddHours(23),
            aforoMaximo: 500,
            costoAdicional: 50f,
            estado: EstadoEvento.Cancelado);

        _mockRepositorio!.Setup(r => r.Agregar(It.IsAny<Evento>())).Verifiable();

        var resultado = _servicioEvento!.AgregarEvento(evento);

        Assert.IsNotNull(resultado);
        Assert.AreEqual(evento.Titulo, resultado.Titulo);
        _mockRepositorio.Verify(r => r.Agregar(evento), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void AgregarEventoFechaFinMenorAInicio()
    {
        var evento = new Evento(
            titulo: "Evento inválido",
            descripcion: "El fin es antes del inicio",
            inicio: DateTime.Today.AddDays(3).AddHours(20),
            fin: DateTime.Today.AddDays(3).AddHours(18),
            aforoMaximo: 300,
            costoAdicional: 25f,
            estado: EstadoEvento.Cancelado);

        _servicioEvento!.AgregarEvento(evento);
    }

    [TestMethod]
    public void EliminarEventoPorId()
    {
        var evento = new Evento("Noche", "Temático", DateTime.Now, DateTime.Now.AddHours(2), 100, 50,
            EstadoEvento.Programado)
        { Id = 1 };

        var mockRepo = new Mock<IRepositorio<Evento>>();
        mockRepo.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Evento, bool>>>())).Returns(evento);
        mockRepo.Setup(r => r.Eliminar(It.IsAny<Expression<Func<Evento, bool>>>()));

        var servicio = new ServicioEvento(mockRepo.Object);

        servicio.EliminarEventoPorId(1);

        mockRepo.Verify(r => r.Eliminar(It.IsAny<Expression<Func<Evento, bool>>>()), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void ObtenerEventoPorIdInvalido()
    {
        _servicioEvento!.ObtenerEventoPorId(0);
    }

    [TestMethod]
    [ExpectedException(typeof(ExcepcionEntidadNoEncontrada))]
    public void EliminarEventoPorIdNoExistente()
    {
        var mockRepo = new Mock<IRepositorio<Evento>>();
        mockRepo.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Evento, bool>>>())).Returns((Evento)null!);

        var servicio = new ServicioEvento(mockRepo.Object);

        servicio.EliminarEventoPorId(10);
    }

    [TestMethod]
    public void ObtenerEventoPorId()
    {
        var evento = new Evento("asada", "Dagdasjh", DateTime.Now, DateTime.Now.AddHours(2), 100, 50,
            EstadoEvento.Programado)
        { Id = 1 };
        _mockRepositorio!.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Evento, bool>>>())).Returns(evento);

        Evento resultado = _servicioEvento!.ObtenerEventoPorId(1);

        Assert.IsNotNull(resultado);
        Assert.AreEqual(evento.Id, resultado.Id);
    }

    [TestMethod]
    public void ListarEventos()
    {
        var eventos = new List<Evento>
        {
            new Evento("Evento 1", "addads", DateTime.Now, DateTime.Now.AddHours(1), 50, 10,
                EstadoEvento.Programado),
            new Evento("Evento 2", "sdadaas", DateTime.Now, DateTime.Now.AddHours(2), 100, 20,
                EstadoEvento.Cancelado)
        };

        _mockRepositorio!.Setup(r => r.ObtenerTodos()).Returns(eventos);

        List<Evento> resultado = _servicioEvento!.ListarEventos();

        Assert.AreEqual(2, resultado.Count);
        Assert.AreEqual("Evento 1", resultado[0].Titulo);
    }

    [TestMethod]
    public void ActualizarEvento()
    {
        var evento = new Evento("Titulo", "Desc", DateTime.Now, DateTime.Now.AddHours(2), 100, 50,
            EstadoEvento.Programado)
        { Id = 1 };

        _mockRepositorio!.Setup(r => r.Editar(It.IsAny<Evento>())).Verifiable();

        _servicioEvento!.ActualizarEvento(evento);

        _mockRepositorio.Verify(r => r.Editar(evento), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void ActualizarEventoIdInvalido()
    {
        var evento = new Evento("Titulo", "Desc", DateTime.Now, DateTime.Now.AddHours(2), 100, 50,
            EstadoEvento.Programado)
        { Id = 0 };
        _servicioEvento!.ActualizarEvento(evento);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void ActualizarEventoFechaInvalida()
    {
        var evento = new Evento("Titulo", "Desc", DateTime.Now.AddHours(2), DateTime.Now, 100, 50,
            EstadoEvento.Programado)
        { Id = 1 };
        _servicioEvento!.ActualizarEvento(evento);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void ActualizarEventoAforoInvalido()
    {
        var evento =
            new Evento("Titulo", "Desc", DateTime.Now, DateTime.Now.AddHours(2), 0, 50, EstadoEvento.Programado)
            {
                Id = 1
            };
        _servicioEvento!.ActualizarEvento(evento);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void ActualizarEventoCostoNegativo()
    {
        var evento = new Evento("Titulo", "Desc", DateTime.Now, DateTime.Now.AddHours(2), 100, -10,
            EstadoEvento.Programado)
        { Id = 1 };
        _servicioEvento!.ActualizarEvento(evento);
    }

    [TestMethod]
    public void ObtenerAtraccionesPorEvento_Valido_RetornaLista()
    {
        var eventoId = 5;
        var atracciones = new List<AtraccionParque>
        {
            new AtraccionParque("Montaña Rusa", TipoAtraccion.MontañaRusa, 12, 50, "Emocionante!") { Id = 1 },
            new AtraccionParque("Carrusel", TipoAtraccion.Simulador, 0, 40, "Familiar") { Id = 2 }
        };

        var evento =
            new Evento("Fiesta", "Evento con atracciones", DateTime.Today, DateTime.Today.AddHours(3), 100, 20,
                EstadoEvento.Programado)
            { Id = eventoId, Atracciones = atracciones };

        var repoMock = new Mock<IRepositorio<Evento>>();
        repoMock.Setup(r => r.EncontrarConRelaciones(It.IsAny<Expression<Func<Evento, bool>>>(), "Atracciones"))
            .Returns(evento);
        var servicio = new ServicioEvento(repoMock.Object);

        var result = servicio.ObtenerAtraccionesPorEvento(eventoId);

        Assert.AreEqual(2, result.Count);
        Assert.AreEqual("Montaña Rusa", result[0].Nombre);
        Assert.AreEqual("Carrusel", result[1].Nombre);
    }

    [TestMethod]
    [ExpectedException(typeof(ExcepcionEntidadNoEncontrada))]
    public void ObtenerAtraccionesPorEvento_EventoNoExiste_LanzaExcepcion()
    {
        var eventoId = 999;
        var repoMock = new Mock<IRepositorio<Evento>>();
        repoMock.Setup(r => r.EncontrarConRelaciones(It.IsAny<Expression<Func<Evento, bool>>>(), "Atracciones"))
            .Returns((Evento?)null);
        var servicio = new ServicioEvento(repoMock.Object);

        servicio.ObtenerAtraccionesPorEvento(eventoId);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void EliminarEventoPorId_IdMenorOIgualCero_LanzaException()
    {
        var repoMock = new Mock<IRepositorio<Evento>>();
        var servicio = new ServicioEvento(repoMock.Object);

        // Evento ID inválido: 0
        servicio.EliminarEventoPorId(0);
    }

    [TestMethod]
    [ExpectedException(typeof(ExcepcionEntidadNoEncontrada))]
    public void ObtenerEventoPorId_EventoNoExiste_LanzaException()
    {
        var repoMock = new Mock<IRepositorio<Evento>>();
        repoMock.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Evento, bool>>>()))
            .Returns((Evento?)null);

        var servicio = new ServicioEvento(repoMock.Object);
        var eventoIdInexistente = 432;

        servicio.ObtenerEventoPorId(eventoIdInexistente);
    }
}
