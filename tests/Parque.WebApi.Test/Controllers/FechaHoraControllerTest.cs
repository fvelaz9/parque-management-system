using Microsoft.AspNetCore.Mvc;
using Moq;
using Parque.Aplicacion.Servicios;
using Parque.WebApi.Controllers.FechaHora;
using Parque.WebApi.Controllers.FechaHora.Models;

namespace Parque.WebApi.Test.Controllers;

[TestClass]
public class FechaHoraControllerTest
{
    private Mock<IServicioFechaHora>? _mockServicioFecha;
    private FechaHoraController? _controller;

    [TestInitialize]
    public void Setup()
    {
        _mockServicioFecha = new Mock<IServicioFechaHora>();
        _controller = new FechaHoraController(_mockServicioFecha.Object);
    }

    [TestMethod]
    public void ObtenerFechaActual_DeberiaRetornarTiempoActual()
    {
        // Arrange
        var expectedTime = new DateTime(2025, 9, 2, 14, 45, 0);
        _mockServicioFecha!.Setup(s => s.ObtenerFechaActual()).Returns(expectedTime);
        _mockServicioFecha.Setup(s => s.UsaFechaPersonalizada()).Returns(true);

        // Act
        var result = _controller!.ObtenerFechaActual() as OkObjectResult;

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);
    }

    [TestMethod]
    public void ConfigurarFecha_ConFormatoValido_DeberiaConfigurar()
    {
        // Arrange
        var request = new ConfigurarFechaRequest { FechaHora = "2025-09-02T14:45" };

        // Act
        var result = _controller!.ConfigurarFecha(request) as OkObjectResult;

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);
        _mockServicioFecha!.Verify(s => s.ConfigurarFecha(It.IsAny<DateTime>()), Times.Once);
    }

    [TestMethod]
    public void ConfigurarFecha_ConFormatoInvalido_DeberiaRetornarBadRequest()
    {
        // Arrange
        var request = new ConfigurarFechaRequest { FechaHora = "Formato Invalido" };

        // Act
        var result = _controller!.ConfigurarFecha(request) as BadRequestObjectResult;

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(400, result.StatusCode);
        _mockServicioFecha!.Verify(s => s.ConfigurarFecha(It.IsAny<DateTime>()), Times.Never);
    }
}
