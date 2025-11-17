using Microsoft.AspNetCore.Mvc;
using Moq;
using Parque.Aplicacion.DTOs.Gamificacion;
using Parque.Aplicacion.Servicios.Gamificacion;
using Parque.WebApi.Controllers.Estrategias;
using Parque.WebApi.Filtros;

namespace Parque.WebApi.Test.Controllers;

[TestClass]
public class EstrategiasControllerTest
{
    private Mock<IServicioPuntuacion>? _serviceMock;
    private EstrategiasController? _controller;

    [TestInitialize]
    public void Initialize()
    {
        _serviceMock = new Mock<IServicioPuntuacion>(MockBehavior.Strict);
        _controller = new EstrategiasController(_serviceMock.Object);
    }

    [TestCleanup]
    public void Cleanup()
    {
        _serviceMock?.VerifyAll();
    }

    #region ObtenerEstrategiasDisponibles Tests

    [TestMethod]
    public void ObtenerEstrategiasDisponibles_RetornaListaDeEstrategias()
    {
        // Arrange
        var estrategiasEsperadas = new List<EstrategiaDto>
        {
            new EstrategiaDto { Nombre = "porAtraccion", EsActiva = true },
            new EstrategiaDto { Nombre = "Combo", EsActiva = false },
            new EstrategiaDto { Nombre = "PorEvento", EsActiva = false }
        };

        _serviceMock!.Setup(s => s.ListarEstrategias())
            .Returns(estrategiasEsperadas);

        // Act
        var result = _controller!.ObtenerEstrategiasDisponibles();

        // Assert
        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);
        var responseDto = okResult.Value as ResponseDto;
        Assert.IsNotNull(responseDto);
        Assert.IsTrue(responseDto.ExecutionSuccessful);
    }

    #endregion

    #region CambiarEstrategiaActiva Tests

    [TestMethod]
    public void CambiarEstrategiaActiva_EstrategiaValida_RetornaOk()
    {
        // Arrange
        var request = new CambiarEstrategiaRequest { NombreEstrategia = "Combo" };
        _serviceMock!.Setup(s => s.CambiarEstrategiaActiva("Combo"));

        // Act
        var result = _controller!.CambiarEstrategiaActiva(request);

        // Assert
        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);
        var responseDto = okResult.Value as ResponseDto;
        Assert.IsNotNull(responseDto);
        Assert.IsTrue(responseDto.ExecutionSuccessful);
    }

    [TestMethod]
    public void CambiarEstrategiaActiva_EstrategiaNoExiste_RetornaBadRequest()
    {
        // Arrange
        var request = new CambiarEstrategiaRequest { NombreEstrategia = "NoExiste" };
        _serviceMock!.Setup(s => s.CambiarEstrategiaActiva("NoExiste"))
            .Throws(new ArgumentException("Estrategia 'NoExiste' no encontrada"));

        // Act
        var result = _controller!.CambiarEstrategiaActiva(request) as BadRequestObjectResult;

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(400, result.StatusCode);

        var responseDto = result.Value as ResponseDto;
        Assert.IsNotNull(responseDto);
        Assert.IsFalse(responseDto.ExecutionSuccessful);
        Assert.AreEqual("Estrategia 'NoExiste' no encontrada", responseDto.Message);
    }

    [TestMethod]
    public void CambiarEstrategiaActiva_NombreVacio_RetornaBadRequest()
    {
        // Arrange
        var request = new CambiarEstrategiaRequest { NombreEstrategia = string.Empty };

        // Act
        var result = _controller!.CambiarEstrategiaActiva(request) as BadRequestObjectResult;

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(400, result.StatusCode);
        var responseDto = result.Value as ResponseDto;
        Assert.IsNotNull(responseDto);
        Assert.IsFalse(responseDto.ExecutionSuccessful);
    }

    #endregion

    #region ObtenerEstrategiaActiva Tests

    [TestMethod]
    public void ObtenerEstrategiaActiva_RetornaNombreEstrategia()
    {
        // Arrange
        var estrategiaActiva = "porAtraccion";
        _serviceMock!.Setup(s => s.ObtenerEstrategiaActiva())
            .Returns(estrategiaActiva);

        // Act
        var result = _controller!.ObtenerEstrategiaActiva();

        // Assert
        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);
        var responseDto = okResult.Value as ResponseDto;
        Assert.IsNotNull(responseDto);
        Assert.IsTrue(responseDto.ExecutionSuccessful);
    }

    #endregion

    #region RecargarPlugins Tests

    [TestMethod]
    public void RecargarPlugins_RetornaEstrategiasRecargadas()
    {
        // Arrange
        var estrategias = new List<EstrategiaDto>
        {
            new EstrategiaDto { Nombre = "porAtraccion", EsActiva = true },
            new EstrategiaDto { Nombre = "PorHora", EsActiva = false, Origen = "Plugin" }
        };

        _serviceMock!.Setup(s => s.RecargarPlugins());
        _serviceMock.Setup(s => s.ListarEstrategias())
            .Returns(estrategias);

        // Act
        var result = _controller!.RecargarPlugins();

        // Assert
        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);
        var responseDto = okResult.Value as ResponseDto;
        Assert.IsNotNull(responseDto);
        Assert.IsTrue(responseDto.ExecutionSuccessful);
    }

    #endregion
}
