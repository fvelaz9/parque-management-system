using System.Linq.Expressions;
using Moq;
using Parque.Aplicacion.Servicios;
using Parque.Aplicacion.Servicios.Gamificacion;
using Parque.Dominio;
using Parque.Dominio.Atracciones;
using Parque.Dominio.Gamificacion;
using Parque.Dominio.Usuarios;
using Parque.Infraestructura.Repositorios;

namespace Parque.Aplicacion.Test.Servicios;

[TestClass]
public class ServicioPuntuacionTest
{
    private Mock<IRepositorio<AtraccionParque>>? _repoAtraccionesMock;
    private Mock<IRepositorio<Dominio.Ticket>>? _repoTicketsMock;
    private Mock<IRepositorio<RegistroVisita>>? _repoRegistrosMock;
    private Mock<IRepositorio<PuntuacionVisitante>>? _repoPuntuacionesMock;
    private Mock<IRepositorio<Cuenta>>? _repoCuentasMock;
    private Mock<IRepositorio<Evento>>? _repoEventosMock;
    private Mock<IRepositorio<ConfiguracionEstrategia>>? _repoConfiguracionMock;
    private Mock<IServicioFechaHora>? _servicioFechaHoraMock;
    private Mock<IEstrategiaPuntuacion>? _estrategiaMock;
    private ServicioPuntuacion? _servicio;
    private readonly DateTime _fechaActual = new(2025, 10, 8, 12, 0, 0);

    [TestInitialize]
    public void Setup()
    {
        _repoAtraccionesMock = new Mock<IRepositorio<AtraccionParque>>(MockBehavior.Strict);
        _repoTicketsMock = new Mock<IRepositorio<Dominio.Ticket>>(MockBehavior.Strict);
        _repoRegistrosMock = new Mock<IRepositorio<RegistroVisita>>(MockBehavior.Strict);
        _repoPuntuacionesMock = new Mock<IRepositorio<PuntuacionVisitante>>(MockBehavior.Strict);
        _repoCuentasMock = new Mock<IRepositorio<Cuenta>>(MockBehavior.Strict);
        _repoEventosMock = new Mock<IRepositorio<Evento>>(MockBehavior.Strict);
        _repoConfiguracionMock = new Mock<IRepositorio<ConfiguracionEstrategia>>(MockBehavior.Strict);
        _servicioFechaHoraMock = new Mock<IServicioFechaHora>(MockBehavior.Loose);
        _estrategiaMock = new Mock<IEstrategiaPuntuacion>(MockBehavior.Strict);

        // Configurar fecha por defecto para TODOS los tests
        _servicioFechaHoraMock.Setup(s => s.ObtenerFechaActual())
            .Returns(_fechaActual);

        var estrategias = new List<IEstrategiaPuntuacion> { _estrategiaMock.Object };

        _servicio = new ServicioPuntuacion(
            _repoAtraccionesMock.Object,
            _repoTicketsMock.Object,
            _repoRegistrosMock.Object,
            _repoPuntuacionesMock.Object,
            _repoCuentasMock.Object,
            _repoEventosMock.Object,
            _repoConfiguracionMock.Object,
            estrategias,
            _servicioFechaHoraMock.Object);
    }

    [TestCleanup]
    public void Cleanup()
    {
        _repoAtraccionesMock?.VerifyAll();
        _repoTicketsMock?.VerifyAll();
        _repoRegistrosMock?.VerifyAll();
        _repoPuntuacionesMock?.VerifyAll();
        _repoCuentasMock?.VerifyAll();
        _repoEventosMock?.VerifyAll();
        _repoConfiguracionMock?.VerifyAll();
        _estrategiaMock?.VerifyAll();
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void CalcularYRegistrarPuntos_RegistroNoExiste_LanzaExcepcion()
    {
        var registroVisitaId = 1;
        _repoRegistrosMock!.Setup(r => r.Encontrar(It.IsAny<Expression<Func<RegistroVisita, bool>>>()))
            .Returns((RegistroVisita)null!);

        _servicio!.CalcularYRegistrarPuntos(registroVisitaId);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void CalcularYRegistrarPuntos_AtraccionNoExiste_LanzaExcepcion()
    {
        var registroVisitaId = 1;
        var registro = new RegistroVisita { Id = registroVisitaId, AtraccionId = 1 };
        _repoRegistrosMock!.Setup(r => r.Encontrar(It.IsAny<Expression<Func<RegistroVisita, bool>>>()))
            .Returns(registro);
        _repoAtraccionesMock!.Setup(r => r.Encontrar(It.IsAny<Expression<Func<AtraccionParque, bool>>>()))
            .Returns((AtraccionParque)null!);

        _servicio!.CalcularYRegistrarPuntos(registroVisitaId);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void CalcularYRegistrarPuntos_TicketNoExiste_LanzaExcepcion()
    {
        var registroVisitaId = 1;
        var registro = new RegistroVisita { Id = registroVisitaId, AtraccionId = 1, Identificador = Guid.NewGuid() };
        var atraccion = new AtraccionParque("Montaña Rusa", TipoAtraccion.Simulador, 12, 20, "Rápida");
        _repoRegistrosMock!.Setup(r => r.Encontrar(It.IsAny<Expression<Func<RegistroVisita, bool>>>()))
            .Returns(registro);
        _repoAtraccionesMock!.Setup(r => r.Encontrar(It.IsAny<Expression<Func<AtraccionParque, bool>>>()))
            .Returns(atraccion);
        _repoTicketsMock!.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Dominio.Ticket, bool>>>()))
            .Returns((Dominio.Ticket)null!);

        _servicio!.CalcularYRegistrarPuntos(registroVisitaId);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void CalcularYRegistrarPuntos_CuentaNoExiste_LanzaExcepcion()
    {
        var registroVisitaId = 1;
        var cuentaId = Guid.NewGuid();
        var fechaVisita = new DateTime(2025, 10, 10, 14, 0, 0);
        var registro = new RegistroVisita { Id = registroVisitaId, AtraccionId = 1, Identificador = Guid.NewGuid() };
        var atraccion = new AtraccionParque("Montaña Rusa", TipoAtraccion.Simulador, 12, 20, "Rápida");
        var ticket = new Dominio.Ticket(cuentaId, fechaVisita, 1, TipoTicket.General, _fechaActual);

        _repoRegistrosMock!.Setup(r => r.Encontrar(It.IsAny<Expression<Func<RegistroVisita, bool>>>()))
            .Returns(registro);
        _repoAtraccionesMock!.Setup(r => r.Encontrar(It.IsAny<Expression<Func<AtraccionParque, bool>>>()))
            .Returns(atraccion);
        _repoTicketsMock!.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Dominio.Ticket, bool>>>()))
            .Returns(ticket);
        _repoCuentasMock!.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Cuenta, bool>>>()))
            .Returns((Cuenta)null!);

        _servicio!.CalcularYRegistrarPuntos(registroVisitaId);
    }

    [TestMethod]
    public void CalcularYRegistrarPuntos_PuntuacionNuevaVisitante_CreaPuntuacion()
    {
        var registroVisitaId = 1;
        var cuentaId = Guid.NewGuid();
        var fechaVisita = new DateTime(2025, 10, 10, 14, 0, 0);
        var fechaRegistro = new DateTime(2025, 10, 8, 10, 0, 0);
        var registro = new RegistroVisita
        {
            Id = registroVisitaId,
            AtraccionId = 1,
            Identificador = Guid.NewGuid(),
            FechaIngreso = fechaRegistro
        };
        var atraccion = new AtraccionParque("Montaña Rusa", TipoAtraccion.Simulador, 12, 20, "Rápida");
        var ticket = new Dominio.Ticket(cuentaId, fechaVisita, 1, TipoTicket.General, _fechaActual);
        var cuenta = Cuenta.Crear("Juan", "Pérez", new Email("test@test.com"), "password123", Rol.Visitante);
        var visitante = Visitante.Crear(new DateTime(1990, 1, 1));
        typeof(Cuenta).GetProperty("Visitante")!.SetValue(cuenta, visitante);
        var visitanteId = visitante.Id;
        var registrosVacios = new List<RegistroVisita>();
        var eventosVacios = new List<Evento>();
        var configuraciones = new List<ConfiguracionEstrategia> { new ConfiguracionEstrategia("TestStrategy") };
        var puntuacionesVacias = new List<PuntuacionVisitante>();

        _repoRegistrosMock!.Setup(r => r.Encontrar(It.IsAny<Expression<Func<RegistroVisita, bool>>>()))
            .Returns(registro);
        _repoAtraccionesMock!.Setup(r => r.Encontrar(It.IsAny<Expression<Func<AtraccionParque, bool>>>()))
            .Returns(atraccion);
        _repoTicketsMock!.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Dominio.Ticket, bool>>>()))
            .Returns(ticket);
        _repoCuentasMock!.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Cuenta, bool>>>()))
            .Returns(cuenta);
        _repoRegistrosMock.Setup(r => r.ObtenerTodos()).Returns(registrosVacios);
        _repoEventosMock!.Setup(r => r.ObtenerTodos()).Returns(eventosVacios);
        _repoConfiguracionMock!.Setup(r => r.ObtenerTodos()).Returns(configuraciones);
        _estrategiaMock!.Setup(e => e.Nombre).Returns("TestStrategy");
        _estrategiaMock.Setup(e => e.CalcularPuntos(registro, atraccion, It.IsAny<List<RegistroVisita>>(), null))
            .Returns(50);
        _repoPuntuacionesMock!.Setup(r => r.ObtenerTodos()).Returns(puntuacionesVacias);
        _repoPuntuacionesMock.Setup(r => r.Agregar(It.IsAny<PuntuacionVisitante>()));

        _servicio!.CalcularYRegistrarPuntos(registroVisitaId);

        _repoPuntuacionesMock.Verify(r => r.Agregar(It.Is<PuntuacionVisitante>(
            p => p.VisitanteId == visitanteId && p.Fecha == fechaRegistro.Date)), Times.Once);
    }

    [TestMethod]
    public void CalcularYRegistrarPuntos_PuntuacionExistente_ActualizaPuntuacion()
    {
        var registroVisitaId = 1;
        var cuentaId = Guid.NewGuid();
        var fechaVisita = new DateTime(2025, 10, 10, 14, 0, 0);
        var fechaRegistro = new DateTime(2025, 10, 8, 10, 0, 0);

        var registro = new RegistroVisita
        {
            Id = registroVisitaId,
            AtraccionId = 1,
            Identificador = Guid.NewGuid(),
            FechaIngreso = fechaRegistro
        };
        var atraccion = new AtraccionParque("Montaña Rusa", TipoAtraccion.Simulador, 12, 20, "Rápida");
        var ticket = new Dominio.Ticket(cuentaId, fechaVisita, 1, TipoTicket.General, _fechaActual);
        var cuenta = Cuenta.Crear("Juan", "Pérez", new Email("test@test.com"), "password123", Rol.Visitante);
        var visitante = Visitante.Crear(new DateTime(1990, 1, 1));
        typeof(Cuenta).GetProperty("Visitante")!.SetValue(cuenta, visitante);
        var puntuacionExistente = new PuntuacionVisitante(visitante.Id, fechaRegistro.Date, 30);
        var registrosVacios = new List<RegistroVisita>();
        var eventosVacios = new List<Evento>();
        var configuraciones = new List<ConfiguracionEstrategia> { new ConfiguracionEstrategia("TestStrategy") };
        var puntuaciones = new List<PuntuacionVisitante> { puntuacionExistente };

        _repoRegistrosMock!.Setup(r => r.Encontrar(It.IsAny<Expression<Func<RegistroVisita, bool>>>()))
            .Returns(registro);
        _repoAtraccionesMock!.Setup(r => r.Encontrar(It.IsAny<Expression<Func<AtraccionParque, bool>>>()))
            .Returns(atraccion);
        _repoTicketsMock!.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Dominio.Ticket, bool>>>()))
            .Returns(ticket);
        _repoCuentasMock!.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Cuenta, bool>>>()))
            .Returns(cuenta);
        _repoRegistrosMock.Setup(r => r.ObtenerTodos()).Returns(registrosVacios);
        _repoEventosMock!.Setup(r => r.ObtenerTodos()).Returns(eventosVacios);
        _repoConfiguracionMock!.Setup(r => r.ObtenerTodos()).Returns(configuraciones);
        _estrategiaMock!.Setup(e => e.Nombre).Returns("TestStrategy");
        _estrategiaMock.Setup(e => e.CalcularPuntos(registro, atraccion, It.IsAny<List<RegistroVisita>>(), null))
            .Returns(20);
        _repoPuntuacionesMock!.Setup(r => r.ObtenerTodos()).Returns(puntuaciones);
        _repoPuntuacionesMock.Setup(r => r.Editar(It.IsAny<PuntuacionVisitante>()));

        _servicio!.CalcularYRegistrarPuntos(registroVisitaId);

        _repoPuntuacionesMock.Verify(r => r.Editar(It.Is<PuntuacionVisitante>(
            p => p.VisitanteId == visitante.Id)), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void ObtenerRankingDiario_TopMenorOIgualACero_LanzaExcepcion()
    {
        _servicio!.ObtenerRankingDiario(null, 0);
    }

    [TestMethod]
    public void ObtenerRankingDiario_FechaNula_UsaFechaActual()
    {
        var puntuaciones = new List<PuntuacionVisitante>
        {
            new PuntuacionVisitante(Guid.NewGuid(), _fechaActual.Date, 100),
            new PuntuacionVisitante(Guid.NewGuid(), _fechaActual.Date, 80)
        };
        _repoPuntuacionesMock!.Setup(r => r.ObtenerTodos()).Returns(puntuaciones);

        var resultado = _servicio!.ObtenerRankingDiario(null, 10);

        Assert.AreEqual(2, resultado.Count);
        Assert.AreEqual(1, resultado[0].Posicion);
        Assert.AreEqual(100, resultado[0].PuntosDiarios);
    }

    [TestMethod]
    public void ObtenerRankingDiario_ConFechaEspecifica_FiltraCorrectamente()
    {
        var fecha = new DateTime(2025, 10, 1);
        var puntuaciones = new List<PuntuacionVisitante>
        {
            new PuntuacionVisitante(Guid.NewGuid(), fecha, 100),
            new PuntuacionVisitante(Guid.NewGuid(), _fechaActual.Date, 80)
        };
        _repoPuntuacionesMock!.Setup(r => r.ObtenerTodos()).Returns(puntuaciones);

        var resultado = _servicio!.ObtenerRankingDiario(fecha, 10);

        Assert.AreEqual(1, resultado.Count);
        Assert.AreEqual(100, resultado[0].PuntosDiarios);
    }

    [TestMethod]
    public void ObtenerRankingDiario_LimitaResultadosSegunTop()
    {
        var puntuaciones = new List<PuntuacionVisitante>
        {
            new PuntuacionVisitante(Guid.NewGuid(), _fechaActual.Date, 100),
            new PuntuacionVisitante(Guid.NewGuid(), _fechaActual.Date, 90),
            new PuntuacionVisitante(Guid.NewGuid(), _fechaActual.Date, 80)
        };
        _repoPuntuacionesMock!.Setup(r => r.ObtenerTodos()).Returns(puntuaciones);

        var resultado = _servicio!.ObtenerRankingDiario(null, 2);

        Assert.AreEqual(2, resultado.Count);
        Assert.AreEqual(100, resultado[0].PuntosDiarios);
        Assert.AreEqual(90, resultado[1].PuntosDiarios);
    }

    [TestMethod]
    public void ObtenerRankingDiario_OrdenaDescendentePorPuntos()
    {
        var puntuaciones = new List<PuntuacionVisitante>
        {
            new PuntuacionVisitante(Guid.NewGuid(), _fechaActual.Date, 50),
            new PuntuacionVisitante(Guid.NewGuid(), _fechaActual.Date, 150),
            new PuntuacionVisitante(Guid.NewGuid(), _fechaActual.Date, 100)
        };
        _repoPuntuacionesMock!.Setup(r => r.ObtenerTodos()).Returns(puntuaciones);

        var resultado = _servicio!.ObtenerRankingDiario(null, 10);

        Assert.AreEqual(150, resultado[0].PuntosDiarios);
        Assert.AreEqual(100, resultado[1].PuntosDiarios);
        Assert.AreEqual(50, resultado[2].PuntosDiarios);
        Assert.AreEqual(1, resultado[0].Posicion);
        Assert.AreEqual(2, resultado[1].Posicion);
        Assert.AreEqual(3, resultado[2].Posicion);
    }

    [TestMethod]
    public void ListarEstrategias_RetornaTodasConIndicadorActiva()
    {
        var configuraciones = new List<ConfiguracionEstrategia> { new ConfiguracionEstrategia("TestStrategy") };
        _estrategiaMock!.Setup(e => e.Nombre).Returns("TestStrategy");
        _repoConfiguracionMock!.Setup(r => r.ObtenerTodos()).Returns(configuraciones);

        var resultado = _servicio!.ListarEstrategias();

        Assert.AreEqual(1, resultado.Count);
        Assert.AreEqual("TestStrategy", resultado[0].Nombre);
        Assert.IsTrue(resultado[0].EsActiva);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CambiarEstrategiaActiva_EstrategiaNoExiste_LanzaExcepcion()
    {
        _estrategiaMock!.Setup(e => e.Nombre).Returns("TestStrategy");

        _servicio!.CambiarEstrategiaActiva("NoExiste");
    }

    [TestMethod]
    public void CambiarEstrategiaActiva_SinConfiguracionPrevia_CreaConfiguracion()
    {
        var configuracionesVacias = new List<ConfiguracionEstrategia>();
        _estrategiaMock!.Setup(e => e.Nombre).Returns("TestStrategy");
        _repoConfiguracionMock!.Setup(r => r.ObtenerTodos()).Returns(configuracionesVacias);
        _repoConfiguracionMock.Setup(r => r.Agregar(It.IsAny<ConfiguracionEstrategia>()));

        _servicio!.CambiarEstrategiaActiva("TestStrategy");

        _repoConfiguracionMock.Verify(r => r.Agregar(It.Is<ConfiguracionEstrategia>(
            c => c.EstrategiaActiva == "TestStrategy")), Times.Once);
    }

    [TestMethod]
    public void CambiarEstrategiaActiva_ConConfiguracionExistente_ActualizaConfiguracion()
    {
        var configuracionExistente = new ConfiguracionEstrategia("OtraEstrategia");
        var configuraciones = new List<ConfiguracionEstrategia> { configuracionExistente };
        _estrategiaMock!.Setup(e => e.Nombre).Returns("TestStrategy");
        _repoConfiguracionMock!.Setup(r => r.ObtenerTodos()).Returns(configuraciones);
        _repoConfiguracionMock.Setup(r => r.Editar(It.IsAny<ConfiguracionEstrategia>()));

        _servicio!.CambiarEstrategiaActiva("TestStrategy");

        _repoConfiguracionMock.Verify(r => r.Editar(It.Is<ConfiguracionEstrategia>(
            c => c.EstrategiaActiva == "TestStrategy")), Times.Once);
    }

    [TestMethod]
    public void ObtenerEstrategiaActiva_SinConfiguracion_CreaYRetornaPrimeraEstrategia()
    {
        var configuracionesVacias = new List<ConfiguracionEstrategia>();
        _estrategiaMock!.Setup(e => e.Nombre).Returns("TestStrategy");
        _repoConfiguracionMock!.Setup(r => r.ObtenerTodos()).Returns(configuracionesVacias);
        _repoConfiguracionMock.Setup(r => r.Agregar(It.IsAny<ConfiguracionEstrategia>()));

        var resultado = _servicio!.ObtenerEstrategiaActiva();

        Assert.AreEqual("TestStrategy", resultado);
        _repoConfiguracionMock.Verify(r => r.Agregar(It.Is<ConfiguracionEstrategia>(
            c => c.EstrategiaActiva == "TestStrategy")), Times.Once);
    }

    [TestMethod]
    public void ObtenerEstrategiaActiva_ConConfiguracion_RetornaEstrategiaConfigurada()
    {
        var configuracion = new ConfiguracionEstrategia("TestStrategy");
        var configuraciones = new List<ConfiguracionEstrategia> { configuracion };
        _repoConfiguracionMock!.Setup(r => r.ObtenerTodos()).Returns(configuraciones);

        var resultado = _servicio!.ObtenerEstrategiaActiva();

        Assert.AreEqual("TestStrategy", resultado);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void ObtenerEstrategiaActiva_SinEstrategiasRegistradas_LanzaExcepcion()
    {
        var configuracionesVacias = new List<ConfiguracionEstrategia>();
        var servicioFechaHoraMockNuevo = new Mock<IServicioFechaHora>(MockBehavior.Loose); // ⚠️ CAMBIO: Loose

        var servicioSinEstrategias = new ServicioPuntuacion(
            _repoAtraccionesMock!.Object,
            _repoTicketsMock!.Object,
            _repoRegistrosMock!.Object,
            _repoPuntuacionesMock!.Object,
            _repoCuentasMock!.Object,
            _repoEventosMock!.Object,
            _repoConfiguracionMock!.Object,
            [],
            servicioFechaHoraMockNuevo.Object);

        _repoConfiguracionMock.Setup(r => r.ObtenerTodos()).Returns(configuracionesVacias);

        servicioSinEstrategias.ObtenerEstrategiaActiva();
    }

    [TestMethod]
    public void CalcularYRegistrarPuntos_UtilizaEstrategiaActivaParaCalcularPuntos()
    {
        // Arrange
        var registroVisitaId = 1;
        var cuentaId = Guid.NewGuid();
        var fechaVisita = new DateTime(2025, 10, 10, 14, 0, 0);
        var fechaRegistro = new DateTime(2025, 10, 8, 10, 0, 0);

        var registro = new RegistroVisita
        {
            Id = registroVisitaId,
            AtraccionId = 1,
            Identificador = Guid.NewGuid(),
            FechaIngreso = fechaRegistro
        };
        var atraccion = new AtraccionParque("Montaña Rusa", TipoAtraccion.Simulador, 12, 20, "Rápida");
        var ticket = new Dominio.Ticket(cuentaId, fechaVisita, 1, TipoTicket.General, _fechaActual);
        var cuenta = Cuenta.Crear("Juan", "Pérez", new Email("test@test.com"), "password123", Rol.Visitante);
        var visitante = Visitante.Crear(new DateTime(1990, 1, 1));
        typeof(Cuenta).GetProperty("Visitante")!.SetValue(cuenta, visitante);

        var registrosVacios = new List<RegistroVisita>();
        var eventosVacios = new List<Evento>();
        var configuraciones = new List<ConfiguracionEstrategia> { new ConfiguracionEstrategia("TestStrategy") };
        var puntuacionesVacias = new List<PuntuacionVisitante>();

        // Setup de mocks
        _repoRegistrosMock!.Setup(r => r.Encontrar(It.IsAny<Expression<Func<RegistroVisita, bool>>>()))
            .Returns(registro);
        _repoAtraccionesMock!.Setup(r => r.Encontrar(It.IsAny<Expression<Func<AtraccionParque, bool>>>()))
            .Returns(atraccion);
        _repoTicketsMock!.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Dominio.Ticket, bool>>>()))
            .Returns(ticket);
        _repoCuentasMock!.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Cuenta, bool>>>()))
            .Returns(cuenta);
        _repoRegistrosMock.Setup(r => r.ObtenerTodos()).Returns(registrosVacios);
        _repoEventosMock!.Setup(r => r.ObtenerTodos()).Returns(eventosVacios);

        // Setup que verifica que se usa ObtenerEstrategiaActivaInterno()
        _repoConfiguracionMock!.Setup(r => r.ObtenerTodos()).Returns(configuraciones);
        _estrategiaMock!.Setup(e => e.Nombre).Returns("TestStrategy");
        _estrategiaMock.Setup(e => e.CalcularPuntos(
            registro,
            atraccion,
            It.IsAny<List<RegistroVisita>>(),
            null))
            .Returns(75);

        _repoPuntuacionesMock!.Setup(r => r.ObtenerTodos()).Returns(puntuacionesVacias);
        _repoPuntuacionesMock.Setup(r => r.Agregar(It.IsAny<PuntuacionVisitante>()));

        // Act
        _servicio!.CalcularYRegistrarPuntos(registroVisitaId);

        // Assert - Verifica que se llamó a la estrategia correcta
        _repoConfiguracionMock.Verify(r => r.ObtenerTodos(), Times.Once,
            "Debe obtener configuraciones para determinar estrategia activa");
        _estrategiaMock.Verify(e => e.CalcularPuntos(
            registro,
            atraccion,
            It.IsAny<List<RegistroVisita>>(),
            null),
            Times.Once,
            "Debe usar la estrategia activa obtenida para calcular puntos");
        _repoPuntuacionesMock.Verify(r => r.Agregar(It.Is<PuntuacionVisitante>(
            p => p.PuntosDiarios == 75 && p.PuntosTotales == 75)), Times.Once);
    }
}
