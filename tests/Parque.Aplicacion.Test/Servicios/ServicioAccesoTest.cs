using System.Linq.Expressions;
using Moq;
using Parque.Aplicacion.DTOS;
using Parque.Aplicacion.Servicios;
using Parque.Aplicacion.Servicios.Acceso;
using Parque.Dominio;
using Parque.Dominio.Atracciones;
using Parque.Dominio.Usuarios;
using Parque.Infraestructura.Repositorios;

namespace Parque.Aplicacion.Test.Servicios;

[TestClass]
public class ServicioAccesoTest
{
    private Mock<IRepositorio<AtraccionParque>>? _repoAtraccionesMock;
    private Mock<IRepositorio<Dominio.Ticket>>? _repoTicketsMock;
    private Mock<IRepositorio<RegistroVisita>>? _repoRegistrosMock;
    private Mock<IRepositorio<Incidencia>>? _repoIncidenciasMock;
    private Mock<IRepositorio<Cuenta>>? _repoCuentasMock;
    private Mock<IRepositorio<Evento>>? _repoEventoMock;
    private Mock<IServicioFechaHora>? _servicioFechaHoraMock;
    private ServicioAcceso? _servicio;

    [TestInitialize]
    public void Setup()
    {
        _repoAtraccionesMock = new Mock<IRepositorio<AtraccionParque>>();
        _repoTicketsMock = new Mock<IRepositorio<Dominio.Ticket>>();
        _repoRegistrosMock = new Mock<IRepositorio<RegistroVisita>>();
        _repoIncidenciasMock = new Mock<IRepositorio<Incidencia>>();
        _repoCuentasMock = new Mock<IRepositorio<Cuenta>>();
        _repoEventoMock = new Mock<IRepositorio<Evento>>();
        _servicioFechaHoraMock = new Mock<IServicioFechaHora>();
        _servicioFechaHoraMock.Setup(s => s.ObtenerFechaActual())
            .Returns(new DateTime(2025, 10, 8, 12, 0, 0));

        _servicio = new ServicioAcceso(
            _repoAtraccionesMock.Object,
            _repoTicketsMock.Object,
            _repoRegistrosMock.Object,
            _repoIncidenciasMock.Object,
            _repoCuentasMock.Object,
            _repoEventoMock.Object,
            _servicioFechaHoraMock.Object);
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
        var ticket = new Dominio.Ticket { Codigo = Guid.NewGuid(), FechaVisita = DateTime.Today };
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
        var fechaVisita = DateTime.Today.AddDays(-1);
        var ticket = new Dominio.Ticket { Codigo = Guid.NewGuid(), FechaVisita = fechaVisita };
        var atraccion = new AtraccionParque("Montaña Rusa", TipoAtraccion.MontañaRusa, 12, 24, "Test") { Id = 1 };
        var request = new ValidarAccesoRequest
        {
            CodigoTicket = ticket.Codigo,
            AtraccionId = 1
        };

        _repoTicketsMock!.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Dominio.Ticket, bool>>>()))
            .Returns(ticket);
        _repoAtraccionesMock!.Setup(r => r.Encontrar(It.IsAny<Expression<Func<AtraccionParque, bool>>>()))
            .Returns(atraccion);

        var resultado = _servicio!.ValidarAcceso(request);

        Assert.IsFalse(resultado.AccesoPermitido);
        Assert.IsTrue(resultado.Mensaje.Contains("Ticket válido solo para"));
    }

    [TestMethod]
    public void ValidarAcceso_CuentaNoEncontrada_RetornaAccesoDenegado()
    {
        var ticket = new Dominio.Ticket { Codigo = Guid.NewGuid(), FechaVisita = DateTime.Today, TipoEntrada = TipoTicket.General };
        var atraccion = new AtraccionParque("Carrusel", TipoAtraccion.Simulador, 0, 30, "Test") { Id = 1 };
        var cuenta = Cuenta.Crear("Juan", "Perez", new Email("test@test.com"), "pass123", Rol.Visitante);
        var request = new ValidarAccesoRequest
        {
            CodigoTicket = ticket.Codigo,
            AtraccionId = 1,
            CuentaVisitante = cuenta
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
        var cuenta = Cuenta.Crear("Juan", "Perez", new Email("test@test.com"), "pass123", Rol.Visitante);
        cuenta.AsignarVisitante(DateTime.Today.AddYears(-25));
        var cuentaId = cuenta.Id;

        var ticket = new Dominio.Ticket
        {
            Codigo = Guid.NewGuid(),
            FechaVisita = DateTime.Today,
            TipoEntrada = TipoTicket.General,
            CuentaId = cuentaId
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
            CuentaVisitante = cuenta
        };

        _repoTicketsMock!.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Dominio.Ticket, bool>>>()))
            .Returns(ticket);
        _repoAtraccionesMock!.Setup(r => r.Encontrar(It.IsAny<Expression<Func<AtraccionParque, bool>>>()))
            .Returns(atraccion);
        _repoCuentasMock!.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Cuenta, bool>>>()))
            .Returns(cuenta);
        _repoIncidenciasMock!.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Incidencia, bool>>>()))
            .Returns((Incidencia?)null);
        _repoRegistrosMock!.Setup(r => r.ObtenerTodos()).Returns(registros);

        var resultado = _servicio!.ValidarAcceso(request);

        Assert.IsFalse(resultado.AccesoPermitido);
        Assert.IsTrue(resultado.Mensaje.Contains("Aforo completo"));
    }

    [TestMethod]
    public void ValidarAcceso_TodoValido_RetornaAccesoPermitido()
    {
        var cuenta = Cuenta.Crear("Juan", "Perez", new Email("test@test.com"), "pass123", Rol.Visitante);
        cuenta.AsignarVisitante(DateTime.Today.AddYears(-25));
        var cuentaId = cuenta.Id;

        var ticket = new Dominio.Ticket
        {
            Codigo = Guid.NewGuid(),
            FechaVisita = DateTime.Today,
            TipoEntrada = TipoTicket.General,
            CuentaId = cuentaId
        };
        var atraccion = new AtraccionParque("Montaña Rusa", TipoAtraccion.MontañaRusa, 12, 24, "Test") { Id = 1 };

        var request = new ValidarAccesoRequest
        {
            CodigoTicket = ticket.Codigo,
            AtraccionId = 1,
            CuentaVisitante = cuenta
        };
        var fechaActual = new DateTime(2025, 10, 8, 12, 0, 0);
        var fechaVisita = new DateTime(2025, 10, 8, 14, 0, 0);
        var ticket = new Dominio.Ticket(cuentaId, fechaVisita, 1, TipoTicket.General, fechaActual);

        _repoTicketsMock!.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Dominio.Ticket, bool>>>()))
            .Returns(ticket);
        _repoAtraccionesMock!.Setup(r => r.Encontrar(It.IsAny<Expression<Func<AtraccionParque, bool>>>()))
            .Returns((AtraccionParque?)null);

        var result = _servicio!.ValidarAcceso(request);

        _repoTicketsMock!.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Dominio.Ticket, bool>>>()))
            .Returns(ticket);
        _repoAtraccionesMock!.Setup(r => r.Encontrar(It.IsAny<Expression<Func<AtraccionParque, bool>>>()))
            .Returns(atraccion);
        _repoCuentasMock!.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Cuenta, bool>>>()))
            .Returns(cuenta);
        _repoIncidenciasMock!.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Incidencia, bool>>>()))
            .Returns((Incidencia?)null);
        _repoRegistrosMock!.Setup(r => r.ObtenerTodos()).Returns(new List<RegistroVisita>());

        var resultado = _servicio!.RegistrarIngreso(codigoTicket, 1, cuenta);

        Assert.IsNotNull(resultado);
        Assert.AreEqual(1, resultado.AtraccionId);
        _repoRegistrosMock.Verify(r => r.Agregar(It.IsAny<RegistroVisita>()), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void RegistrarIngreso_AccesoDenegado_LanzaExcepcion()
    {
        var codigoTicket = Guid.NewGuid();
        var cuenta = Cuenta.Crear("Pedro", "Garcia", new Email("pedro@test.com"), "pass123", Rol.Visitante);

        _repoTicketsMock!.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Dominio.Ticket, bool>>>()))
                   .Returns((Dominio.Ticket?)null);

        _servicio!.RegistrarIngreso(codigoTicket, atraccionId, cuentaVisitante);
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
        var cuenta = Cuenta.Crear("Niño", "Perez", new Email("nino@test.com"), "pass123", Rol.Visitante);
        cuenta.AsignarVisitante(DateTime.Today.AddYears(-5));
        var cuentaId = cuenta.Id;

        var ticket = new Dominio.Ticket
        {
            Codigo = Guid.NewGuid(),
            FechaVisita = DateTime.Today,
            TipoEntrada = TipoTicket.General,
            CuentaId = cuentaId
        };
        var atraccion = new AtraccionParque("Montaña Rusa", TipoAtraccion.MontañaRusa, 12, 24, "Test") { Id = 1 };

        var request = new ValidarAccesoRequest
        {
            CodigoTicket = ticket.Codigo,
            AtraccionId = 1,
            CuentaVisitante = cuenta
        };

        _repoTicketsMock!.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Dominio.Ticket, bool>>>()))
            .Returns(ticket);
        _repoAtraccionesMock!.Setup(r => r.Encontrar(It.IsAny<Expression<Func<AtraccionParque, bool>>>()))
            .Returns(atraccion);
        _repoCuentasMock!.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Cuenta, bool>>>()))
            .Returns(cuenta);

        var resultado = _servicio!.ValidarAcceso(request);

        Assert.IsFalse(resultado.AccesoPermitido);
        Assert.IsTrue(resultado.Mensaje.Contains("Edad mínima requerida"));
    }

    [TestMethod]
    public void ValidarAcceso_TicketNoPerteneceCuenta_RetornaAccesoDenegado()
    {
        var cuenta = Cuenta.Crear("Juan", "Perez", new Email("test@test.com"), "pass123", Rol.Visitante);
        cuenta.AsignarVisitante(DateTime.Today.AddYears(-25));

        var ticket = new Dominio.Ticket
        {
            Codigo = Guid.NewGuid(),
            FechaVisita = DateTime.Today,
            TipoEntrada = TipoTicket.General,
            CuentaId = Guid.NewGuid()
        };
        var atraccion = new AtraccionParque("Carrusel", TipoAtraccion.Simulador, 0, 30, "Test") { Id = 1 };

        var request = new ValidarAccesoRequest
        {
            CodigoTicket = ticket.Codigo,
            AtraccionId = 1,
            CuentaVisitante = cuenta
        };

        _repoTicketsMock!.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Dominio.Ticket, bool>>>()))
            .Returns(ticket);
        _repoAtraccionesMock!.Setup(r => r.Encontrar(It.IsAny<Expression<Func<AtraccionParque, bool>>>()))
            .Returns(atraccion);
        _repoCuentasMock!.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Cuenta, bool>>>()))
            .Returns(cuenta);

        var resultado = _servicio!.ValidarAcceso(request);

        Assert.IsFalse(resultado.AccesoPermitido);
        Assert.AreEqual("El ticket no pertenece a esta cuenta", resultado.Mensaje);
    }

    [TestMethod]
    public void ValidarAcceso_AtraccionConIncidencia_RetornaAccesoDenegado()
    {
        var cuenta = Cuenta.Crear("Maria", "Lopez", new Email("maria@test.com"), "pass123", Rol.Visitante);
        cuenta.AsignarVisitante(DateTime.Today.AddYears(-20));
        var cuentaId = cuenta.Id;

        var ticket = new Dominio.Ticket
        {
            Codigo = Guid.NewGuid(),
            FechaVisita = DateTime.Today,
            TipoEntrada = TipoTicket.General,
            CuentaId = cuentaId
        };
        var atraccion = new AtraccionParque("Simulador", TipoAtraccion.Simulador, 8, 12, "Test") { Id = 1 };
        var incidencia = new Incidencia
        {
            AtraccionId = 1,
            FechaReporte = DateTime.Now.AddHours(-2),
            FechaResolucionEstimada = DateTime.Now.AddHours(2),
            Descripcion = "En mantenimiento"
        };

        var request = new ValidarAccesoRequest
        {
            CodigoTicket = ticket.Codigo,
            AtraccionId = 1,
            CuentaVisitante = cuenta
        };

        _repoTicketsMock!.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Dominio.Ticket, bool>>>()))
            .Returns(ticket);
        _repoAtraccionesMock!.Setup(r => r.Encontrar(It.IsAny<Expression<Func<AtraccionParque, bool>>>()))
            .Returns(atraccion);
        _repoCuentasMock!.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Cuenta, bool>>>()))
            .Returns(cuenta);
        _repoIncidenciasMock!.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Incidencia, bool>>>()))
            .Returns(incidencia);

        var resultado = _servicio!.ValidarAcceso(request);

        Assert.IsFalse(resultado.AccesoPermitido);
        Assert.AreEqual("Atracción temporalmente fuera de servicio", resultado.Mensaje);
    }

    [TestMethod]
    public void ValidarAcceso_EventoEspecialSinEventoId_RetornaAccesoDenegado()
    {
        var cuenta = Cuenta.Crear("Pedro", "Garcia", new Email("pedro@test.com"), "pass123", Rol.Visitante);
        cuenta.AsignarVisitante(DateTime.Today.AddYears(-30));
        var cuentaId = cuenta.Id;

        var ticket = new Dominio.Ticket
        {
            Codigo = Guid.NewGuid(),
            FechaVisita = DateTime.Today,
            TipoEntrada = TipoTicket.EventoEspecial,
            CuentaId = cuentaId,
            EventoId = null
        };
        var atraccion = new AtraccionParque("Carrusel", TipoAtraccion.Simulador, 0, 30, "Test") { Id = 1 };

        var request = new ValidarAccesoRequest
        {
            CodigoTicket = ticket.Codigo,
            AtraccionId = 1,
            CuentaVisitante = cuenta
        };

        _repoTicketsMock!.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Dominio.Ticket, bool>>>()))
            .Returns(ticket);
        _repoAtraccionesMock!.Setup(r => r.Encontrar(It.IsAny<Expression<Func<AtraccionParque, bool>>>()))
            .Returns(atraccion);

        var resultado = _servicio!.ValidarAcceso(request);

        Assert.IsFalse(resultado.AccesoPermitido);
        Assert.AreEqual("Ticket de evento especial sin evento asociado", resultado.Mensaje);
    }

    [TestMethod]
    public void ValidarAcceso_EventoNoEncontrado_RetornaAccesoDenegado()
    {
        var cuenta = Cuenta.Crear("Ana", "Ramirez", new Email("ana@test.com"), "pass123", Rol.Visitante);
        cuenta.AsignarVisitante(DateTime.Today.AddYears(-22));
        var cuentaId = cuenta.Id;

        var ticket = new Dominio.Ticket
        {
            Codigo = Guid.NewGuid(),
            FechaVisita = DateTime.Today,
            TipoEntrada = TipoTicket.EventoEspecial,
            CuentaId = cuentaId,
            EventoId = 999
        };
        var atraccion = new AtraccionParque("Montaña Rusa", TipoAtraccion.MontañaRusa, 12, 24, "Test") { Id = 1 };

        var request = new ValidarAccesoRequest
        {
            CodigoTicket = ticket.Codigo,
            AtraccionId = 1,
            CuentaVisitante = cuenta
        };

        _repoTicketsMock!.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Dominio.Ticket, bool>>>()))
            .Returns(ticket);
        _repoAtraccionesMock!.Setup(r => r.Encontrar(It.IsAny<Expression<Func<AtraccionParque, bool>>>()))
            .Returns(atraccion);
        _repoEventoMock!.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Evento, bool>>>()))
            .Returns((Evento?)null);

        var resultado = _servicio!.ValidarAcceso(request);

        Assert.IsFalse(resultado.AccesoPermitido);
        Assert.AreEqual("No se encontró el evento asociado al ticket", resultado.Mensaje);
    }
}
