using Parque.Dominio.Excepciones;
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

    [TestMethod]
    public void Crear_ConFechaMinima_DeberiaLanzarExcepcion()
    {
        // Arrange
        var fechaInvalida = DateTime.MinValue;

        // Act & Assert
        Assert.ThrowsException<ExcepcionDominio>(() =>
            Visitante.Crear(fechaInvalida));
    }

    [TestMethod]
    public void Crear_ConFechaMuyAntigua_DeberiaLanzarExcepcion()
    {
        // Arrange
        var fechaMuyAntigua = DateTime.UtcNow.AddYears(-126);

        // Act & Assert
        Assert.ThrowsException<ExcepcionDominio>(() =>
            Visitante.Crear(fechaMuyAntigua));
    }

    [TestMethod]
    public void Crear_ConFechaFutura_DeberiaLanzarExcepcion()
    {
        // Arrange
        var fechaFutura = DateTime.UtcNow.AddDays(1);

        // Act & Assert
        Assert.ThrowsException<ExcepcionDominio>(() =>
            Visitante.Crear(fechaFutura));
    }
}
