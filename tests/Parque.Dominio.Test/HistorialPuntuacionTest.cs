namespace Parque.Dominio.Test;
[TestClass]
public class HistorialPuntuacionTest
{
    [TestMethod]
    public void ConstructorSinProperties()
    {
        var historial = new HistorialPuntuacion();

        Assert.AreEqual(string.Empty, historial.OrigenPuntos);
        Assert.AreEqual(string.Empty, historial.EstrategiaActiva);
        Assert.AreEqual(default(DateTime), historial.FechaHora);
        Assert.AreEqual(0, historial.Puntos);
    }

    [TestMethod]
    public void ConstructorConPropiedades()
    {
        var fechaHora = DateTime.Now;
        var origen = "Atracción X";
        var estrategia = "Estrategia ABC";
        var puntos = 123;

        var historial = new HistorialPuntuacion(fechaHora, origen, estrategia, puntos);

        Assert.AreEqual(fechaHora, historial.FechaHora);
        Assert.AreEqual(origen, historial.OrigenPuntos);
        Assert.AreEqual(estrategia, historial.EstrategiaActiva);
        Assert.AreEqual(puntos, historial.Puntos);
    }
}
