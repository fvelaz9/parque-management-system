namespace Parque.Dominio.Test;

[TestClass]
public class EventosTest
{
    [TestMethod]
    public void Constructor_Evento_PropiedadesCorrectamenteInicializadas()
    {
        // Arrange
        var inicio = DateTime.Now;
        var fin = inicio.AddHours(2);

        // Act
        var evento = new Evento("Concierto", "Concierto de rock", inicio, fin, 100, 50.0f, EstadoEvento.Programado);

        // Assert
        Assert.AreEqual("Concierto", evento.Titulo);
        Assert.AreEqual("Concierto de rock", evento.Descripcion);
        Assert.AreEqual(inicio, evento.Inicio);
        Assert.AreEqual(fin, evento.Fin);
        Assert.AreEqual(100, evento.AforoMaximo);
        Assert.AreEqual(50.0f, evento.CostoAdicional);
        Assert.AreEqual(EstadoEvento.Programado, evento.Estado);
        Assert.AreEqual(0, evento.Atracciones.Count);
    }
}
