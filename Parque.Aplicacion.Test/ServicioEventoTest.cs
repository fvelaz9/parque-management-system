using Moq;
using Parque.Aplicacion;
using Parque.Dominio;
using Parque.Infraestructura.Repositorios;

namespace Parque.Aplicacion.Test;

[TestClass]
public class ServicioEventoTest
{
    [TestMethod]
    public void AgregarEventoValido()
    {
        var mockRepositorio = new Mock<IRepositorio<Evento>>();
        mockRepositorio.Setup(r => r.ObtenerTodos()).Returns(new List<Evento>());

        var servicio = new ServicioEvento(mockRepositorio.Object);

        var evento = new Evento(
            "Noche de Dinosaurios",
            "Temático",
            new DateTime(2025, 12, 20, 20, 0, 0),
            new DateTime(2025, 12, 20, 23, 0, 0),
            300,
            500f,
            EstadoEvento.Programado);
        var resultado = servicio.AgregarEvento(evento);

        mockRepositorio.Verify(r => r.Agregar(It.Is<Evento>(e => e == evento)), Times.Once);
        Assert.Equals("Noche de Dinosaurios", resultado.Titulo);
    }
}
