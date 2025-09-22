using Parque.Dominio.Atracciones;

namespace Parque.Dominio.Test;

[TestClass]
public class AtraccionesTest
{
    [TestMethod]
    public void CalcularAforoDisponible_AforoMenorQueCapacidad_RetornaDisponible()
    {
        // Arrange
        var atraccion = new AtraccionParque("Montaña Rusa", TipoAtraccion.MontañaRusa, 12, 10, "Alta velocidad");
        var aforoActual = 6;

        // Act
        var disponible = atraccion.CalcularAforoDisponible(aforoActual);

        // Assert
        Assert.AreEqual(4, disponible);
    }

    [TestMethod]
    public void CalcularAforoDisponible_AforoIgualACapacidad_RetornaCero()
    {
        // Arrange
        var atraccion = new AtraccionParque("Simulador", TipoAtraccion.Simulador, 10, 8, "Realidad virtual");
        var aforoActual = 8;

        // Act
        var disponible = atraccion.CalcularAforoDisponible(aforoActual);

        // Assert
        Assert.AreEqual(0, disponible);
    }

    [TestMethod]
    public void CalcularAforoDisponible_AforoMayorQueCapacidad_RetornaNegativo()
    {
        // Arrange
        var atraccion = new AtraccionParque("Espectáculo", TipoAtraccion.Espectaculo, 5, 50, "Show en vivo");
        var aforoActual = 55;

        // Act
        var disponible = atraccion.CalcularAforoDisponible(aforoActual);

        // Assert
        Assert.AreEqual(-5, disponible);
    }

    [TestMethod]
    public void Constructor_Atraccion_PropiedadesCorrectamenteInicializadas()
    {
        // Arrange & Act
        var atraccion = new AtraccionParque("Zona Interactiva", TipoAtraccion.ZonaInteractiva, 6, 20, "Juegos interactivos");

        // Assert
        Assert.AreEqual("Zona Interactiva", atraccion.Nombre);
        Assert.AreEqual(TipoAtraccion.ZonaInteractiva, atraccion.Tipo);
        Assert.AreEqual(6, atraccion.EdadMinima);
        Assert.AreEqual(20, atraccion.Capacidad);
        Assert.AreEqual("Juegos interactivos", atraccion.Descripcion);
        Assert.AreEqual(EstadoAtraccion.Disponible, atraccion.Estado);
    }
}
