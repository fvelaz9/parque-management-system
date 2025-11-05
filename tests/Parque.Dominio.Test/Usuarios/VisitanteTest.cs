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

    [TestMethod]
    public void Constructor_ConParametros_InicializaCorrectamente()
    {
        var visitanteId = Guid.NewGuid();
        var fecha = new DateTime(2025, 10, 8, 14, 30, 0);
        var puntos = 50;

        var puntuacion = new PuntuacionVisitante(visitanteId, fecha, puntos);

        Assert.AreEqual(visitanteId, puntuacion.VisitanteId);
        Assert.AreEqual(new DateTime(2025, 10, 8), puntuacion.Fecha);
        Assert.AreEqual(50, puntuacion.PuntosDiarios);
        Assert.AreEqual(50, puntuacion.PuntosTotales);
    }

    [TestMethod]
    public void Constructor_SinParametros_CreaInstanciaVacia()
    {
        var puntuacion = new PuntuacionVisitante();

        Assert.IsNotNull(puntuacion);
        Assert.AreEqual(Guid.Empty, puntuacion.VisitanteId);
        Assert.AreEqual(default(DateTime), puntuacion.Fecha);
        Assert.AreEqual(0, puntuacion.PuntosDiarios);
        Assert.AreEqual(0, puntuacion.PuntosTotales);
    }

    [TestMethod]
    public void AgregarPuntos_IncrementaPuntosDiariosYTotales()
    {
        var puntuacion = new PuntuacionVisitante(Guid.NewGuid(), DateTime.Today, 30);

        puntuacion.AgregarPuntos(20);

        Assert.AreEqual(50, puntuacion.PuntosDiarios);
        Assert.AreEqual(50, puntuacion.PuntosTotales);
    }

    [TestMethod]
    public void RestablecerPuntosDiarios_PonePuntosDiariosEnCero()
    {
        var puntuacion = new PuntuacionVisitante(Guid.NewGuid(), DateTime.Today, 100);
        puntuacion.AgregarPuntos(50);

        puntuacion.RestablecerPuntosDiarios();

        Assert.AreEqual(0, puntuacion.PuntosDiarios);
        Assert.AreEqual(150, puntuacion.PuntosTotales);
    }

    [TestMethod]
    public void Id_SetYGet_FuncionaCorrectamente()
    {
        var puntuacion = new PuntuacionVisitante();

        puntuacion.Id = 42;

        Assert.AreEqual(42, puntuacion.Id);
    }

    [TestMethod]
    public void Constructor_NormalizaFechaASoloFecha()
    {
        var visitanteId = Guid.NewGuid();
        var fechaConHora = new DateTime(2025, 10, 8, 15, 30, 45);

        var puntuacion = new PuntuacionVisitante(visitanteId, fechaConHora, 10);

        Assert.AreEqual(new DateTime(2025, 10, 8), puntuacion.Fecha);
        Assert.AreEqual(TimeSpan.Zero, puntuacion.Fecha.TimeOfDay);
    }

    [TestMethod]
    public void AgregarPuntuacionAHistoria()
    {
        // Arrange
        var visitante = Visitante.Crear(DateTime.UtcNow.AddYears(-20));
        var historial = new HistorialPuntuacion(DateTime.UtcNow, "estrategiaX", "eventoX", 25);

        // Act
        visitante.AgregarPuntuacionAHistorial(historial);

        // Assert
        Assert.AreEqual(1, visitante.HistorialPuntuaciones.Count);
        Assert.AreEqual(historial, visitante.HistorialPuntuaciones[0]);
    }
}
