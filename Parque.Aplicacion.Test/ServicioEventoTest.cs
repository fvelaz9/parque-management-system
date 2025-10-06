using System.Linq.Expressions;
using Moq;
using Parque.Aplicacion;
using Parque.Dominio;
using Parque.Infraestructura.Repositorios;

namespace Parque.Aplicacion.Test;

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
        Evento evento = new Evento(
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
    [ExpectedException(typeof(InvalidOperationException))]
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
        Evento evento = new Evento("Noche", "Temático", DateTime.Now, DateTime.Now.AddHours(2), 100, 50, EstadoEvento.Programado) { Id = 1 };

        var mockRepo = new Mock<IRepositorio<Evento>>();
        mockRepo.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Evento, bool>>>())).Returns(evento);
        mockRepo.Setup(r => r.Eliminar(It.IsAny<Expression<Func<Evento, bool>>>()));

        var servicio = new ServicioEvento(mockRepo.Object);

        servicio.EliminarEventoPorId(1);

        mockRepo.Verify(r => r.Eliminar(It.IsAny<Expression<Func<Evento, bool>>>()), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(Exception))]
    public void EliminarEventoPorIdInvalida()
    {
        var mockRepo = new Mock<IRepositorio<Evento>>();
        var servicio = new ServicioEvento(mockRepo.Object);

        servicio.EliminarEventoPorId(-10);
    }

    [TestMethod]
    [ExpectedException(typeof(Exception))]
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
        Evento evento = new Evento("asada", "Dagdasjh", DateTime.Now, DateTime.Now.AddHours(2), 100, 50, EstadoEvento.Programado) { Id = 1 };
        _mockRepositorio!.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Evento, bool>>>())).Returns(evento);

        Evento resultado = _servicioEvento!.ObtenerEventoPorId(1);

        Assert.IsNotNull(resultado);
        Assert.AreEqual(evento.Id, resultado.Id);
    }

    [TestMethod]
    [ExpectedException(typeof(Exception))]
    public void ObtenerEventoPorIdInvalido()
    {
        _servicioEvento!.ObtenerEventoPorId(0);
    }

    [TestMethod]
    [ExpectedException(typeof(Exception))]
    public void ObtenerEventoPorIdNoExistente()
    {
        _mockRepositorio!.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Evento, bool>>>())).Returns((Evento)null!);

        _servicioEvento!.ObtenerEventoPorId(999);
    }

    [TestMethod]
    public void ListarEventos()
    {
        List<Evento> eventos = new List<Evento>
        {
            new Evento("Evento 1", "addads", DateTime.Now, DateTime.Now.AddHours(1), 50, 10, EstadoEvento.Programado),
            new Evento("Evento 2", "sdadaas", DateTime.Now, DateTime.Now.AddHours(2), 100, 20, EstadoEvento.Cancelado)
        };

        _mockRepositorio!.Setup(r => r.ObtenerTodos()).Returns(eventos);

        List<Evento> resultado = _servicioEvento!.ListarEventos();

        Assert.AreEqual(2, resultado.Count);
        Assert.AreEqual("Evento 1", resultado[0].Titulo);
    }
}
