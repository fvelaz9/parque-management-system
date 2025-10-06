namespace Parque.Dominio.Test;

[TestClass]
public class IncidenciaTest
{
    [TestMethod]
    public void ConstructorPorDefecto_DeberiaInicializarValoresPorDefecto()
    {
        var incidencia = new Incidencia();
        Assert.AreEqual(0, incidencia.Id);
        Assert.IsNull(incidencia.Descripcion);
    }

    [TestMethod]
    public void Id_DeberiaPoderAsignarYRecuperar()
    {
        var incidencia = new Incidencia();
        var idEsperado = 123;
        incidencia.Id = idEsperado;
        Assert.AreEqual(idEsperado, incidencia.Id);
    }

    [TestMethod]
    public void Descripcion_DeberiaPoderAsignarYRecuperar()
    {
        var incidencia = new Incidencia();
        var descripcionEsperada = "Falla en el sistema de frenos";
        incidencia.Descripcion = descripcionEsperada;
        Assert.AreEqual(descripcionEsperada, incidencia.Descripcion);
    }

    [TestMethod]
    public void Id_DeberiaPoderSerNegativo()
    {
        var incidencia = new Incidencia();
        incidencia.Id = -1;
        Assert.AreEqual(-1, incidencia.Id);
    }

    [TestMethod]
    public void InstanciasDiferentes_DeberianTenerPropiedadesIndependientes()
    {
        var incidencia1 = new Incidencia();
        var incidencia2 = new Incidencia();
        incidencia1.Id = 1;
        incidencia1.Descripcion = "Incidencia 1";
        incidencia2.Id = 2;
        incidencia2.Descripcion = "Incidencia 2";
        Assert.AreEqual(1, incidencia1.Id);
        Assert.AreEqual("Incidencia 1", incidencia1.Descripcion);
        Assert.AreEqual(2, incidencia2.Id);
        Assert.AreEqual("Incidencia 2", incidencia2.Descripcion);
    }
}
