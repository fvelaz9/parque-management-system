using Microsoft.VisualStudio.TestTools.UnitTesting;
using Parque.Dominio.Excepciones;
using Parque.Dominio.Gamificacion;
using Parque.Dominio.Usuarios;

namespace Parque.Dominio.Tests.Gamificacion;

[TestClass]
public class RecompensaTest
{
    [TestMethod]
    public void Constructor_ConDatosValidos_DebeCrearRecompensaConPropiedadesCorrectas()
    {
        // Arrange
        var id = Guid.NewGuid();
        var nombre = "Entrada VIP";
        var costo = 500;
        var cantidad = 10;

        // Act
        var recompensa = new Recompensas
        {
            Id = id,
            Nombre = nombre,
            CostoEnPuntos = costo,
            CantidadDisponible = cantidad
        };

        // Assert
        Assert.AreEqual(id, recompensa.Id);
        Assert.AreEqual(nombre, recompensa.Nombre);
        Assert.AreEqual(costo, recompensa.CostoEnPuntos);
        Assert.AreEqual(cantidad, recompensa.CantidadDisponible);
    }

    [TestMethod]
    public void ReducirStock_ConStockDisponible_DebeDisminuirCantidadEnUno()
    {
        // Arrange
        var recompensa = new Recompensas
        {
            Id = Guid.NewGuid(),
            Nombre = "Test",
            CostoEnPuntos = 100,
            CantidadDisponible = 5
        };

        // Act
        recompensa.ReducirStock();

        // Assert
        Assert.AreEqual(4, recompensa.CantidadDisponible);
    }

    [TestMethod]
    public void ReducirStock_ConStockNegativo_DebeLanzarExcepcion()
    {
        var fecha = new DateTime(2025, 11, 10);
        var recompensa = new Recompensas
        {
            Id = Guid.NewGuid(),
            Nombre = "Test",
            Descripcion = "Carlitos",
            CostoEnPuntos = 100,
            CantidadDisponible = -1,
            NivelMembresiaRequerido = NivelMembresia.Premium,
            FechaCreacion = fecha
        };

        // Act & Assert
        Assert.ThrowsException<ExcepcionDominio>(() => recompensa.ReducirStock());
        Assert.AreEqual(recompensa.Nombre, "Test");
        Assert.AreEqual(recompensa.Descripcion, "Carlitos");
        Assert.AreEqual(recompensa.NivelMembresiaRequerido, NivelMembresia.Premium);
        Assert.AreEqual(recompensa.FechaCreacion, fecha);
    }
}
