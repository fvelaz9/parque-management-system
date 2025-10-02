using Microsoft.AspNetCore.Mvc;
using Moq;
using Parque.Aplicacion.Servicios;
using Parque.WebApi.Controllers;

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
    public void GetCurrentTime_DeberiaRetornarTiempoActual()
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
}
