using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Parque.Aplicacion.DTOS;
using Parque.Aplicacion.Servicios.Incidencias;
using Parque.Dominio;
using Parque.WebApi.Controllers;
namespace Parque.WebApi.Test.Controllers;

[TestClass]
public class IncidenciasControllerTest
{
    private Mock<IServicioIncidencia>? _servicioMock;
    private IncidenciasController? _controller;

    [TestInitialize]
    public void Initialize()
    {
        _servicioMock = new Mock<IServicioIncidencia>(MockBehavior.Strict);
        _controller = new IncidenciasController(_servicioMock.Object);
    }

    [TestMethod]
    public void CrearIncidencia_Exitoso_ReturnsOk()
    {
        var request = new CrearIncidenciaRequest
        {
            AtraccionId = 1,
            Descripcion = "Falla en el motor",
            FechaResolucionEstimada = DateTime.Now.AddDays(2)
        };
        var incidencia = new Incidencia
        {
            AtraccionId = 1,
            Descripcion = "Falla en el motor",
            FechaReporte = DateTime.Now,
            FechaResolucionEstimada = DateTime.Now.AddDays(2)
        };

        _servicioMock!.Setup(s => s.CrearIncidencia(request)).Returns(incidencia);

        var result = _controller!.CrearIncidencia(request);

        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);
        Assert.AreEqual(incidencia, okResult.Value);
        _servicioMock.VerifyAll();
    }

    [TestMethod]
    public void CrearIncidencia_ConExcepcion_ReturnsBadRequest()
    {
        var request = new CrearIncidenciaRequest
        {
            AtraccionId = 999,
            Descripcion = "Falla",
            FechaResolucionEstimada = DateTime.Now.AddDays(1)
        };

        _servicioMock!.Setup(s => s.CrearIncidencia(request))
            .Throws(new ArgumentException("Atraccion no encontrada"));

        var result = _controller!.CrearIncidencia(request);

        var badRequestResult = result as BadRequestObjectResult;
        Assert.IsNotNull(badRequestResult);
        Assert.AreEqual(400, badRequestResult.StatusCode);
        _servicioMock.VerifyAll();
    }

    [TestMethod]
    public void VerificarDisponibilidad_AtraccionDisponible_ReturnsOk()
    {
        var atraccionId = 1;

        _servicioMock!.Setup(s => s.EstaDisponible(atraccionId)).Returns(true);

        var result = _controller!.VerificarDisponibilidad(atraccionId);

        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);
        _servicioMock.VerifyAll();
    }

    [TestMethod]
    public void VerificarDisponibilidad_AtraccionNoDisponible_ReturnsOk()
    {
        var atraccionId = 1;

        _servicioMock!.Setup(s => s.EstaDisponible(atraccionId)).Returns(false);

        var result = _controller!.VerificarDisponibilidad(atraccionId);

        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);
        _servicioMock.VerifyAll();
    }

    [TestMethod]
    public void VerificarDisponibilidad_ConExcepcion_ReturnsBadRequest()
    {
        var atraccionId = 999;

        _servicioMock!.Setup(s => s.EstaDisponible(atraccionId))
            .Throws(new ArgumentException("Atraccion no encontrada"));

        var result = _controller!.VerificarDisponibilidad(atraccionId);

        var badRequestResult = result as BadRequestObjectResult;
        Assert.IsNotNull(badRequestResult);
        Assert.AreEqual(400, badRequestResult.StatusCode);
        _servicioMock.VerifyAll();
    }
}
