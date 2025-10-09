using Parque.Dominio.Atracciones;

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
    public void Set_Descripcion_ActualizaCorrectamente()
    {
        var evento = new Evento("Evento", "Desc original", DateTime.Now, DateTime.Now.AddHours(1), 100, 0, EstadoEvento.Programado);

        evento.Descripcion = "Nueva descripción";

        Assert.AreEqual("Nueva descripción", evento.Descripcion);
    }

    [TestMethod]
    public void Set_Inicio_ActualizaCorrectamente()
    {
        var evento = new Evento("Evento", "Desc", DateTime.Now, DateTime.Now.AddHours(1), 100, 0, EstadoEvento.Programado);
        var nuevoInicio = DateTime.Now.AddDays(1);

        evento.Inicio = nuevoInicio;

        Assert.AreEqual(nuevoInicio, evento.Inicio);
    }

    [TestMethod]
    public void Set_Fin_ActualizaCorrectamente()
    {
        var evento = new Evento("Evento", "Desc", DateTime.Now, DateTime.Now.AddHours(1), 100, 0, EstadoEvento.Programado);
        var nuevoFin = DateTime.Now.AddDays(2);

        evento.Fin = nuevoFin;

        Assert.AreEqual(nuevoFin, evento.Fin);
    }

    [TestMethod]
    public void Set_CostoAdicional_ActualizaCorrectamente()
    {
        var evento = new Evento("Evento", "Desc", DateTime.Now, DateTime.Now.AddHours(1), 100, 0, EstadoEvento.Programado);

        evento.CostoAdicional = 75.5f;

        Assert.AreEqual(75.5f, evento.CostoAdicional);
    }

    [TestMethod]
    public void Set_Atracciones_ActualizaCorrectamente()
    {
        var evento = new Evento("Evento", "Desc", DateTime.Now, DateTime.Now.AddHours(1), 100, 0, EstadoEvento.Programado);
        var nuevaLista = new List<AtraccionParque>
        {
            new AtraccionParque("Atraccion1", TipoAtraccion.MontañaRusa, 12, 20, "Desc1")
        };

        evento.Atracciones = nuevaLista;

        Assert.AreEqual(1, evento.Atracciones.Count);
        Assert.AreEqual("Atraccion1", evento.Atracciones[0].Nombre);
    }

    [TestMethod]
    public void Get_Id_RetornaValorCorrecto()
    {
        var evento = new Evento("Evento", "Desc", DateTime.Now, DateTime.Now.AddHours(1), 100, 0, EstadoEvento.Programado);
        evento.Id = 5;

        var id = evento.Id;

        Assert.AreEqual(5, id);
    }

    [TestMethod]
    public void Atracciones_Lista_InicializadaVacia()
    {
        var evento = new Evento("Evento", "Desc", DateTime.Now, DateTime.Now.AddHours(1), 100, 0, EstadoEvento.Programado);

        Assert.AreEqual(0, evento.Atracciones.Count);
    }
}
