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
}
