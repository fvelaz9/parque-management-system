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
        var fechaInicio = new DateTime(2025, 10, 8, 10, 0, 0);
        var fechaFin = new DateTime(2025, 10, 8, 12, 0, 0);
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
        var fechaReporte = new DateTime(2025, 10, 8, 10, 0, 0);
        var fechaResolucion = new DateTime(2025, 10, 8, 12, 0, 0);
        var fechaReferencia = new DateTime(2025, 10, 8, 11, 0, 0);
        var incidencia = new Incidencia("Test", fechaReporte, fechaResolucion, 1);
        incidencia.Disponible = true;
        Assert.IsTrue(incidencia.EstaActiva(fechaReferencia));
        Assert.IsTrue(incidencia.Disponible);
    }

    [TestMethod]
    public void EstaActiva_DeberiaRetornarFalsoSiYaPasoFechaFin()
    {
        var fechaReporte = new DateTime(2025, 10, 8, 10, 0, 0);
        var fechaResolucion = new DateTime(2025, 10, 8, 12, 0, 0);
        var fechaReferencia = new DateTime(2025, 10, 8, 13, 0, 0);
        var incidencia = new Incidencia("Test", fechaReporte, fechaResolucion, 1);
        Assert.IsFalse(incidencia.EstaActiva(fechaReferencia));
    }

    [TestMethod]
    public void EstaDisponible_DeberiaRetornarVerdaderoSiYaPasoFechaFin()
    {
        var fechaReporte = new DateTime(2025, 10, 8, 10, 0, 0);
        var fechaResolucion = new DateTime(2025, 10, 8, 12, 0, 0);
        var fechaReferencia = new DateTime(2025, 10, 8, 13, 0, 0);
        var incidencia = new Incidencia("Test", fechaReporte, fechaResolucion, 1);
        Assert.IsTrue(incidencia.EstaDisponible(fechaReferencia));
    }

    [TestMethod]
    public void EstaDisponible_DeberiaRetornarFalsoSiNoHaLlegadoFechaFin()
    {
        var fechaReporte = new DateTime(2025, 10, 8, 10, 0, 0);
        var fechaResolucion = new DateTime(2025, 10, 8, 12, 0, 0);
        var fechaReferencia = new DateTime(2025, 10, 8, 11, 0, 0);
        var incidencia = new Incidencia("Test", fechaReporte, fechaResolucion, 1);
        Assert.IsFalse(incidencia.EstaDisponible(fechaReferencia));
    }
}
