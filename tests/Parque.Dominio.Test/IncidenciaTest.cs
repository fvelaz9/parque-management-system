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

    [TestMethod]
    public void ConstructorConParametros_DeberiaAsignarPropiedades()
    {
        var descripcion = "Falla";
        var fechaInicio = DateTime.Now;
        var fechaFin = DateTime.Now.AddHours(2);
        var atraccionId = 5;

        var incidencia = new Incidencia(descripcion, fechaInicio, fechaFin, atraccionId);

        Assert.AreEqual(descripcion, incidencia.Descripcion);
        Assert.AreEqual(fechaInicio, incidencia.FechaReporte);
        Assert.AreEqual(fechaFin, incidencia.FechaResolucionEstimada);
        Assert.AreEqual(atraccionId, incidencia.AtraccionId);
    }

    [TestMethod]
    public void EstaActiva_DeberiaRetornarVerdaderoSiNoHaLlegadoFechaFin()
    {
        var incidencia = new Incidencia("Test", DateTime.Now, DateTime.Now.AddHours(1), 1);
        Assert.IsTrue(incidencia.EstaActiva());
    }

    [TestMethod]
    public void EstaDisponible_DeberiaRetornarVerdaderoSiYaPasoFechaFin()
    {
        var incidencia = new Incidencia("Test", DateTime.Now.AddHours(-2), DateTime.Now.AddHours(-1), 1);
        Assert.IsTrue(incidencia.EstaDisponible());
    }
}
