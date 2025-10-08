using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Parque.Aplicacion.Servicios.Gamificacion;
using Parque.Dominio;
using Parque.Dominio.Atracciones;
using Parque.Dominio.Gamificacion;
using Parque.Dominio.Usuarios;
using Parque.Infraestructura.Repositorios;

namespace Parque.Aplicacion.Test.Servicios.Puntuacion;

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
    private Mock<IEstrategiaPuntuacion>? _estrategiaMock;
    private ServicioPuntuacion? _servicio;

    [TestInitialize]
    public void Setup()
    {
        _repoAtraccionesMock = new Mock<IRepositorio<AtraccionParque>>();
        _repoTicketsMock = new Mock<IRepositorio<Dominio.Ticket>>();
        _repoRegistrosMock = new Mock<IRepositorio<RegistroVisita>>();
        _repoPuntuacionesMock = new Mock<IRepositorio<PuntuacionVisitante>>();
        _repoCuentasMock = new Mock<IRepositorio<Cuenta>>();
        _repoEventosMock = new Mock<IRepositorio<Evento>>();
        _repoConfiguracionMock = new Mock<IRepositorio<ConfiguracionEstrategia>>();
        _estrategiaMock = new Mock<IEstrategiaPuntuacion>();

        var estrategias = new List<IEstrategiaPuntuacion> { _estrategiaMock.Object };

        _servicio = new ServicioPuntuacion(
            _repoAtraccionesMock.Object,
            _repoTicketsMock.Object,
            _repoRegistrosMock.Object,
            _repoPuntuacionesMock.Object,
            _repoCuentasMock.Object,
            _repoEventosMock.Object,
            _repoConfiguracionMock.Object,
            estrategias);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void CalcularYRegistrarPuntos_WhenRegistroNotFound_ShouldThrowException()
    {
        var registroVisitaId = 1;

        _repoRegistrosMock.Setup(r => r.Encontrar(It.IsAny<Expression<Func<RegistroVisita, bool>>>()))
                         .Returns((RegistroVisita)null);

        _servicio.CalcularYRegistrarPuntos(registroVisitaId);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void CalcularYRegistrarPuntos_WhenAtraccionNotFound_ShouldThrowException()
    {
        var registroVisitaId = 1;
        var registro = new RegistroVisita { Id = registroVisitaId, AtraccionId = 1 };

        _repoRegistrosMock.Setup(r => r.Encontrar(It.IsAny<Expression<Func<RegistroVisita, bool>>>()))
                         .Returns(registro);
        _repoAtraccionesMock.Setup(r => r.Encontrar(It.IsAny<Expression<Func<AtraccionParque, bool>>>()))
                           .Returns((AtraccionParque)null);

        _servicio.CalcularYRegistrarPuntos(registroVisitaId);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void CalcularYRegistrarPuntos_WhenTicketNotFound_ShouldThrowException()
    {
        var registroVisitaId = 1;
        var registro = new RegistroVisita { Id = registroVisitaId, AtraccionId = 1, Identificador = Guid.NewGuid() };
        var atraccion = new AtraccionParque("Montaña Rusa", TipoAtraccion.Simulador, 12, 20, "Rápida");

        _repoRegistrosMock.Setup(r => r.Encontrar(It.IsAny<Expression<Func<RegistroVisita, bool>>>()))
                         .Returns(registro);
        _repoAtraccionesMock.Setup(r => r.Encontrar(It.IsAny<Expression<Func<AtraccionParque, bool>>>()))
                           .Returns(atraccion);
        _repoTicketsMock.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Dominio.Ticket, bool>>>()))
                       .Returns((Dominio.Ticket)null);

        _servicio.CalcularYRegistrarPuntos(registroVisitaId);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void CalcularYRegistrarPuntos_WhenCuentaNotFound_ShouldThrowException()
    {
        var registroVisitaId = 1;
        var cuentaId = Guid.NewGuid();
        var registro = new RegistroVisita { Id = registroVisitaId, AtraccionId = 1, Identificador = Guid.NewGuid() };
        var atraccion = new AtraccionParque("Montaña Rusa", TipoAtraccion.Simulador, 12, 20, "Rápida");
        var ticket = new Dominio.Ticket(cuentaId, DateTime.Today.AddDays(1), 1, TipoTicket.General);

        _repoRegistrosMock.Setup(r => r.Encontrar(It.IsAny<Expression<Func<RegistroVisita, bool>>>()))
                         .Returns(registro);
        _repoAtraccionesMock.Setup(r => r.Encontrar(It.IsAny<Expression<Func<AtraccionParque, bool>>>()))
                           .Returns(atraccion);
        _repoTicketsMock.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Dominio.Ticket, bool>>>()))
                       .Returns(ticket);
        _repoCuentasMock.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Cuenta, bool>>>()))
                       .Returns((Cuenta)null);

        _servicio.CalcularYRegistrarPuntos(registroVisitaId);
    }
}
