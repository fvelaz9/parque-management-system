using System;
using DefaultNamespace;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Parque.Dominio;

namespace Parque.Tests;

[TestClass]
public class AtraccionesTest
{
    [TestMethod]
    public void CalcularAforoDisponible_AforoMenorQueCapacidad_RetornaDisponible()
    {
        // Arrange
        var atraccion = new Atraccion("Montaña Rusa", TipoAtraccion.MontañaRusa, 12, 10, "Alta velocidad");
        var aforoActual = 6;

        // Act
        var disponible = atraccion.CalcularAforoDisponible(aforoActual);

        // Assert
        Assert.AreEqual(4, disponible);
    }
}
