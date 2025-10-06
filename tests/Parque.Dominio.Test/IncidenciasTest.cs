using Microsoft.VisualStudio.TestTools.UnitTesting;
using Parque.Dominio;

namespace Parque.Dominio.Test;

[TestClass]
public class IncidenciasTest
{
    [TestMethod]
    public void ConstructorPorDefecto_DeberiaInicializarValoresPorDefecto()
    {
        // Act
        var incidencia = new Incidencias();

        // Assert
        Assert.AreEqual(0, incidencia.Id);
        Assert.IsNull(incidencia.Descripcion);
    }

    [TestMethod]
    public void Id_DeberiaPoderAsignarYRecuperar()
    {
        // Arrange
        var incidencia = new Incidencias();
        var idEsperado = 123;

        // Act
        incidencia.Id = idEsperado;

        // Assert
        Assert.AreEqual(idEsperado, incidencia.Id);
    }

    [TestMethod]
    public void Descripcion_DeberiaPoderAsignarYRecuperar()
    {
        // Arrange
        var incidencia = new Incidencias();
        var descripcionEsperada = "Falla en el sistema de frenos";

        // Act
        incidencia.Descripcion = descripcionEsperada;

        // Assert
        Assert.AreEqual(descripcionEsperada, incidencia.Descripcion);
    }

    [TestMethod]
    public void Id_DeberiaPoderSerNegativo()
    {
        // Arrange
        var incidencia = new Incidencias();

        // Act
        incidencia.Id = -1;

        // Assert
        Assert.AreEqual(-1, incidencia.Id);
    }

    [TestMethod]
    public void InstanciasDiferentes_DeberianTenerPropiedadesIndependientes()
    {
        // Arrange
        var incidencia1 = new Incidencias();
        var incidencia2 = new Incidencias();

        // Act
        incidencia1.Id = 1;
        incidencia1.Descripcion = "Incidencia 1";
        incidencia2.Id = 2;
        incidencia2.Descripcion = "Incidencia 2";

        // Assert
        Assert.AreEqual(1, incidencia1.Id);
        Assert.AreEqual("Incidencia 1", incidencia1.Descripcion);
        Assert.AreEqual(2, incidencia2.Id);
        Assert.AreEqual("Incidencia 2", incidencia2.Descripcion);
    }
}
