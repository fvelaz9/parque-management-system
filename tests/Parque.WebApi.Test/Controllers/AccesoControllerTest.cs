using Microsoft.AspNetCore.Mvc;
using Moq;
using Parque.Aplicacion.DTOS;
using Parque.Aplicacion.Servicios.Acceso;
using Parque.Dominio.Atracciones;
using Parque.Dominio.Usuarios;
using Parque.WebApi.Controllers;
namespace Parque.WebApi.Test.Controllers;

[TestClass]
public class AccesoControllerTest
{
    private Mock<IServicioAcceso>? _servicioMock;
    private AccesoController? _controller;

    [TestInitialize]
    public void Initialize()
    {
        _servicioMock = new Mock<IServicioAcceso>(MockBehavior.Strict);
        _controller = new AccesoController(_servicioMock.Object);
    }

    [TestMethod]
    public void ValidarAcceso_AccesoPermitido_ReturnsOk()
    {
        var request = new ValidarAccesoRequest
        {
            CodigoTicket = Guid.NewGuid(),
            AtraccionId = 1
        };
        var response = new ValidarAccesoResponse
        {
            AccesoPermitido = true,
            Mensaje = "Acceso permitido",
            NombreAtraccion = "Montaña Rusa"
        };

        _servicioMock!.Setup(s => s.ValidarAcceso(request)).Returns(response);

        var result = _controller!.ValidarAcceso(request);

        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);
        Assert.AreEqual(response, okResult.Value);
        _servicioMock.VerifyAll();
    }

    [TestMethod]
    public void ValidarAcceso_AccesoDenegado_ReturnsBadRequest()
    {
        var request = new ValidarAccesoRequest
        {
            CodigoTicket = Guid.NewGuid(),
            AtraccionId = 1
        };
        var response = new ValidarAccesoResponse
        {
            AccesoPermitido = false,
            Mensaje = "Ticket no encontrado",
            NombreAtraccion = "Montaña Rusa"
        };

        _servicioMock!.Setup(s => s.ValidarAcceso(request)).Returns(response);

        var result = _controller!.ValidarAcceso(request);

        var badRequestResult = result as BadRequestObjectResult;
        Assert.IsNotNull(badRequestResult);
        Assert.AreEqual(400, badRequestResult.StatusCode);
        Assert.AreEqual(response, badRequestResult.Value);
        _servicioMock.VerifyAll();
    }

    [TestMethod]
    public void RegistrarIngresos_Exitoso_ReturnsOk()
    {
        var codigoTicket = Guid.NewGuid();
        var cuenta = Cuenta.Crear("Juan", "Perez", new Email("test@test.com"), "pass123", Rol.Visitante);
        cuenta.AsignarVisitante(DateTime.Today.AddYears(-25));

        var dto = new ValidarAccesoRequest
        {
            CodigoTicket = codigoTicket,
            AtraccionId = 1,
            CuentaVisitante = cuenta
        };

        var registro = new RegistroVisita
        {
            AtraccionId = 1,
            Identificador = codigoTicket,
            FechaIngreso = DateTime.Now
        };

        _servicioMock!.Setup(s => s.RegistrarIngreso(codigoTicket, 1, cuenta)).Returns(registro);

        var result = _controller!.RegistrarIngresos(1, dto);

        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);
        Assert.AreEqual(registro, okResult.Value);
        _servicioMock.VerifyAll();
    }

    [TestMethod]
    public void RegistrarIngresos_ConExcepcion_ReturnsBadRequest()
    {
        var codigoTicket = Guid.NewGuid();
        var cuenta = Cuenta.Crear("Maria", "Lopez", new Email("maria@test.com"), "pass123", Rol.Visitante);

        var dto = new ValidarAccesoRequest
        {
            CodigoTicket = codigoTicket,
            AtraccionId = 1,
            CuentaVisitante = cuenta
        };

        _servicioMock!.Setup(s => s.RegistrarIngreso(codigoTicket, 1, cuenta))
            .Throws(new ArgumentException("Acceso denegado"));

        var result = _controller!.RegistrarIngresos(1, dto);

        var badRequestResult = result as BadRequestObjectResult;
        Assert.IsNotNull(badRequestResult);
        Assert.AreEqual(400, badRequestResult.StatusCode);
        _servicioMock.VerifyAll();
    }

    [TestMethod]
    public void RegistrarEgreso_Exitoso_ReturnsOk()
    {
        var codigoTicket = Guid.NewGuid();
        var registro = new RegistroVisita
        {
            AtraccionId = 1,
            Identificador = codigoTicket,
            FechaIngreso = DateTime.Now.AddHours(-1),
            FechaEgreso = DateTime.Now
        };

        _servicioMock!.Setup(s => s.RegistrarEgreso(codigoTicket, 1)).Returns(registro);

        var result = _controller!.RegistrarEgreso(1, codigoTicket);

        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);
        _servicioMock.VerifyAll();
    }

    [TestMethod]
    public void RegistrarEgreso_ConExcepcion_ReturnsBadRequest()
    {
        var codigoTicket = Guid.NewGuid();

        _servicioMock!.Setup(s => s.RegistrarEgreso(codigoTicket, 1))
            .Throws(new ArgumentException("No hay ingreso registrado"));

        var result = _controller!.RegistrarEgreso(1, codigoTicket);

        var badRequestResult = result as BadRequestObjectResult;
        Assert.IsNotNull(badRequestResult);
        Assert.AreEqual(400, badRequestResult.StatusCode);
        _servicioMock.VerifyAll();
    }
}
