using Microsoft.VisualStudio.TestTools.UnitTesting;
using Parque.Dominio;
using System;

namespace Parque.Tests
{
    [TestClass]
    public class AtraccionesTest
    {
        [TestMethod]
        public void CalcularAforoDisponible_AforoMenorQueCapacidad_RetornaDisponible()
        {
            // Arrange
            var atraccion = new Atraccion("Montaña Rusa", TipoAtraccion.MontañaRusa, 12, 10, "Alta velocidad");
            
            int aforoActual = 6;

            // Act
            int disponible = atraccion.CalcularAforoDisponible(aforoActual);

            // Assert
            Assert.AreEqual(4, disponible);
        }
        
    }
}
