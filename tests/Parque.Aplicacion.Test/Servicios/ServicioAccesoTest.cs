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
    public void ValidarAcceso_WhenTicketNotFound_ShouldReturnFalse()
    {
        var cuentaId = Guid.NewGuid();
        var request = new ValidarAccesoRequest
        {
            CodigoTicket = Guid.NewGuid(),
            AtraccionId = 1,
            CuentaVisitante = null
        };
        _repoTicketsMock!.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Dominio.Ticket, bool>>>()))
                       .Returns((Dominio.Ticket?)null);

        var result = _servicio!.ValidarAcceso(request);

        Assert.IsFalse(result.AccesoPermitido);
        Assert.AreEqual("El ticket no fue encontrado", result.Mensaje);
    }

    [TestMethod]
    public void ValidarAcceso_WhenAtraccionNotFound_ShouldReturnFalse()
    {
        var cuentaId = Guid.NewGuid();
        var cuentaVisitante = Cuenta.Crear("Juan", "Pérez", new Email("test@test.com"), "password123", Rol.Visitante);
        var request = new ValidarAccesoRequest
        {
            CodigoTicket = Guid.NewGuid(),
            AtraccionId = 1,
            CuentaVisitante = cuentaVisitante
        };
        var fechaActual = new DateTime(2025, 10, 8, 12, 0, 0);
        var fechaVisita = new DateTime(2025, 10, 8, 14, 0, 0);
        var ticket = new Dominio.Ticket(cuentaId, fechaVisita, 1, TipoTicket.General, fechaActual);

        _repoTicketsMock!.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Dominio.Ticket, bool>>>()))
            .Returns(ticket);
        _repoAtraccionesMock!.Setup(r => r.Encontrar(It.IsAny<Expression<Func<AtraccionParque, bool>>>()))
            .Returns((AtraccionParque?)null);

        var result = _servicio!.ValidarAcceso(request);

        Assert.IsFalse(result.AccesoPermitido);
        Assert.AreEqual("Atracción no encontrada", result.Mensaje);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void RegistrarIngreso_WhenInvalidAccess_ShouldThrowException()
    {
        var cuentaId = Guid.NewGuid();
        var codigoTicket = Guid.NewGuid();
        var atraccionId = 1;
        var cuentaVisitante = Cuenta.Crear("Juan", "Pérez", new Email("test@test.com"), "password123", Rol.Visitante);

        _repoTicketsMock!.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Dominio.Ticket, bool>>>()))
                   .Returns((Dominio.Ticket?)null);

        _servicio!.RegistrarIngreso(codigoTicket, atraccionId, cuentaVisitante);
    }

    [TestMethod]
    public void RegistrarEgreso_WhenValidRegistro_ShouldUpdateFechaEgreso()
    {
        var codigoTicket = Guid.NewGuid();
        var atraccionId = 1;
        var fechaIngreso = new DateTime(2025, 10, 8, 10, 0, 0);
        var fechaEgreso = new DateTime(2025, 10, 8, 12, 0, 0);

        var registro = new RegistroVisita
        {
            AtraccionId = atraccionId,
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
    public void RegistrarEgreso_WhenRegistroNotFound_ShouldThrowException()
    {
        var codigoTicket = Guid.NewGuid();
        var atraccionId = 1;

        _repoRegistrosMock!.Setup(r => r.Encontrar(It.IsAny<Expression<Func<RegistroVisita, bool>>>()))
                         .Returns((RegistroVisita?)null);

        _servicio!.RegistrarEgreso(codigoTicket, atraccionId);
    }
}
