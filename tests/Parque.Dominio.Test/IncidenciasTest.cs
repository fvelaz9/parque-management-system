namespace Parque.Dominio.Test;

[TestClass]
public class IncidenciasTest
{
    [TestMethod]
    public void ConstructorPorDefecto_DeberiaInicializarValoresPorDefecto()
    {
        var incidencia = new Incidencias();
        Assert.AreEqual(0, incidencia.Id);
        Assert.IsNull(incidencia.Descripcion);
    }

    [TestMethod]
    public void Id_DeberiaPoderAsignarYRecuperar()
    {
        var incidencia = new Incidencias();
        var idEsperado = 123;
        incidencia.Id = idEsperado;
        Assert.AreEqual(idEsperado, incidencia.Id);
    }

    [TestMethod]
    public void Descripcion_DeberiaPoderAsignarYRecuperar()
    {
        var incidencia = new Incidencias();
        var descripcionEsperada = "Falla en el sistema de frenos";
        incidencia.Descripcion = descripcionEsperada;
        Assert.AreEqual(descripcionEsperada, incidencia.Descripcion);
    }

    [TestMethod]
    public void Id_DeberiaPoderSerNegativo()
    {
        var incidencia = new Incidencias();
        incidencia.Id = -1;
        Assert.AreEqual(-1, incidencia.Id);
    }

    [TestMethod]
    public void InstanciasDiferentes_DeberianTenerPropiedadesIndependientes()
    {
        var incidencia1 = new Incidencias();
        var incidencia2 = new Incidencias();
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
