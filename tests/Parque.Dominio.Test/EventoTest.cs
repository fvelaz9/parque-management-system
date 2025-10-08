namespace Parque.Dominio.Test;

[TestClass]
public class EventosTest
{
    [TestMethod]
    public void Constructor_Evento_PropiedadesCorrectamenteInicializadas()
    {
        var inicio = DateTime.Now;
        var fin = inicio.AddHours(2);
        var evento = new Evento("Concierto", "Concierto de rock", inicio, fin, 100, 50.0f, EstadoEvento.Programado);

        Assert.AreEqual("Concierto", evento.Titulo);
        Assert.AreEqual("Concierto de rock", evento.Descripcion);
        Assert.AreEqual(inicio, evento.Inicio);
        Assert.AreEqual(fin, evento.Fin);
        Assert.AreEqual(100, evento.AforoMaximo);
        Assert.AreEqual(50.0f, evento.CostoAdicional);
        Assert.AreEqual(EstadoEvento.Programado, evento.Estado);
        Assert.AreEqual(0, evento.Atracciones.Count);
    }

    [TestMethod]
    public void Constructor_Evento_PropiedadesInicializadas()
    {
        var evento = new Evento("Concierto", "Rock", DateTime.Now, DateTime.Now.AddHours(2), 100, 50.0f, EstadoEvento.Programado);

        Assert.AreEqual("Concierto", evento.Titulo);
        Assert.AreEqual(100, evento.AforoMaximo);
        Assert.AreEqual(50.0f, evento.CostoAdicional);
        Assert.AreEqual(EstadoEvento.Programado, evento.Estado);
    }

    [TestMethod]
    public void Set_Propiedades_Evento_SeActualizan()
    {
        var evento = new Evento("Original", "Desc", DateTime.Now, DateTime.Now.AddHours(1), 50, 25.0f, EstadoEvento.Programado);
        evento.Titulo = "Nuevo Titulo";
        evento.AforoMaximo = 200;
        evento.Estado = EstadoEvento.Cancelado;

        Assert.AreEqual("Nuevo Titulo", evento.Titulo);
        Assert.AreEqual(200, evento.AforoMaximo);
        Assert.AreEqual(EstadoEvento.Cancelado, evento.Estado);
    }

    [TestMethod]
    public void Atracciones_Lista_InicializadaVacia()
    {
        var evento = new Evento("Evento", "Desc", DateTime.Now, DateTime.Now.AddHours(1), 100, 0, EstadoEvento.Programado);

        Assert.AreEqual(0, evento.Atracciones.Count);
    }
}
