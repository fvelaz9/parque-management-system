using Parque.Dominio.Atracciones;
using Parque.Dominio.Gamificacion;
using Parque.Dominio.Usuarios;

namespace Parque.Dominio.Test;

[TestClass]
public class GamificacionTest
{
    private readonly AtraccionParque _montañaRusa;
    private readonly AtraccionParque _simulador;
    private readonly AtraccionParque _espectaculo;
    private readonly AtraccionParque _zonaInteractiva;
    private readonly RegistroVisita _registroBase;
    private readonly Evento _eventoActivo;
    private readonly Cuenta _cuenta;
    private readonly Visitante _visitante;

    public GamificacionTest()
    {
        _montañaRusa = new AtraccionParque("Montaña Rusa", TipoAtraccion.MontañaRusa, 12, 20, "Emocionante");
        _simulador = new AtraccionParque("Simulador", TipoAtraccion.Simulador, 10, 15, "Realidad virtual");
        _espectaculo = new AtraccionParque("Espectáculo", TipoAtraccion.Espectaculo, 5, 50, "Show en vivo");
        _zonaInteractiva = new AtraccionParque("Zona Interactiva", TipoAtraccion.ZonaInteractiva, 6, 30, "Juegos");

        _montañaRusa.Id = 1;
        _simulador.Id = 2;
        _espectaculo.Id = 3;
        _zonaInteractiva.Id = 4;

        _cuenta = Cuenta.Crear("Juan", "Perez", new Email("juan@test.com"), "password", Rol.Visitante);
        _visitante = Visitante.Crear(new DateTime(2000, 1, 1));
        _cuenta.AsignarVisitante(_visitante.FechaNacimiento);

        _registroBase = new RegistroVisita(1, Guid.NewGuid(), DateTime.Now);

        _eventoActivo = new Evento("Evento Especial", "Descripción", DateTime.Now, DateTime.Now.AddHours(2), 100, 50.0f, EstadoEvento.Programado);
        _eventoActivo.Atracciones.Add(_montañaRusa);
        _eventoActivo.Id = 1;
    }

    #region PuntuacionPorAtraccion Tests

    [TestMethod]
    public void PuntuacionPorAtraccion_CalcularPuntos_MontañaRusa_Retorna15()
    {
        // Arrange
        var estrategia = new PuntuacionPorAtraccion();

        // Act
        var puntos = estrategia.CalcularPuntos(_registroBase, _montañaRusa, [], null);

        // Assert
        Assert.AreEqual(15, puntos);
    }

    [TestMethod]
    public void PuntuacionPorAtraccion_CalcularPuntos_Simulador_Retorna12()
    {
        // Arrange
        var estrategia = new PuntuacionPorAtraccion();

        // Act
        var puntos = estrategia.CalcularPuntos(_registroBase, _simulador, [], null);

        // Assert
        Assert.AreEqual(12, puntos);
    }

    [TestMethod]
    public void PuntuacionPorAtraccion_CalcularPuntos_Espectaculo_Retorna10()
    {
        var estrategia = new PuntuacionPorAtraccion();

        var puntos = estrategia.CalcularPuntos(_registroBase, _espectaculo, [], null);

        Assert.AreEqual(10, puntos);
    }

    [TestMethod]
    public void PuntuacionPorAtraccion_CalcularPuntos_ZonaInteractiva_Retorna8()
    {
        // Arrange
        var estrategia = new PuntuacionPorAtraccion();

        // Act
        var puntos = estrategia.CalcularPuntos(_registroBase, _zonaInteractiva, [], null);

        // Assert
        Assert.AreEqual(8, puntos);
    }

    [TestMethod]
    public void PuntuacionPorAtraccion_CalcularPuntos_TipoDesconocido_Retorna10()
    {
        // Arrange
        var estrategia = new PuntuacionPorAtraccion();
        var atraccionDesconocida = new AtraccionParque("Desconocida", (TipoAtraccion)999, 10, 20, "Desc");

        // Act
        var puntos = estrategia.CalcularPuntos(_registroBase, atraccionDesconocida, [], null);

        // Assert
        Assert.AreEqual(10, puntos);
    }

    [TestMethod]
    public void PuntuacionPorAtraccion_Nombre_RetornaPorAtraccion()
    {
        // Arrange
        var estrategia = new PuntuacionPorAtraccion();

        // Act & Assert
        Assert.AreEqual("porAtraccion", estrategia.Nombre);
    }

    #endregion

    #region PuntuacionPorEvento Tests

    [TestMethod]
    public void PuntuacionPorEvento_CalcularPuntos_ConEventoActivo_RetornaPuntosMultiplicados()
    {
        // Arrange
        var estrategia = new PuntuacionPorEvento();

        // Act
        var puntos = estrategia.CalcularPuntos(_registroBase, _montañaRusa, [], _eventoActivo);

        // Assert
        Assert.AreEqual(30, puntos); // 10 * 3
    }

    [TestMethod]
    public void PuntuacionPorEvento_CalcularPuntos_SinEventoActivo_RetornaPuntosBase()
    {
        // Arrange
        var estrategia = new PuntuacionPorEvento();

        // Act
        var puntos = estrategia.CalcularPuntos(_registroBase, _montañaRusa, [], null);

        // Assert
        Assert.AreEqual(10, puntos);
    }

    [TestMethod]
    public void PuntuacionPorEvento_CalcularPuntos_EventoNoIncluyeAtraccion_RetornaPuntosBase()
    {
        // Arrange
        var estrategia = new PuntuacionPorEvento();

        // Act
        var puntos = estrategia.CalcularPuntos(_registroBase, _simulador, [], _eventoActivo);

        // Assert
        Assert.AreEqual(10, puntos);
    }

    [TestMethod]
    public void PuntuacionPorEvento_CalcularPuntos_EventoCancelado_RetornaPuntosBase()
    {
        // Arrange
        var estrategia = new PuntuacionPorEvento();
        var eventoCancelado = new Evento("Evento Cancelado", "Desc", DateTime.Now, DateTime.Now.AddHours(1), 50, 25.0f, EstadoEvento.Cancelado);
        eventoCancelado.Atracciones.Add(_montañaRusa);

        // Act
        var puntos = estrategia.CalcularPuntos(_registroBase, _montañaRusa, [], eventoCancelado);

        // Assert
        Assert.AreEqual(10, puntos);
    }

    [TestMethod]
    public void PuntuacionPorEvento_CalcularPuntos_EventoFinalizado_RetornaPuntosBase()
    {
        // Arrange
        var estrategia = new PuntuacionPorEvento();
        var eventoFinalizado = new Evento("Evento Finalizado", "Desc", DateTime.Now.AddHours(-3), DateTime.Now.AddHours(-1), 50, 25.0f, EstadoEvento.Finalizado);
        eventoFinalizado.Atracciones.Add(_montañaRusa);

        // Act
        var puntos = estrategia.CalcularPuntos(_registroBase, _montañaRusa, [], eventoFinalizado);

        // Assert
        Assert.AreEqual(10, puntos);
    }

    [TestMethod]
    public void PuntuacionPorEvento_Nombre_RetornaPorEvento()
    {
        // Arrange
        var estrategia = new PuntuacionPorEvento();

        // Act & Assert
        Assert.AreEqual("PorEvento", estrategia.Nombre);
    }

    #endregion

    #region PuntuacionCombo Tests

    [TestMethod]
    public void PuntuacionCombo_Constructor_ParametrosValidos_CreaInstancia()
    {
        // Arrange & Act
        var estrategia = new PuntuacionCombo(minutosVentana: 15, atraccionesMinimasCombo: 3, puntosBase: 5, puntosCombo: 20);

        // Assert
        Assert.AreEqual("Combo", estrategia.Nombre);
    }

    [TestMethod]
    public void PuntuacionCombo_Constructor_MinutosVentanaCero_LanzaExcepcion()
    {
        // Arrange, Act & Assert
        Assert.ThrowsException<ArgumentException>(() =>
            new PuntuacionCombo(minutosVentana: 0));
    }

    [TestMethod]
    public void PuntuacionCombo_Constructor_AtraccionesMinimasMenorA2_LanzaExcepcion()
    {
        // Arrange, Act & Assert
        Assert.ThrowsException<ArgumentException>(() =>
            new PuntuacionCombo(atraccionesMinimasCombo: 1));
    }

    [TestMethod]
    public void PuntuacionCombo_Constructor_PuntosBaseNegativos_LanzaExcepcion()
    {
        // Arrange, Act & Assert
        Assert.ThrowsException<ArgumentException>(() =>
            new PuntuacionCombo(puntosBase: -1));
    }

    [TestMethod]
    public void PuntuacionCombo_Constructor_PuntosComboNegativos_LanzaExcepcion()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            new PuntuacionCombo(puntosCombo: -5));
    }

    [TestMethod]
    public void PuntuacionCombo_CalcularPuntos_SinCombo_RetornaPuntosBase()
    {
        var estrategia = new PuntuacionCombo(atraccionesMinimasCombo: 3);

        var puntos = estrategia.CalcularPuntos(_registroBase, _montañaRusa, [], null);

        Assert.AreEqual(8, puntos);
    }

    [TestMethod]
    public void PuntuacionCombo_CalcularPuntos_ConCombo_RetornaPuntosCombo()
    {
        // Arrange
        var estrategia = new PuntuacionCombo(minutosVentana: 60, atraccionesMinimasCombo: 2);
        var ahora = DateTime.Now;
        var historial = new List<RegistroVisita>
        {
            new RegistroVisita(2, Guid.NewGuid(), ahora.AddMinutes(-30))
        };

        // Act
        var puntos = estrategia.CalcularPuntos(_registroBase, _montañaRusa, historial, null);

        // Assert
        Assert.AreEqual(25, puntos); // Puntos combo por defecto
    }

    [TestMethod]
    public void PuntuacionCombo_CalcularPuntos_AtraccionesFueraDeVentana_NoCuentanParaCombo()
    {
        // Arrange
        var estrategia = new PuntuacionCombo(minutosVentana: 10, atraccionesMinimasCombo: 2);
        var ahora = DateTime.Now;
        var historial = new List<RegistroVisita>
        {
            new RegistroVisita(2, Guid.NewGuid(), ahora.AddMinutes(-15))
        };

        // Act
        var puntos = estrategia.CalcularPuntos(_registroBase, _montañaRusa, historial, null);

        // Assert
        Assert.AreEqual(8, puntos); // Puntos base porque la atracción está fuera de la ventana
    }

    [TestMethod]
    public void PuntuacionCombo_CalcularPuntos_AtraccionesRepetidas_NoCuentanParaCombo()
    {
        // Arrange
        var estrategia = new PuntuacionCombo(minutosVentana: 60, atraccionesMinimasCombo: 3);
        var ahora = DateTime.Now;
        var historial = new List<RegistroVisita>
        {
            new RegistroVisita(2, Guid.NewGuid(), ahora.AddMinutes(-30)),
            new RegistroVisita(2, Guid.NewGuid(), ahora.AddMinutes(-20)) // Misma atracción
        };

        // Act
        var puntos = estrategia.CalcularPuntos(_registroBase, _montañaRusa, historial, null);

        // Assert
        Assert.AreEqual(8, puntos); // Solo 2 atracciones distintas, no alcanza el combo
    }

    [TestMethod]
    public void PuntuacionCombo_CalcularPuntos_ComboConTresAtracciones_RetornaPuntosCombo()
    {
        // Arrange
        var estrategia = new PuntuacionCombo(minutosVentana: 60, atraccionesMinimasCombo: 3);
        var ahora = DateTime.Now;
        var historial = new List<RegistroVisita>
        {
            new RegistroVisita(2, Guid.NewGuid(), ahora.AddMinutes(-40)),
            new RegistroVisita(3, Guid.NewGuid(), ahora.AddMinutes(-20))
        };

        // Act
        var puntos = estrategia.CalcularPuntos(_registroBase, _montañaRusa, historial, null);

        // Assert
        Assert.AreEqual(25, puntos); // 3 atracciones distintas: 2, 3 y 1 (actual)
    }

    [TestMethod]
    public void PuntuacionCombo_CalcularPuntos_ConParametrosPersonalizados_CalculaCorrectamente()
    {
        // Arrange
        var estrategia = new PuntuacionCombo(minutosVentana: 30, atraccionesMinimasCombo: 2, puntosBase: 5, puntosCombo: 15);
        var ahora = DateTime.Now;
        var historial = new List<RegistroVisita>
        {
            new RegistroVisita(2, Guid.NewGuid(), ahora.AddMinutes(-10))
        };

        // Act
        var puntos = estrategia.CalcularPuntos(_registroBase, _montañaRusa, historial, null);

        // Assert
        Assert.AreEqual(15, puntos); // Puntos combo personalizados
    }

    #endregion

    #region ConfiguracionEstrategia Tests

    [TestMethod]
    public void ConfiguracionEstrategia_ConstructorPorDefecto_InicializaPropiedades()
    {
        // Arrange & Act
        var config = new ConfiguracionEstrategia();

        // Assert
        Assert.AreEqual(string.Empty, config.EstrategiaActiva);
        Assert.IsTrue(config.FechaModificacion <= DateTime.UtcNow);
    }

    [TestMethod]
    public void ConfiguracionEstrategia_ConstructorConEstrategia_InicializaCorrectamente()
    {
        // Arrange & Act
        var config = new ConfiguracionEstrategia("porAtraccion");

        // Assert
        Assert.AreEqual("porAtraccion", config.EstrategiaActiva);
        Assert.IsTrue(config.FechaModificacion <= DateTime.UtcNow);
    }

    [TestMethod]
    public void ConfiguracionEstrategia_ConstructorConEstrategiaVacia_LanzaExcepcion()
    {
        // Arrange, Act & Assert
        Assert.ThrowsException<ArgumentException>(() =>
            new ConfiguracionEstrategia(" "));
    }

    [TestMethod]
    public void ConfiguracionEstrategia_ConstructorConEstrategiaNull_LanzaExcepcion()
    {
        // Arrange, Act & Assert
        Assert.ThrowsException<ArgumentException>(() =>
            new ConfiguracionEstrategia(null!));
    }

    [TestMethod]
    public void ConfiguracionEstrategia_CambiarEstrategia_EstrategiaValida_ActualizaCorrectamente()
    {
        // Arrange
        var config = new ConfiguracionEstrategia("porAtraccion");
        var fechaOriginal = config.FechaModificacion;

        // Act
        config.CambiarEstrategia("Combo");

        // Assert
        Assert.AreEqual("Combo", config.EstrategiaActiva);
        Assert.IsTrue(config.FechaModificacion > fechaOriginal);
    }

    [TestMethod]
    public void ConfiguracionEstrategia_CambiarEstrategia_EstrategiaVacia_LanzaExcepcion()
    {
        // Arrange
        var config = new ConfiguracionEstrategia("porAtraccion");

        // Act & Assert
        Assert.ThrowsException<ArgumentException>(() =>
            config.CambiarEstrategia(" "));
    }

    [TestMethod]
    public void ConfiguracionEstrategia_CambiarEstrategia_EstrategiaNull_LanzaExcepcion()
    {
        // Arrange
        var config = new ConfiguracionEstrategia("porAtraccion");

        // Act & Assert
        Assert.ThrowsException<ArgumentException>(() =>
            config.CambiarEstrategia(null!));
    }

    #endregion

    #region IEstrategiaPuntuacion Interface Tests

    [TestMethod]
    public void IEstrategiaPuntuacion_Implementaciones_TienenPropiedadNombre()
    {
        // Arrange
        IEstrategiaPuntuacion porAtraccion = new PuntuacionPorAtraccion();
        IEstrategiaPuntuacion porEvento = new PuntuacionPorEvento();
        IEstrategiaPuntuacion combo = new PuntuacionCombo();

        // Act & Assert
        Assert.AreEqual("porAtraccion", porAtraccion.Nombre);
        Assert.AreEqual("PorEvento", porEvento.Nombre);
        Assert.AreEqual("Combo", combo.Nombre);
    }

    #endregion

    #region Integration Tests - Escenarios del Mundo Real

    [TestMethod]
    public void EscenarioCompleto_VisitanteConEventoYCombo_CalculaPuntosCorrectamente()
    {
        // Arrange - Visitante en evento especial con múltiples atracciones
        var estrategiaCombo = new PuntuacionCombo(minutosVentana: 60, atraccionesMinimasCombo: 2);
        var ahora = DateTime.Now;

        var historial = new List<RegistroVisita>
        {
            new RegistroVisita(2, Guid.NewGuid(), ahora.AddMinutes(-45)), // Simulador
            new RegistroVisita(3, Guid.NewGuid(), ahora.AddMinutes(-30))  // Espectáculo
        };

        var registroActual = new RegistroVisita(1, Guid.NewGuid(), ahora); // Montaña Rusa en evento

        // Act
        var puntos = estrategiaCombo.CalcularPuntos(registroActual, _montañaRusa, historial, _eventoActivo);

        // Assert - Aunque hay evento, la estrategia Combo no lo considera
        Assert.AreEqual(25, puntos); // Puntos por combo
    }

    [TestMethod]
    public void ComparacionEstrategias_DiferentesResultadosParaMismaVisita()
    {
        // Arrange
        var porAtraccion = new PuntuacionPorAtraccion();
        var porEvento = new PuntuacionPorEvento();
        var combo = new PuntuacionCombo(minutosVentana: 60, atraccionesMinimasCombo: 3);

        var historialCombo = new List<RegistroVisita>
        {
            new RegistroVisita(2, Guid.NewGuid(), DateTime.Now.AddMinutes(-10)),
            new RegistroVisita(3, Guid.NewGuid(), DateTime.Now.AddMinutes(-5))
        };

        // Act
        var puntosAtraccion = porAtraccion.CalcularPuntos(_registroBase, _montañaRusa, [], _eventoActivo);
        var puntosEvento = porEvento.CalcularPuntos(_registroBase, _montañaRusa, [], _eventoActivo);
        var puntosCombo = combo.CalcularPuntos(_registroBase, _montañaRusa, historialCombo, _eventoActivo);

        // Assert
        Assert.AreEqual(15, puntosAtraccion); // Solo por tipo de atracción
        Assert.AreEqual(30, puntosEvento);    // Multiplicado por evento
        Assert.AreEqual(25, puntosCombo);     // Por combo de 3 atracciones
    }

    #endregion
}
