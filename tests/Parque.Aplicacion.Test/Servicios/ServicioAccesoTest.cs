using System.Linq.Expressions;
using Moq;
using Parque.Aplicacion.DTOs;
using Parque.Aplicacion.DTOs.Usuarios;
using Parque.Aplicacion.Servicios;
using Parque.Aplicacion.Servicios.Acceso;
using Parque.Aplicacion.Servicios.Gamificacion;
using Parque.Dominio;
using Parque.Dominio.Atracciones;
using Parque.Dominio.Excepciones;
using Parque.Dominio.Usuarios;
using Parque.Infraestructura.Repositorios;

namespace Parque.Aplicacion.Test.Servicios;

[TestClass]
public class ServicioAccesoTest
{
    private Mock<IRepositorio<AtraccionParque>>? _repoAtraccionesMock;
    private Mock<IRepositorio<Dominio.Ticket>>? _repoTicketsMock;
    private Mock<IRepositorio<RegistroVisita>>? _repoRegistrosMock;
    private Mock<IRepositorio<MantenimientoPreventivo>>? _repoMantenimientosMock;
    private Mock<IRepositorio<Cuenta>>? _repoCuentasMock;
    private Mock<IRepositorio<Evento>>? _repoEventoMock;
    private Mock<IServicioFechaHora>? _servicioFechaHoraMock;
    private Mock<IServicioPuntuacion>? _servicioPuntuacionMock;

    private ServicioAcceso? _servicio;

    [TestInitialize]
    public void Setup()
    {
        _repoAtraccionesMock = new Mock<IRepositorio<AtraccionParque>>();
        _repoTicketsMock = new Mock<IRepositorio<Dominio.Ticket>>();
        _repoRegistrosMock = new Mock<IRepositorio<RegistroVisita>>();
        _repoMantenimientosMock = new Mock<IRepositorio<MantenimientoPreventivo>>();
        _repoCuentasMock = new Mock<IRepositorio<Cuenta>>();
        _repoEventoMock = new Mock<IRepositorio<Evento>>();
        _servicioFechaHoraMock = new Mock<IServicioFechaHora>();
        _servicioPuntuacionMock = new Mock<IServicioPuntuacion>();

        _servicioFechaHoraMock.Setup(s => s.ObtenerFechaActual())
            .Returns(new DateTime(2025, 10, 8, 12, 0, 0));

        _servicio = new ServicioAcceso(
            _repoAtraccionesMock.Object,
            _repoTicketsMock.Object,
            _repoRegistrosMock.Object,
            _repoMantenimientosMock.Object,
            _repoCuentasMock.Object,
            _repoEventoMock.Object,
            _servicioFechaHoraMock.Object,
            _servicioPuntuacionMock.Object);
    }

    [TestMethod]
    public void ValidarAcceso_TicketNoEncontrado_RetornaAccesoDenegado()
    {
        var request = new ValidarAccesoRequest
        {
            CodigoTicket = Guid.NewGuid(),
            AtraccionId = 1
        };

        _repoTicketsMock!.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Dominio.Ticket, bool>>>()))
            .Returns((Dominio.Ticket?)null);

        var resultado = _servicio!.ValidarAcceso(request);

        Assert.IsFalse(resultado.AccesoPermitido);
        Assert.AreEqual("El ticket no fue encontrado", resultado.Mensaje);
    }

    [TestMethod]
    public void ValidarAcceso_AtraccionNoEncontrada_RetornaAccesoDenegado()
    {
        var fechaActual = new DateTime(2025, 10, 8);
        var ticket = new Dominio.Ticket { Codigo = Guid.NewGuid(), FechaVisita = fechaActual };

        var request = new ValidarAccesoRequest
        {
            CodigoTicket = ticket.Codigo,
            AtraccionId = 999
        };

        _repoTicketsMock!.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Dominio.Ticket, bool>>>()))
            .Returns(ticket);

        _repoAtraccionesMock!.Setup(r => r.Encontrar(It.IsAny<Expression<Func<AtraccionParque, bool>>>()))
            .Returns((AtraccionParque?)null);

        var resultado = _servicio!.ValidarAcceso(request);

        Assert.IsFalse(resultado.AccesoPermitido);
        Assert.AreEqual("Atracción no encontrada", resultado.Mensaje);
    }

    [TestMethod]
    public void ValidarAcceso_FechaTicketInvalida_RetornaAccesoDenegado()
    {
        var fechaActual = new DateTime(2025, 10, 8);
        var fechaVisita = fechaActual.AddDays(-1);

        var cuenta = Cuenta.Crear("Juan", "Perez", new Email("test@test.com"), "pass123", Rol.Visitante);
        cuenta.AsignarVisitante(fechaActual.AddYears(-25));

        var ticket = new Dominio.Ticket
        {
            Codigo = Guid.NewGuid(),
            FechaVisita = fechaVisita,
            EsValido = true,
            CuentaId = cuenta.Id
        };

        var atraccion = new AtraccionParque("Montaña Rusa", TipoAtraccion.MontañaRusa, 12, 24, "Test") { Id = 1 };

        var request = new ValidarAccesoRequest
        {
            CodigoTicket = ticket.Codigo,
            AtraccionId = 1,
            CuentaVisitanteId = cuenta.Id
        };

        _repoTicketsMock!.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Dominio.Ticket, bool>>>()))
            .Returns(ticket);

        _repoAtraccionesMock!.Setup(r => r.Encontrar(It.IsAny<Expression<Func<AtraccionParque, bool>>>()))
            .Returns(atraccion);

        _repoCuentasMock!.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Cuenta, bool>>>()))
            .Returns(cuenta);

        var resultado = _servicio!.ValidarAcceso(request);

        Assert.IsFalse(resultado.AccesoPermitido);
        Assert.IsTrue(resultado.Mensaje.Contains("Ticket válido solo para"));
    }

    [TestMethod]
    public void ValidarAcceso_CuentaNoEncontrada_RetornaAccesoDenegado()
    {
        var fechaActual = new DateTime(2025, 10, 8);

        var cuenta = Cuenta.Crear("Juan", "Perez", new Email("test@test.com"), "pass123", Rol.Visitante);

        var ticket = new Dominio.Ticket
        {
            Codigo = Guid.NewGuid(),
            FechaVisita = fechaActual,
            TipoEntrada = TipoTicket.General,
            EsValido = true
        };

        var atraccion = new AtraccionParque("Carrusel", TipoAtraccion.Simulador, 0, 30, "Test") { Id = 1 };

        var request = new ValidarAccesoRequest
        {
            CodigoTicket = ticket.Codigo,
            AtraccionId = 1,
            CuentaVisitanteId = cuenta.Id
        };

        _repoTicketsMock!.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Dominio.Ticket, bool>>>()))
            .Returns(ticket);

        _repoAtraccionesMock!.Setup(r => r.Encontrar(It.IsAny<Expression<Func<AtraccionParque, bool>>>()))
            .Returns(atraccion);

        _repoCuentasMock!.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Cuenta, bool>>>()))
            .Returns((Cuenta?)null);

        var resultado = _servicio!.ValidarAcceso(request);

        Assert.IsFalse(resultado.AccesoPermitido);
        Assert.AreEqual("Cuenta no encontrada", resultado.Mensaje);
    }

    [TestMethod]
    public void ValidarAcceso_AforoCompleto_RetornaAccesoDenegado()
    {
        var fechaActual = new DateTime(2025, 10, 8);

        var cuenta = Cuenta.Crear("Juan", "Perez", new Email("test@test.com"), "pass123", Rol.Visitante);
        cuenta.AsignarVisitante(fechaActual.AddYears(-25));
        var cuentaId = cuenta.Id;

        var ticket = new Dominio.Ticket
        {
            Codigo = Guid.NewGuid(),
            FechaVisita = fechaActual,
            TipoEntrada = TipoTicket.General,
            CuentaId = cuentaId,
            EsValido = true
        };

        var atraccion = new AtraccionParque("Simulador", TipoAtraccion.Simulador, 8, 2, "Test") { Id = 1 };

        var registros = new List<RegistroVisita>
        {
            new() { AtraccionId = 1, FechaEgreso = null },
            new() { AtraccionId = 1, FechaEgreso = null }
        };

        var request = new ValidarAccesoRequest
        {
            CodigoTicket = ticket.Codigo,
            AtraccionId = 1,
            CuentaVisitanteId = cuenta.Id
        };

        _repoTicketsMock!.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Dominio.Ticket, bool>>>()))
            .Returns(ticket);

        _repoAtraccionesMock!.Setup(r => r.Encontrar(It.IsAny<Expression<Func<AtraccionParque, bool>>>()))
            .Returns(atraccion);

        _repoCuentasMock!.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Cuenta, bool>>>()))
            .Returns(cuenta);

        // Sin mantenimientos activos
        _repoMantenimientosMock!.Setup(r => r.ObtenerTodos()).Returns([]);

        _repoRegistrosMock!.Setup(r => r.ObtenerTodos()).Returns(registros);

        _repoEventoMock!.Setup(r => r.ObtenerConRelaciones(
            It.IsAny<Expression<Func<Evento, bool>>>(),
            It.IsAny<string>()))
            .Returns([]);

        var resultado = _servicio!.ValidarAcceso(request);

        Assert.IsFalse(resultado.AccesoPermitido);
        Assert.IsTrue(resultado.Mensaje.Contains("Aforo completo"));
    }

    [TestMethod]
    public void ValidarAcceso_TodoValido_RetornaAccesoPermitido()
    {
        var fechaActual = new DateTime(2025, 10, 8);

        var cuenta = Cuenta.Crear("Juan", "Perez", new Email("test@test.com"), "pass123", Rol.Visitante);
        cuenta.AsignarVisitante(fechaActual.AddYears(-25));
        var cuentaId = cuenta.Id;

        var ticket = new Dominio.Ticket
        {
            Codigo = Guid.NewGuid(),
            FechaVisita = fechaActual,
            TipoEntrada = TipoTicket.General,
            CuentaId = cuentaId,
            EsValido = true
        };

        var atraccion = new AtraccionParque("Montaña Rusa", TipoAtraccion.MontañaRusa, 12, 24, "Test") { Id = 1 };

        var request = new ValidarAccesoRequest
        {
            CodigoTicket = ticket.Codigo,
            AtraccionId = 1,
            CuentaVisitanteId = cuenta.Id
        };

        _repoTicketsMock!.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Dominio.Ticket, bool>>>()))
            .Returns(ticket);

        _repoAtraccionesMock!.Setup(r => r.Encontrar(It.IsAny<Expression<Func<AtraccionParque, bool>>>()))
            .Returns(atraccion);

        _repoCuentasMock!.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Cuenta, bool>>>()))
            .Returns(cuenta);

        _repoMantenimientosMock!.Setup(r => r.ObtenerTodos()).Returns([]);
        _repoRegistrosMock!.Setup(r => r.ObtenerTodos()).Returns([]);

        _repoEventoMock!.Setup(r => r.ObtenerConRelaciones(
            It.IsAny<Expression<Func<Evento, bool>>>(),
            It.IsAny<string>()))
            .Returns([]);

        var resultado = _servicio!.ValidarAcceso(request);

        Assert.IsTrue(resultado.AccesoPermitido);
        Assert.AreEqual("Acceso permitido", resultado.Mensaje);
        Assert.AreEqual("Montaña Rusa", resultado.NombreAtraccion);
        Assert.AreEqual("Juan Perez", resultado.NombreVisitante);
    }

    [TestMethod]
    public void RegistrarIngreso_TodoValido_CreaRegistro()
    {
        var fechaActual = new DateTime(2025, 10, 8);

        var cuenta = Cuenta.Crear("Juan", "Perez", new Email("test@test.com"), "pass123", Rol.Visitante);
        cuenta.AsignarVisitante(fechaActual.AddYears(-25));
        var cuentaId = cuenta.Id;

        var ticket = new Dominio.Ticket
        {
            Codigo = Guid.NewGuid(),
            FechaVisita = fechaActual,
            TipoEntrada = TipoTicket.General,
            CuentaId = cuentaId,
            EsValido = true
        };

        var atraccion = new AtraccionParque("Montaña Rusa", TipoAtraccion.MontañaRusa, 12, 24, "Test") { Id = 1 };
        var codigoTicket = ticket.Codigo;

        _repoTicketsMock!.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Dominio.Ticket, bool>>>()))
            .Returns(ticket);

        _repoAtraccionesMock!.Setup(r => r.Encontrar(It.IsAny<Expression<Func<AtraccionParque, bool>>>()))
            .Returns(atraccion);

        _repoCuentasMock!.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Cuenta, bool>>>()))
            .Returns(cuenta);

        _repoMantenimientosMock!.Setup(r => r.ObtenerTodos()).Returns([]);
        _repoRegistrosMock!.Setup(r => r.ObtenerTodos()).Returns([]);

        _repoEventoMock!.Setup(r => r.ObtenerConRelaciones(
            It.IsAny<Expression<Func<Evento, bool>>>(),
            It.IsAny<string>()))
            .Returns([]);

        _repoTicketsMock.Setup(r => r.Editar(It.IsAny<Dominio.Ticket>()));
        _repoRegistrosMock.Setup(r => r.Agregar(It.IsAny<RegistroVisita>()));

        var resultado = _servicio!.RegistrarIngreso(codigoTicket, 1, cuenta);

        Assert.IsNotNull(resultado);
        Assert.AreEqual(1, resultado.AtraccionId);
        _repoRegistrosMock.Verify(r => r.Agregar(It.IsAny<RegistroVisita>()), Times.Once);
        _repoTicketsMock.Verify(r => r.Editar(It.IsAny<Dominio.Ticket>()), Times.Once);
    }

    [TestMethod]
    public void RegistrarEgreso_RegistroExiste_ActualizaFechaEgreso()
    {
        var codigoTicket = Guid.NewGuid();
        var atraccionId = 1;

        var fechaIngreso = new DateTime(2025, 10, 8, 10, 0, 0);
        var fechaEgreso = new DateTime(2025, 10, 8, 12, 0, 0);

        var registro = new RegistroVisita
        {
            Identificador = codigoTicket,
            AtraccionId = atraccionId,
            FechaIngreso = fechaIngreso
        };

        _servicioFechaHoraMock!.Setup(s => s.ObtenerFechaActual()).Returns(fechaEgreso);
        _repoRegistrosMock!.Setup(r => r.Encontrar(It.IsAny<Expression<Func<RegistroVisita, bool>>>()))
            .Returns(registro);
        _repoRegistrosMock!.Setup(r => r.Editar(It.IsAny<RegistroVisita>()));

        var result = _servicio!.RegistrarEgreso(codigoTicket, atraccionId);

        Assert.IsNotNull(result);
        Assert.IsNotNull(result.FechaEgreso);
        Assert.AreEqual(fechaEgreso, result.FechaEgreso);
        _repoRegistrosMock.Verify(r => r.Editar(It.IsAny<RegistroVisita>()), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void RegistrarEgreso_RegistroNoExiste_LanzaExcepcion()
    {
        var codigoTicket = Guid.NewGuid();

        _repoRegistrosMock!.Setup(r => r.Encontrar(It.IsAny<Expression<Func<RegistroVisita, bool>>>()))
            .Returns((RegistroVisita?)null);

        _servicio!.RegistrarEgreso(codigoTicket, 1);
    }

    [TestMethod]
    public void ValidarAcceso_EdadMenorARequerida_RetornaAccesoDenegado()
    {
        var fechaActual = new DateTime(2025, 10, 8);

        var cuenta = Cuenta.Crear("Niño", "Perez", new Email("nino@test.com"), "pass123", Rol.Visitante);
        cuenta.AsignarVisitante(fechaActual.AddYears(-5));
        var cuentaId = cuenta.Id;

        var ticket = new Dominio.Ticket
        {
            Codigo = Guid.NewGuid(),
            FechaVisita = fechaActual,
            TipoEntrada = TipoTicket.General,
            CuentaId = cuentaId,
            EsValido = true
        };

        var atraccion = new AtraccionParque("Montaña Rusa", TipoAtraccion.MontañaRusa, 12, 24, "Test") { Id = 1 };

        var request = new ValidarAccesoRequest
        {
            CodigoTicket = ticket.Codigo,
            AtraccionId = 1,
            CuentaVisitanteId = cuenta.Id
        };

        _repoTicketsMock!.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Dominio.Ticket, bool>>>()))
            .Returns(ticket);
        _repoAtraccionesMock!.Setup(r => r.Encontrar(It.IsAny<Expression<Func<AtraccionParque, bool>>>()))
            .Returns(atraccion);
        _repoCuentasMock!.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Cuenta, bool>>>()))
            .Returns(cuenta);

        _repoMantenimientosMock!.Setup(r => r.ObtenerTodos()).Returns([]);
        _repoRegistrosMock!.Setup(r => r.ObtenerTodos()).Returns([]);
        _repoEventoMock!.Setup(r => r.ObtenerConRelaciones(It.IsAny<Expression<Func<Evento, bool>>>(), It.IsAny<string>()))
            .Returns([]);

        var resultado = _servicio!.ValidarAcceso(request);

        Assert.IsFalse(resultado.AccesoPermitido);
        Assert.IsTrue(resultado.Mensaje.Contains("Edad mínima requerida"));
        Assert.IsTrue(resultado.Mensaje.Contains("12 años"));
    }

    [TestMethod]
    public void ValidarAcceso_TicketNoPerteneceCuenta_RetornaAccesoDenegado()
    {
        var fechaActual = new DateTime(2025, 10, 8);

        var cuenta = Cuenta.Crear("Juan", "Perez", new Email("test@test.com"), "pass123", Rol.Visitante);
        cuenta.AsignarVisitante(fechaActual.AddYears(-25));

        var ticket = new Dominio.Ticket
        {
            Codigo = Guid.NewGuid(),
            FechaVisita = fechaActual,
            TipoEntrada = TipoTicket.General,
            CuentaId = Guid.NewGuid(), // distinto
            EsValido = true
        };

        var atraccion = new AtraccionParque("Carrusel", TipoAtraccion.Simulador, 0, 30, "Test") { Id = 1 };

        var request = new ValidarAccesoRequest
        {
            CodigoTicket = ticket.Codigo,
            AtraccionId = 1,
            CuentaVisitanteId = cuenta.Id
        };

        _repoTicketsMock!.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Dominio.Ticket, bool>>>()))
            .Returns(ticket);
        _repoAtraccionesMock!.Setup(r => r.Encontrar(It.IsAny<Expression<Func<AtraccionParque, bool>>>()))
            .Returns(atraccion);
        _repoCuentasMock!.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Cuenta, bool>>>()))
            .Returns(cuenta);

        _repoMantenimientosMock!.Setup(r => r.ObtenerTodos()).Returns([]);
        _repoRegistrosMock!.Setup(r => r.ObtenerTodos()).Returns([]);
        _repoEventoMock!.Setup(r => r.ObtenerConRelaciones(It.IsAny<Expression<Func<Evento, bool>>>(), It.IsAny<string>()))
            .Returns([]);

        var resultado = _servicio!.ValidarAcceso(request);

        Assert.IsFalse(resultado.AccesoPermitido);
        Assert.AreEqual("El ticket no pertenece a esta cuenta", resultado.Mensaje);
    }

    [TestMethod]
    public void ValidarAcceso_AtraccionEnMantenimiento_RetornaAccesoDenegado()
    {
        // Caso que reemplaza “incidencia” por mantenimiento preventivo activo
        var ahora = new DateTime(2025, 10, 8, 12, 0, 0);
        _servicioFechaHoraMock!.Setup(s => s.ObtenerFechaActual()).Returns(ahora);

        var cuenta = Cuenta.Crear("Maria", "Lopez", new Email("maria@test.com"), "pass123", Rol.Visitante);
        cuenta.AsignarVisitante(ahora.AddYears(-20));
        var ticket = new Dominio.Ticket
        {
            Codigo = Guid.NewGuid(),
            FechaVisita = ahora.Date,
            TipoEntrada = TipoTicket.General,
            CuentaId = cuenta.Id,
            EsValido = true
        };

        var atraccion = new AtraccionParque("Simulador", TipoAtraccion.Simulador, 8, 12, "Test") { Id = 1 };

        // Mantenimiento activo: ahora entre inicio y fin
        var mant = new MantenimientoPreventivo
        {
            Id = 1,
            AtraccionId = 1,
            Descripcion = "Trabajo programado",
            FechaProgramada = ahora.Date,
            HoraInicio = new TimeSpan(10, 0, 0),
            DuracionEstimada = new TimeSpan(14, 0, 0)
        };

        var request = new ValidarAccesoRequest
        {
            CodigoTicket = ticket.Codigo,
            AtraccionId = 1,
            CuentaVisitanteId = cuenta.Id
        };

        _repoTicketsMock!.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Dominio.Ticket, bool>>>()))
            .Returns(ticket);
        _repoAtraccionesMock!.Setup(r => r.Encontrar(It.IsAny<Expression<Func<AtraccionParque, bool>>>()))
            .Returns(atraccion);
        _repoCuentasMock!.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Cuenta, bool>>>()))
            .Returns(cuenta);

        _repoMantenimientosMock!.Setup(r => r.ObtenerTodos()).Returns([mant]);

        // No bloquear por evento ni aforo
        _repoRegistrosMock!.Setup(r => r.ObtenerTodos()).Returns([]);
        _repoEventoMock!.Setup(r => r.ObtenerConRelaciones(It.IsAny<Expression<Func<Evento, bool>>>(), It.IsAny<string>()))
            .Returns([]);

        var resultado = _servicio!.ValidarAcceso(request);

        Assert.IsFalse(resultado.AccesoPermitido);
        Assert.IsTrue(resultado.Mensaje.Contains("Atracción en mantenimiento preventivo"));
        Assert.IsTrue(resultado.Mensaje.Contains("Inicio:"));
        Assert.IsTrue(resultado.Mensaje.Contains("Fin estimado:"));
    }

    [TestMethod]
    public void ValidarAcceso_EventoEspecialSinEventoId_RetornaAccesoDenegado()
    {
        var fechaActual = new DateTime(2025, 10, 8);
        var cuenta = Cuenta.Crear("Pedro", "Garcia", new Email("pedro@test.com"), "pass123", Rol.Visitante);
        cuenta.AsignarVisitante(fechaActual.AddYears(-30));

        var ticket = new Dominio.Ticket
        {
            Codigo = Guid.NewGuid(),
            FechaVisita = fechaActual,
            TipoEntrada = TipoTicket.EventoEspecial,
            CuentaId = cuenta.Id,
            EventoId = null,
            EsValido = true
        };

        var atraccion = new AtraccionParque("Carrusel", TipoAtraccion.Simulador, 0, 30, "Test") { Id = 1 };

        var request = new ValidarAccesoRequest
        {
            CodigoTicket = ticket.Codigo,
            AtraccionId = 1,
            CuentaVisitanteId = cuenta.Id
        };

        _repoTicketsMock!.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Dominio.Ticket, bool>>>()))
            .Returns(ticket);
        _repoAtraccionesMock!.Setup(r => r.Encontrar(It.IsAny<Expression<Func<AtraccionParque, bool>>>()))
            .Returns(atraccion);
        _repoCuentasMock!.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Cuenta, bool>>>()))
            .Returns(cuenta);

        var resultado = _servicio!.ValidarAcceso(request);

        Assert.IsFalse(resultado.AccesoPermitido);
        Assert.AreEqual("Ticket de evento especial sin evento asociado", resultado.Mensaje);
    }

    [TestMethod]
    public void ValidarAcceso_EventoNoEncontrado_RetornaAccesoDenegado()
    {
        var fechaActual = new DateTime(2025, 10, 8);
        var cuenta = Cuenta.Crear("Ana", "Ramirez", new Email("ana@test.com"), "pass123", Rol.Visitante);
        cuenta.AsignarVisitante(fechaActual.AddYears(-22));

        var ticket = new Dominio.Ticket
        {
            Codigo = Guid.NewGuid(),
            FechaVisita = fechaActual,
            TipoEntrada = TipoTicket.EventoEspecial,
            CuentaId = cuenta.Id,
            EventoId = 999,
            EsValido = true
        };

        var atraccion = new AtraccionParque("Montaña Rusa", TipoAtraccion.MontañaRusa, 12, 24, "Test") { Id = 1 };

        var request = new ValidarAccesoRequest
        {
            CodigoTicket = ticket.Codigo,
            AtraccionId = 1,
            CuentaVisitanteId = cuenta.Id
        };

        _repoTicketsMock!.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Dominio.Ticket, bool>>>()))
            .Returns(ticket);
        _repoAtraccionesMock!.Setup(r => r.Encontrar(It.IsAny<Expression<Func<AtraccionParque, bool>>>()))
            .Returns(atraccion);
        _repoCuentasMock!.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Cuenta, bool>>>()))
            .Returns(cuenta);

        _repoEventoMock!.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Evento, bool>>>()))
            .Returns((Evento?)null);

        var resultado = _servicio!.ValidarAcceso(request);

        Assert.IsFalse(resultado.AccesoPermitido);
        Assert.AreEqual("No se encontró el evento asociado al ticket", resultado.Mensaje);
    }

    [TestMethod]
    public void ObtenerAforoAtraccion_ConVisitantesActuales_RetornaAforoCalculado()
    {
        var atraccionId = 1;
        var atraccion = new AtraccionParque("Montaña Rusa", TipoAtraccion.MontañaRusa, 12, 20, "Test") { Id = atraccionId };

        var registros = new List<RegistroVisita>
        {
            new() { AtraccionId = atraccionId, FechaEgreso = null },
            new() { AtraccionId = atraccionId, FechaEgreso = null },
            new() { AtraccionId = atraccionId, FechaEgreso = null },
            new() { AtraccionId = atraccionId, FechaEgreso = DateTime.Now },
            new() { AtraccionId = 2, FechaEgreso = null }
        };

        _repoAtraccionesMock!.Setup(r => r.Encontrar(It.IsAny<Expression<Func<AtraccionParque, bool>>>()))
            .Returns(atraccion);

        _repoRegistrosMock!.Setup(r => r.ObtenerTodos()).Returns(registros);

        var resultado = _servicio!.ObtenerAforoAtraccion(atraccionId);

        Assert.IsNotNull(resultado);
        Assert.AreEqual(atraccionId, resultado.AtraccionId);
        Assert.AreEqual("Montaña Rusa", resultado.NombreAtraccion);
        Assert.AreEqual(20, resultado.CapacidadTotal);
        Assert.AreEqual(3, resultado.VisitantesActuales);
        Assert.AreEqual(17, resultado.CapacidadRestante);
        Assert.AreEqual(15.0, resultado.PorcentajeOcupacion);
        Assert.IsFalse(resultado.AforoCompleto);
    }

    [TestMethod]
    public void GetRegistrosActivos_SinRegistros_ReturnsListaVacia()
    {
        var usuarioId = Guid.NewGuid();
        var atraccionId = 1;

        var mockRepoRegistros = new Mock<IRepositorio<RegistroVisita>>();
        var mockRepoTickets = new Mock<IRepositorio<Ticket>>();

        mockRepoRegistros.Setup(r => r.ObtenerTodos()).Returns([]);

        var servicio = new ServicioAcceso(
            null!, mockRepoTickets.Object, mockRepoRegistros.Object,
            null!, null!, null!, null!, null!);

        var respuesta = servicio.ObtenerRegistrosActivosPorUsuario(usuarioId, atraccionId);

        Assert.IsNotNull(respuesta);
        Assert.AreEqual(0, respuesta.Count);
    }

    [TestMethod]
    public void GetRegistrosActivos_RegistroSinTicket_ReturnsListaVacia()
    {
        var usuarioId = Guid.NewGuid();
        var atraccionId = 1;

        var registro = new RegistroVisita
        {
            Id = 5,
            AtraccionId = atraccionId,
            Identificador = Guid.NewGuid(),
            FechaIngreso = DateTime.Now,
            FechaEgreso = null
        };

        var mockRepoRegistros = new Mock<IRepositorio<RegistroVisita>>();
        var mockRepoTickets = new Mock<IRepositorio<Ticket>>();

        mockRepoRegistros.Setup(r => r.ObtenerTodos()).Returns([registro]);
        mockRepoTickets.Setup(r => r.Obtener(It.IsAny<Expression<Func<Ticket, bool>>>()))
            .Returns([]);

        var servicio = new ServicioAcceso(
            null!, mockRepoTickets.Object, mockRepoRegistros.Object,
            null!, null!, null!, null!, null!);

        var respuesta = servicio.ObtenerRegistrosActivosPorUsuario(usuarioId, atraccionId);

        Assert.IsNotNull(respuesta);
        Assert.AreEqual(0, respuesta.Count);
    }

    [TestMethod]
    public void GetRegistrosActivos_RegistroTicketOtroUsuario_ReturnsListaVacia()
    {
        var usuarioId = Guid.NewGuid();
        var otroUsuarioId = Guid.NewGuid();
        var atraccionId = 1;

        var registro = new RegistroVisita
        {
            Id = 7,
            AtraccionId = atraccionId,
            Identificador = Guid.NewGuid(),
            FechaIngreso = DateTime.Now,
            FechaEgreso = null
        };

        var ticket = new Ticket
        {
            Codigo = registro.Identificador,
            CuentaId = otroUsuarioId
        };

        var mockRepoRegistros = new Mock<IRepositorio<RegistroVisita>>();
        var mockRepoTickets = new Mock<IRepositorio<Ticket>>();

        mockRepoRegistros.Setup(r => r.ObtenerTodos()).Returns([registro]);
        mockRepoTickets.Setup(r => r.Obtener(It.IsAny<Expression<Func<Ticket, bool>>>()))
            .Returns<Expression<Func<Ticket, bool>>>(expr =>
            {
                var func = expr.Compile();
                return func(ticket) ? [ticket] : [];
            });

        var servicio = new ServicioAcceso(
            null!, mockRepoTickets.Object, mockRepoRegistros.Object,
            null!, null!, null!, null!, null!);

        var respuesta = servicio.ObtenerRegistrosActivosPorUsuario(usuarioId, atraccionId);

        Assert.IsNotNull(respuesta);
        Assert.AreEqual(0, respuesta.Count);
    }

    [TestMethod]
    public void GetRegistrosActivos_Exitoso_ReturnsRegistros()
    {
        var usuarioId = Guid.NewGuid();
        var atraccionId = 1;

        var registro = new RegistroVisita
        {
            Id = 11,
            AtraccionId = atraccionId,
            Identificador = Guid.NewGuid(),
            FechaIngreso = DateTime.Now,
            FechaEgreso = null
        };

        var ticket = new Ticket
        {
            Codigo = registro.Identificador,
            CuentaId = usuarioId
        };

        var mockRepoRegistros = new Mock<IRepositorio<RegistroVisita>>();
        var mockRepoTickets = new Mock<IRepositorio<Ticket>>();

        mockRepoRegistros.Setup(r => r.ObtenerTodos()).Returns([registro]);
        mockRepoTickets.Setup(r => r.Obtener(It.IsAny<Expression<Func<Ticket, bool>>>()))
            .Returns<Expression<Func<Ticket, bool>>>(expr =>
            {
                var func = expr.Compile();
                return func(ticket) ? [ticket] : [];
            });

        var servicio = new ServicioAcceso(
            null!, mockRepoTickets.Object, mockRepoRegistros.Object,
            null!, null!, null!, null!, null!);

        var respuesta = servicio.ObtenerRegistrosActivosPorUsuario(usuarioId, atraccionId);

        Assert.IsNotNull(respuesta);
        Assert.AreEqual(1, respuesta.Count);
        Assert.AreEqual(registro.Id, respuesta[0].Id);
        Assert.AreEqual(registro.Identificador, respuesta[0].Identificador);
        Assert.AreEqual(registro.AtraccionId, respuesta[0].AtraccionId);
        Assert.AreEqual(registro.FechaIngreso, respuesta[0].FechaIngreso);
    }

    [TestMethod]
    public void ValidarDatosModificacion_ValoresWhitespaceEnConstructor_LanzaExcepcionDominio()
    {
        var dto = new ModificarPerfilDto(" ", " ", " ", DateTime.Today.AddYears(-30));
        var ex = Assert.ThrowsException<ExcepcionDominio>(() => ServicioCuenta.ValidarDatosModificacion(dto));
        Assert.IsTrue(ex.Message.Contains("vacío"));
    }

    [TestMethod]
    public void ValidarAcceso_EventoEspecial_AtraccionNoIncluidaEnEvento_RetornaAccesoDenegado()
    {
        var request = new ValidarAccesoRequest
        {
            CodigoTicket = Guid.NewGuid(),
            AtraccionId = 2, // No está incluida
            CuentaVisitanteId = Guid.NewGuid()
        };
        var ticket = new Dominio.Ticket
        {
            Codigo = request.CodigoTicket,
            TipoEntrada = TipoTicket.EventoEspecial,
            EventoId = 101,
            EsValido = true,
            FechaVisita = DateTime.Today
        };
        var atraccion = new AtraccionParque("Montaña Rusa", TipoAtraccion.MontañaRusa, 12, 30, "test") { Id = 2 };
        var cuenta = Cuenta.Crear("Ana", "Test", new Email("ana@test.com"), "pass", Rol.Visitante);
        var evento = new Evento("Mega Fiesta", "Evento multiatracción", DateTime.Today.AddDays(-1), DateTime.Today.AddDays(5), 100, 30, EstadoEvento.Programado)
        {
            Id = 101,
            Atracciones = new List<AtraccionParque>
            {
                new AtraccionParque("Carrusel", TipoAtraccion.Simulador, 0, 20, "child") { Id = 1 }
            }
        };

        var repoTickets = new Mock<IRepositorio<Dominio.Ticket>>();
        var repoAtracciones = new Mock<IRepositorio<AtraccionParque>>();
        var repoCuentas = new Mock<IRepositorio<Cuenta>>();
        var repoEvento = new Mock<IRepositorio<Evento>>();
        var servicioFechaHora = new Mock<IServicioFechaHora>();
        repoTickets.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Dominio.Ticket, bool>>>())).Returns(ticket);
        repoAtracciones.Setup(r => r.Encontrar(It.IsAny<Expression<Func<AtraccionParque, bool>>>())).Returns(atraccion);
        repoCuentas.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Cuenta, bool>>>())).Returns(cuenta);
        repoEvento.Setup(r => r.EncontrarConRelaciones(It.IsAny<Expression<Func<Evento, bool>>>(), "Atracciones")).Returns(evento);
        servicioFechaHora.Setup(s => s.ObtenerFechaActual()).Returns(DateTime.Today);

        var servicioAcceso = new ServicioAcceso(repoAtracciones.Object, repoTickets.Object, null!, null!, repoCuentas.Object, repoEvento.Object, servicioFechaHora.Object, null!);
        var resultado = servicioAcceso.ValidarAcceso(request);

        Assert.IsFalse(resultado.AccesoPermitido);
        Assert.IsTrue(resultado.Mensaje.Contains("no forma parte del evento"));
        Assert.IsTrue(resultado.Mensaje.Contains("Carrusel"));
        Assert.AreEqual(atraccion.Nombre, resultado.NombreAtraccion);
    }

    [TestMethod]
    public void ValidarAcceso_EventoEspecial_FechaActualFueraDelEvento_RetornaAccesoDenegado()
{
    var request = new ValidarAccesoRequest
    {
        CodigoTicket = Guid.NewGuid(),
        AtraccionId = 1,
        CuentaVisitanteId = Guid.NewGuid()
    };

    var hoy = DateTime.Today;
    var ticket = new Dominio.Ticket
    {
        Codigo = request.CodigoTicket,
        TipoEntrada = TipoTicket.EventoEspecial,
        EventoId = 101,
        EsValido = true,
        FechaVisita = hoy // dentro del rango, pero fechaActual se simula fuera
    };
    var atraccion = new AtraccionParque("Carrusel", TipoAtraccion.Simulador, 0, 20, "child") { Id = 1 };
    var cuenta = Cuenta.Crear("Ana", "Test", new Email("ana@test.com"), "pass", Rol.Visitante);

    var evento = new Evento("Mega Fiesta", "Evento multiatracción", hoy.AddDays(2), hoy.AddDays(5), 100, 30, EstadoEvento.Programado) // evento inicia en el futuro
    {
        Id = 101,
        Atracciones = new List<AtraccionParque> { atraccion }
    };

    var repoTickets = new Mock<IRepositorio<Dominio.Ticket>>();
    var repoAtracciones = new Mock<IRepositorio<AtraccionParque>>();
    var repoCuentas = new Mock<IRepositorio<Cuenta>>();
    var repoEvento = new Mock<IRepositorio<Evento>>();
    var servicioFechaHora = new Mock<IServicioFechaHora>();
    repoTickets.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Dominio.Ticket, bool>>>())).Returns(ticket);
    repoAtracciones.Setup(r => r.Encontrar(It.IsAny<Expression<Func<AtraccionParque, bool>>>())).Returns(atraccion);
    repoCuentas.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Cuenta, bool>>>())).Returns(cuenta);
    repoEvento.Setup(r => r.EncontrarConRelaciones(It.IsAny<Expression<Func<Evento, bool>>>(), "Atracciones")).Returns(evento);
    servicioFechaHora.Setup(s => s.ObtenerFechaActual()).Returns(hoy);

    var servicioAcceso = new ServicioAcceso(repoAtracciones.Object, repoTickets.Object, null!, null!, repoCuentas.Object, repoEvento.Object, servicioFechaHora.Object, null!);
    var resultado = servicioAcceso.ValidarAcceso(request);

    Assert.IsFalse(resultado.AccesoPermitido);
    Assert.IsTrue(resultado.Mensaje.Contains("no está activo"));
    Assert.IsTrue(resultado.Mensaje.Contains(evento.Titulo));
}
}
