using Parque.Dominio.Usuarios;

namespace Parque.Dominio.Test.Usuarios;

[TestClass]
public class VisitanteTest
{
    [TestMethod]
    public void Crear_ConDatosValidos_DebeCrearVisitanteConPropiedadesCorrectas()
    {
        // Arrange
        var fechaNacimiento = DateTime.UtcNow;

        // Act
        var visitante = Visitante.Crear(fechaNacimiento);

        // Assert
        Assert.AreNotEqual(Guid.Empty, visitante.Id);
        Assert.AreEqual(fechaNacimiento, visitante.FechaNacimiento);
        Assert.AreEqual(NivelMembresia.Estandar, visitante.NivelMembresia);
    }
}
