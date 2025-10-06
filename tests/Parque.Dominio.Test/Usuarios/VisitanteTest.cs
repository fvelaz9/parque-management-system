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

    [TestMethod]
    public void ActualizarFecha_ConFechaValida_ActualizaCorrectamente()
    {
        // Arrange
        var fechaOriginal = DateTime.UtcNow.AddYears(-25);
        var visitante = Visitante.Crear(fechaOriginal);
        var nuevaFecha = DateTime.UtcNow.AddYears(-30);

        // Act
        visitante.ActualizarFecha(nuevaFecha);

        // Assert
        Assert.AreEqual(nuevaFecha, visitante.FechaNacimiento);
    }

    [TestMethod]
    public void ActualizarFecha_ConFechaFutura_LanzaExcepcion()
    {
        // Arrange
        var visitante = Visitante.Crear(DateTime.UtcNow.AddYears(-25));
        var fechaFutura = DateTime.UtcNow.AddDays(1);

        // Act & Assert
        var ex = Assert.ThrowsException<ExcepcionDominio>(() =>
            visitante.ActualizarFecha(fechaFutura));

        Assert.AreEqual("La fecha de nacimiento no puede ser en el futuro", ex.Message);
    }

    [TestMethod]
    public void ActualizarFecha_ConFechaMuyAntigua_LanzaExcepcion()
    {
        // Arrange
        var visitante = Visitante.Crear(DateTime.UtcNow.AddYears(-25));
        var fechaMuyAntigua = DateTime.UtcNow.AddYears(-126);

        // Act & Assert
        var ex = Assert.ThrowsException<ExcepcionDominio>(() =>
            visitante.ActualizarFecha(fechaMuyAntigua));

        Assert.AreEqual("La edad max es 125.", ex.Message);
    }

    [TestMethod]
    public void ActualizarFecha_ConFechaEnLimite125Anos_NoLanzaExcepcion()
    {
        // Arrange
        var visitante = Visitante.Crear(DateTime.UtcNow.AddYears(-25));
        var fechaLimite = DateTime.UtcNow.AddYears(-125).AddDays(1);

        // Act
        visitante.ActualizarFecha(fechaLimite);

        // Assert
        Assert.AreEqual(fechaLimite, visitante.FechaNacimiento);
    }

    [TestMethod]
    public void ActualizarFecha_ConFechaActual_NoLanzaExcepcion()
    {
        // Arrange
        var visitante = Visitante.Crear(DateTime.UtcNow.AddYears(-25));
        var fechaActual = DateTime.UtcNow;

        // Act
        visitante.ActualizarFecha(fechaActual);

        // Assert
        Assert.AreEqual(fechaActual, visitante.FechaNacimiento);
    }

    [TestMethod]
    public void AsignarMembresia_APremium_AsignaCorrectamente()
    {
        // Arrange
        var visitante = Visitante.Crear(DateTime.UtcNow.AddYears(-25));

        // Act
        visitante.AsignarMembresia(NivelMembresia.Premium);

        // Assert
        Assert.AreEqual(NivelMembresia.Premium, visitante.NivelMembresia);
    }

    [TestMethod]
    public void AsignarMembresia_AVIP_AsignaCorrectamente()
    {
        // Arrange
        var visitante = Visitante.Crear(DateTime.UtcNow.AddYears(-30));

        // Act
        visitante.AsignarMembresia(NivelMembresia.VIP);

        // Assert
        Assert.AreEqual(NivelMembresia.VIP, visitante.NivelMembresia);
    }

    [TestMethod]
    public void AsignarMembresia_AEstandar_AsignaCorrectamente()
    {
        // Arrange
        var visitante = Visitante.Crear(DateTime.UtcNow.AddYears(-25));
        visitante.AsignarMembresia(NivelMembresia.Premium);

        // Act
        visitante.AsignarMembresia(NivelMembresia.Estandar);

        // Assert
        Assert.AreEqual(NivelMembresia.Estandar, visitante.NivelMembresia);
    }
}
