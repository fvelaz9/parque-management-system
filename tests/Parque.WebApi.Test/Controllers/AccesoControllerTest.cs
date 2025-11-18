using Microsoft.AspNetCore.Mvc;
using Moq;
using Parque.Aplicacion.DTOs;
using Parque.Aplicacion.Servicios;
using Parque.Aplicacion.Servicios.Acceso;
using Parque.Dominio.Atracciones;
using Parque.Dominio.Usuarios;
using Parque.WebApi.Controllers.Acceso;
using Parque.WebApi.Controllers.Acceso.Models;

namespace Parque.WebApi.Test.Controllers;

[TestClass]
public class AccesoControllerTest
{
    private Mock<IServicioAcceso>? _servicioAccesoMock;
    private Mock<IServicioCuenta>? _servicioCuentaMock;
    private AccesoController? _controller;

    [TestInitialize]
    public void Initialize()
    {
        _servicioAccesoMock = new Mock<IServicioAcceso>(MockBehavior.Strict);
        _servicioCuentaMock = new Mock<IServicioCuenta>(MockBehavior.Strict);
        _controller = new AccesoController(_servicioAccesoMock.Object, _servicioCuentaMock.Object);
    }

    #region ValidarAcceso Tests

    [TestMethod]
    public void ValidarAcceso_AccesoPermitido_ReturnsOk()
    {
        var request = new ValidarAccesoRequest
        {
            CodigoTicket = Guid.NewGuid(),
            AtraccionId = 1,
            CuentaVisitanteId = Guid.NewGuid()
        };
        var response = new ValidarAccesoResponse
        {
            AccesoPermitido = true,
            Mensaje = "Acceso permitido",
            NombreAtraccion = "Montaña Rusa"
        };

        _servicioAccesoMock!.Setup(s => s.ValidarAcceso(It.IsAny<ValidarAccesoRequest>())).Returns(response);

        var result = _controller!.ValidarAcceso(request);

        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);
        Assert.AreEqual(response, okResult.Value);
        _servicioAccesoMock.VerifyAll();
    }

    [TestMethod]
    public void ValidarAcceso_AccesoDenegado_ReturnsBadRequest()
    {
        var request = new ValidarAccesoRequest
        {
            CodigoTicket = Guid.NewGuid(),
            AtraccionId = 1,
            CuentaVisitanteId = Guid.NewGuid()
        };
        var response = new ValidarAccesoResponse
        {
            AccesoPermitido = false,
            Mensaje = "Ticket no encontrado",
            NombreAtraccion = "Montaña Rusa"
        };

        _servicioAccesoMock!.Setup(s => s.ValidarAcceso(It.IsAny<ValidarAccesoRequest>())).Returns(response);

        var result = _controller!.ValidarAcceso(request);

        var badRequestResult = result as BadRequestObjectResult;
        Assert.IsNotNull(badRequestResult);
        Assert.AreEqual(400, badRequestResult.StatusCode);
        Assert.AreEqual(response, badRequestResult.Value);
        _servicioAccesoMock.VerifyAll();
    }

    [TestMethod]
    public void ValidarAcceso_CapacidadExcedida_ReturnsBadRequest()
    {
        var request = new ValidarAccesoRequest
        {
            CodigoTicket = Guid.NewGuid(),
            AtraccionId = 1,
            CuentaVisitanteId = Guid.NewGuid()
        };
        var response = new ValidarAccesoResponse
        {
            AccesoPermitido = false,
            Mensaje = "Capacidad de la atracción excedida",
            NombreAtraccion = "Montaña Rusa"
        };

        _servicioAccesoMock!.Setup(s => s.ValidarAcceso(It.IsAny<ValidarAccesoRequest>())).Returns(response);

        var result = _controller!.ValidarAcceso(request);

        var badRequestResult = result as BadRequestObjectResult;
        Assert.IsNotNull(badRequestResult);
        Assert.AreEqual(400, badRequestResult.StatusCode);
        _servicioAccesoMock.VerifyAll();
    }

    #endregion

    #region RegistrarIngreso Tests

    [TestMethod]
    public void RegistrarIngreso_Exitoso_ReturnsOk()
    {
        var codigoTicket = Guid.NewGuid();
        var cuentaVisitanteId = Guid.NewGuid();
        var email = new Email("test@test.com");
        var cuenta = Cuenta.Crear("Juan", "Perez", email, "pass123", Rol.Visitante);
        cuenta.AsignarVisitante(DateTime.Today.AddYears(-25));

        var request = new RegistrarIngresoRequest
        {
            CodigoTicket = codigoTicket,
            CuentaVisitanteId = cuentaVisitanteId
        };

        var registro = new RegistroVisita
        {
            AtraccionId = 1,
            Identificador = codigoTicket,
            FechaIngreso = DateTime.Now
        };

        _servicioCuentaMock!.Setup(s => s.ObtenerCuenta(cuentaVisitanteId)).Returns(cuenta);
        _servicioAccesoMock!.Setup(s => s.RegistrarIngreso(codigoTicket, 1, cuenta)).Returns(registro);

        var result = _controller!.RegistrarIngreso(1, request);

        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);

        var value = okResult.Value;
        Assert.IsNotNull(value);

        var mensajeProp = value.GetType().GetProperty("mensaje")?.GetValue(value);
        var registroProp = value.GetType().GetProperty("registro")?.GetValue(value) as RegistroVisita;

        Assert.AreEqual("Ingreso registrado exitosamente", mensajeProp);
        Assert.AreEqual(registro, registroProp);

        _servicioCuentaMock.VerifyAll();
        _servicioAccesoMock.VerifyAll();
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void RegistrarIngreso_TicketNoEncontrado_LanzaExcepcion()
    {
        var codigoTicket = Guid.NewGuid();
        var cuentaVisitanteId = Guid.NewGuid();
        var email = new Email("maria@test.com");
        var cuenta = Cuenta.Crear("Maria", "Lopez", email, "pass123", Rol.Visitante);

        var request = new RegistrarIngresoRequest
        {
            CodigoTicket = codigoTicket,
            CuentaVisitanteId = cuentaVisitanteId
        };

        _servicioCuentaMock!.Setup(s => s.ObtenerCuenta(cuentaVisitanteId)).Returns(cuenta);
        _servicioAccesoMock!.Setup(s => s.RegistrarIngreso(codigoTicket, 1, cuenta))
            .Throws(new ArgumentException("Ticket no encontrado"));

        _controller!.RegistrarIngreso(1, request);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void RegistrarIngreso_AtraccionNoEncontrada_LanzaExcepcion()
    {
        var codigoTicket = Guid.NewGuid();
        var cuentaVisitanteId = Guid.NewGuid();
        var email = new Email("carlos@test.com");
        var cuenta = Cuenta.Crear("Carlos", "Gonzalez", email, "pass123", Rol.Visitante);

        var request = new RegistrarIngresoRequest
        {
            CodigoTicket = codigoTicket,
            CuentaVisitanteId = cuentaVisitanteId
        };

        _servicioCuentaMock!.Setup(s => s.ObtenerCuenta(cuentaVisitanteId)).Returns(cuenta);
        _servicioAccesoMock!.Setup(s => s.RegistrarIngreso(codigoTicket, 1, cuenta))
            .Throws(new InvalidOperationException("Atracción no encontrada"));

        _controller!.RegistrarIngreso(1, request);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void RegistrarIngreso_CuentaNoEncontrada_LanzaExcepcion()
    {
        var codigoTicket = Guid.NewGuid();
        var cuentaVisitanteId = Guid.NewGuid();

        var request = new RegistrarIngresoRequest
        {
            CodigoTicket = codigoTicket,
            CuentaVisitanteId = cuentaVisitanteId
        };

        _servicioCuentaMock!.Setup(s => s.ObtenerCuenta(cuentaVisitanteId))
            .Throws(new ArgumentException("Cuenta no encontrada"));

        _controller!.RegistrarIngreso(1, request);
    }

    #endregion

    #region RegistrarEgreso Tests

    [TestMethod]
    public void RegistrarEgreso_Exitoso_ReturnsOkConTiempoVisita()
    {
        var codigoTicket = Guid.NewGuid();
        var request = new RegistrarEgresoRequest
        {
            CodigoTicket = codigoTicket
        };

        var fechaIngreso = DateTime.Now.AddHours(-1);
        var fechaEgreso = DateTime.Now;

        var registro = new RegistroVisita
        {
            AtraccionId = 1,
            Identificador = codigoTicket,
            FechaIngreso = fechaIngreso,
            FechaEgreso = fechaEgreso
        };

        _servicioAccesoMock!.Setup(s => s.RegistrarEgreso(codigoTicket, 1)).Returns(registro);

        var result = _controller!.RegistrarEgreso(1, request);

        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);

        var value = okResult.Value;
        Assert.IsNotNull(value);

        var mensajeProp = value.GetType().GetProperty("mensaje")?.GetValue(value);
        var tiempoVisitaProp = value.GetType().GetProperty("tiempoVisitaMinutos")?.GetValue(value);

        Assert.AreEqual("Egreso registrado exitosamente", mensajeProp);
        Assert.IsNotNull(tiempoVisitaProp);
        Assert.IsInstanceOfType(tiempoVisitaProp, typeof(double));
        Assert.IsTrue((double)tiempoVisitaProp! >= 59 && (double)tiempoVisitaProp <= 61);

        _servicioAccesoMock.VerifyAll();
    }

    [TestMethod]
    public void RegistrarEgreso_SinEgreso_RetornaZeroMinutos()
    {
        var codigoTicket = Guid.NewGuid();
        var request = new RegistrarEgresoRequest
        {
            CodigoTicket = codigoTicket
        };

        var registro = new RegistroVisita
        {
            AtraccionId = 1,
            Identificador = codigoTicket,
            FechaIngreso = DateTime.Now,
            FechaEgreso = null
        };

        _servicioAccesoMock!.Setup(s => s.RegistrarEgreso(codigoTicket, 1)).Returns(registro);

        var result = _controller!.RegistrarEgreso(1, request);

        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);

        var value = okResult.Value;
        var tiempoVisitaProp = value.GetType().GetProperty("tiempoVisitaMinutos")?.GetValue(value);
        Assert.IsNotNull(tiempoVisitaProp);
        Assert.AreEqual(0, Convert.ToInt32(tiempoVisitaProp));
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void RegistrarEgreso_TicketNoEncontrado_LanzaExcepcion()
    {
        var codigoTicket = Guid.NewGuid();
        var request = new RegistrarEgresoRequest
        {
            CodigoTicket = codigoTicket
        };

        _servicioAccesoMock!.Setup(s => s.RegistrarEgreso(codigoTicket, 1))
            .Throws(new ArgumentException("No hay ingreso registrado para este ticket"));

        _controller!.RegistrarEgreso(1, request);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void RegistrarEgreso_YaHayEgreso_LanzaExcepcion()
    {
        var codigoTicket = Guid.NewGuid();
        var request = new RegistrarEgresoRequest
        {
            CodigoTicket = codigoTicket
        };

        _servicioAccesoMock!.Setup(s => s.RegistrarEgreso(codigoTicket, 1))
            .Throws(new InvalidOperationException("El ticket ya tiene egreso registrado"));

        _controller!.RegistrarEgreso(1, request);
    }

    #endregion

    #region ObtenerAforo Tests

    [TestMethod]
    public void ObtenerAforo_Exitoso_ReturnsOkConAforo()
    {
        var atraccionId = 1;
        var aforo = new AforoResponse
        {
            AtraccionId = atraccionId,
            NombreAtraccion = "Montaña Rusa",
            CapacidadTotal = 50,
            VisitantesActuales = 23,
            CapacidadRestante = 27,
            PorcentajeOcupacion = 46.0,
            AforoCompleto = false
        };

        _servicioAccesoMock!.Setup(s => s.ObtenerAforoAtraccion(atraccionId)).Returns(aforo);

        var result = _controller!.ObtenerAforo(atraccionId);

        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);
        Assert.AreEqual(aforo, okResult.Value);
        _servicioAccesoMock.VerifyAll();
    }

    [TestMethod]
    public void ObtenerAforo_AtraccionVacia_ReturnsOkConCeroPersonas()
    {
        var atraccionId = 1;
        var aforo = new AforoResponse
        {
            AtraccionId = atraccionId,
            NombreAtraccion = "Montaña Rusa",
            CapacidadTotal = 50,
            VisitantesActuales = 0,
            CapacidadRestante = 50,
            PorcentajeOcupacion = 0.0,
            AforoCompleto = false
        };

        _servicioAccesoMock!.Setup(s => s.ObtenerAforoAtraccion(atraccionId)).Returns(aforo);

        var result = _controller!.ObtenerAforo(atraccionId);

        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);

        var value = okResult.Value;
        Assert.IsNotNull(value);
        Assert.AreEqual(aforo, value);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void ObtenerAforo_AtraccionNoExiste_LanzaExcepcion()
    {
        var atraccionId = 999;

        _servicioAccesoMock!.Setup(s => s.ObtenerAforoAtraccion(atraccionId))
            .Throws(new ArgumentException("Atracción no encontrada"));

        _controller!.ObtenerAforo(atraccionId);
    }

    #endregion

    #region GetRegistrosActivos Tests

    [TestMethod]
    public void GetRegistrosActivos_ConRegistros_ReturnsOkConLista()
    {
        var usuarioId = Guid.NewGuid();
        var atraccionId = 1;

        var registros = new List<RegistroVisitaDto>
        {
            new RegistroVisitaDto { AtraccionId = atraccionId, Identificador = Guid.NewGuid(), FechaIngreso = DateTime.Now.AddHours(-2) },
            new RegistroVisitaDto { AtraccionId = atraccionId, Identificador = Guid.NewGuid(), FechaIngreso = DateTime.Now.AddHours(-1) }
        };

        _servicioAccesoMock!.Setup(s => s.ObtenerRegistrosActivosPorUsuario(usuarioId, atraccionId)).Returns(registros);

        var result = _controller!.GetRegistrosActivos(usuarioId, atraccionId);

        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);

        var value = okResult.Value as List<RegistroVisitaDto>;
        Assert.IsNotNull(value);
        Assert.AreEqual(2, value.Count);
        _servicioAccesoMock.VerifyAll();
    }

    [TestMethod]
    public void GetRegistrosActivos_SinRegistros_ReturnsOkConListaVacia()
    {
        var usuarioId = Guid.NewGuid();
        var atraccionId = 1;

        _servicioAccesoMock!.Setup(s => s.ObtenerRegistrosActivosPorUsuario(usuarioId, atraccionId))
            .Returns([]);

        var result = _controller!.GetRegistrosActivos(usuarioId, atraccionId);

        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);

        var value = okResult.Value as List<RegistroVisitaDto>;
        Assert.IsNotNull(value);
        Assert.AreEqual(0, value.Count);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void GetRegistrosActivos_UsuarioInvalido_LanzaExcepcion()
    {
        var usuarioId = Guid.Empty;
        var atraccionId = 1;

        _servicioAccesoMock!.Setup(s => s.ObtenerRegistrosActivosPorUsuario(usuarioId, atraccionId))
            .Throws(new ArgumentException("ID de usuario inválido"));

        _controller!.GetRegistrosActivos(usuarioId, atraccionId);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void GetRegistrosActivos_AtraccionInvalida_LanzaExcepcion()
    {
        var usuarioId = Guid.NewGuid();
        var atraccionId = -1;

        _servicioAccesoMock!.Setup(s => s.ObtenerRegistrosActivosPorUsuario(usuarioId, atraccionId))
            .Throws(new ArgumentException("ID de atracción inválido"));

        _controller!.GetRegistrosActivos(usuarioId, atraccionId);
    }

    #endregion
}
