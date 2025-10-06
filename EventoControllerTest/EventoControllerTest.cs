using Moq;
using Parque.Aplicacion;
using Parque.WebApi.Evento;

namespace EventoControllerTest;

[TestClass]
public class EventoControllerTest
{
    private Mock<IServicioEvento>? _servicioEventoMock;
    private EventoController? _controller;
    [TestInitialize]
    public void Initialize()
    {
        _servicioEventoMock = new Mock<IServicioEvento>(MockBehavior.Strict);
        _controller = new EventoController(_servicioEventoMock.Object);
    }

    [TestMethod]
    [ExpectedException(typeof(Exception))]
    public void CrearConRequestNull()
    {
        _controller!.Crear(null);
    }
}
