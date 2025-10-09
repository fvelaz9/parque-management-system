using Microsoft.AspNetCore.Mvc;
using Moq;
using Parque.Aplicacion.DTOs;
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
    public void CrearIncidencia_Exitoso_ReturnsCreated()
    {
        // Arrange
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

        // Act
        var result = _controller!.CrearIncidencia(request);

        // Assert
        var createdResult = result as CreatedAtActionResult;
        Assert.IsNotNull(createdResult);
        Assert.AreEqual(201, createdResult.StatusCode);
        Assert.AreEqual(nameof(_controller.VerificarDisponibilidad), createdResult.ActionName);
        Assert.AreEqual(incidencia, createdResult.Value);
        _servicioMock.VerifyAll();
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CrearIncidencia_ConExcepcion_LanzaExcepcion()
    {
        // Arrange
        var request = new CrearIncidenciaRequest
        {
            AtraccionId = 999,
            Descripcion = "Falla",
            FechaResolucionEstimada = DateTime.Now.AddDays(1)
        };

        _servicioMock!.Setup(s => s.CrearIncidencia(request))
            .Throws(new ArgumentException("Atraccion no encontrada"));

        // Act
        _controller!.CrearIncidencia(request);

        // Assert
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

        var value = okResult.Value;
        Assert.IsNotNull(value);

        var atraccionIdProp = value.GetType().GetProperty("atraccionId")?.GetValue(value);
        var disponibleProp = value.GetType().GetProperty("disponible")?.GetValue(value);

        Assert.AreEqual(atraccionId, atraccionIdProp);
        Assert.AreEqual(true, disponibleProp);

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

        var value = okResult.Value;
        Assert.IsNotNull(value);

        var atraccionIdProp = value.GetType().GetProperty("atraccionId")?.GetValue(value);
        var disponibleProp = value.GetType().GetProperty("disponible")?.GetValue(value);

        Assert.AreEqual(atraccionId, atraccionIdProp);
        Assert.AreEqual(false, disponibleProp);

        _servicioMock.VerifyAll();
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void VerificarDisponibilidad_ConExcepcion_LanzaExcepcion()
    {
        // Arrange
        var atraccionId = 999;

        _servicioMock!.Setup(s => s.EstaDisponible(atraccionId))
            .Throws(new ArgumentException("Atraccion no encontrada"));

        // Act
        _controller!.VerificarDisponibilidad(atraccionId);

        // Assert
    }

    [TestMethod]
    public void ResolverIncidencia_Exitoso_ReturnsNoContent()
    {
        // Arrange
        var incidenciaId = 1;
        _servicioMock!.Setup(s => s.ResolverIncidencia(incidenciaId));

        // Act
        var result = _controller!.ResolverIncidencia(incidenciaId);

        // Assert
        Assert.IsInstanceOfType(result, typeof(NoContentResult));
        var noContentResult = result as NoContentResult;
        Assert.IsNotNull(noContentResult);
        Assert.AreEqual(204, noContentResult.StatusCode);
        _servicioMock.VerifyAll();
    }
}
