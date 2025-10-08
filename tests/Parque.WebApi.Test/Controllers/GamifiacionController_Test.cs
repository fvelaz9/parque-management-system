using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Parque.Aplicacion.DTOS.Gamificacion;
using Parque.Aplicacion.Servicios.Gamificacion;
using Parque.WebApi.Controllers;

namespace Parque.WebApi.Test.Controllers;

[TestClass]
public class GamifiacionController_Test
{
     private Mock<IServicioPuntuacion>? _serviceMock;
    private GamificacionController? _controller;

    [TestInitialize]
    public void Initialize()
    {
        _serviceMock = new Mock<IServicioPuntuacion>(MockBehavior.Strict);
        _controller = new GamificacionController(_serviceMock.Object);
    }

    [TestCleanup]
    public void Cleanup()
    {
        _serviceMock?.VerifyAll();
    }

    #region CalcularPuntos Tests

    [TestMethod]
    public void CalcularPuntos_RegistroValido_RetornaOk()
    {
        // Arrange
        var registroVisitaId = 1;
        _serviceMock!.Setup(s => s.CalcularYRegistrarPuntos(registroVisitaId));

        // Act
        var result = _controller!.CalcularPuntos(registroVisitaId);

        // Assert
        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);
    }

    [TestMethod]
    public void CalcularPuntos_RegistroNoExiste_RetornaBadRequest()
    {
        // Arrange
        var registroVisitaId = 999;
        _serviceMock!.Setup(s => s.CalcularYRegistrarPuntos(registroVisitaId))
            .Throws(new InvalidOperationException("Registro de visita con ID 999 no encontrado"));

        // Act
        var result = _controller!.CalcularPuntos(registroVisitaId);

        // Assert
        Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        var badResult = result as BadRequestObjectResult;
        Assert.IsNotNull(badResult);
        Assert.AreEqual(400, badResult.StatusCode);
    }

    [TestMethod]
    public void CalcularPuntos_ErrorInterno_RetornaStatusCode500()
    {
        // Arrange
        var registroVisitaId = 1;
        _serviceMock!.Setup(s => s.CalcularYRegistrarPuntos(registroVisitaId))
            .Throws(new Exception("Error de base de datos"));

        // Act
        var result = _controller!.CalcularPuntos(registroVisitaId);

        // Assert
        Assert.IsInstanceOfType(result, typeof(ObjectResult));
        var errorResult = result as ObjectResult;
        Assert.IsNotNull(errorResult);
        Assert.AreEqual(500, errorResult.StatusCode);
    }

    #endregion

    #region ObtenerRankingDiario Tests

    [TestMethod]
    public void ObtenerRankingDiario_SinParametros_RetornaRankingConDefaults()
    {
        // Arrange
        var rankingEsperado = new List<RankingVisitanteDto>
        {
            new RankingVisitanteDto { VisitanteId = Guid.NewGuid(), PuntosDiarios = 50, PuntosTotales = 150, Posicion = 1 },
            new RankingVisitanteDto { VisitanteId = Guid.NewGuid(), PuntosDiarios = 40, PuntosTotales = 120, Posicion = 2 }
        };

        _serviceMock!.Setup(s => s.ObtenerRankingDiario(null, 10))
            .Returns(rankingEsperado);

        // Act
        var result = _controller!.ObtenerRankingDiario(null, 10);

        // Assert
        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);
    }

    [TestMethod]
    public void ObtenerRankingDiario_ConFechaYTop_RetornaRankingEspecifico()
    {
        // Arrange
        var fecha = new DateTime(2025, 10, 8);
        var top = 5;
        var rankingEsperado = new List<RankingVisitanteDto>
        {
            new RankingVisitanteDto { VisitanteId = Guid.NewGuid(), PuntosDiarios = 100, PuntosTotales = 500, Posicion = 1 }
        };

        _serviceMock!.Setup(s => s.ObtenerRankingDiario(fecha, top))
            .Returns(rankingEsperado);

        // Act
        var result = _controller!.ObtenerRankingDiario(fecha, top);

        // Assert
        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);
    }

    [TestMethod]
    public void ObtenerRankingDiario_TopMenorOIgualACero_RetornaBadRequest()
    {
        // Arrange
        var topInvalido = 0;

        // Act
        var result = _controller!.ObtenerRankingDiario(null, topInvalido);

        // Assert
        Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        var badResult = result as BadRequestObjectResult;
        Assert.IsNotNull(badResult);
        Assert.AreEqual(400, badResult.StatusCode);
    }

    [TestMethod]
    public void ObtenerRankingDiario_ErrorInterno_RetornaStatusCode500()
    {
        // Arrange
        _serviceMock!.Setup(s => s.ObtenerRankingDiario(null, 10))
            .Throws(new Exception("Error al acceder a la base de datos"));

        // Act
        var result = _controller!.ObtenerRankingDiario(null, 10);

        // Assert
        Assert.IsInstanceOfType(result, typeof(ObjectResult));
        var errorResult = result as ObjectResult;
        Assert.IsNotNull(errorResult);
        Assert.AreEqual(500, errorResult.StatusCode);
    }

    #endregion

    #region ListarEstrategias Tests

    [TestMethod]
    public void ListarEstrategias_RetornaListaDeEstrategias()
    {
        // Arrange
        var estrategiasEsperadas = new List<EstrategiaDto>
        {
            new EstrategiaDto { Nombre = "PorAtraccion", EsActiva = true },
            new EstrategiaDto { Nombre = "Combo", EsActiva = false },
            new EstrategiaDto { Nombre = "PorEvento", EsActiva = false }
        };

        _serviceMock!.Setup(s => s.ListarEstrategias())
            .Returns(estrategiasEsperadas);

        // Act
        var result = _controller!.ListarEstrategias();

        // Assert
        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);
    }

    [TestMethod]
    public void ListarEstrategias_ErrorInterno_RetornaStatusCode500()
    {
        // Arrange
        _serviceMock!.Setup(s => s.ListarEstrategias())
            .Throws(new Exception("Error al listar estrategias"));

        // Act
        var result = _controller!.ListarEstrategias();

        // Assert
        Assert.IsInstanceOfType(result, typeof(ObjectResult));
        var errorResult = result as ObjectResult;
        Assert.IsNotNull(errorResult);
        Assert.AreEqual(500, errorResult.StatusCode);
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
    }

    [TestMethod]
    public void CambiarEstrategiaActiva_NombreVacio_RetornaBadRequest()
    {
        var request = new CambiarEstrategiaRequest { NombreEstrategia = string.Empty };

        var result = _controller!.CambiarEstrategiaActiva(request);

        Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        var badResult = result as BadRequestObjectResult;
        Assert.IsNotNull(badResult);
        Assert.AreEqual(400, badResult.StatusCode);
    }

    [TestMethod]
    public void CambiarEstrategiaActiva_RequestNull_RetornaBadRequest()
    {
        // Arrange
        CambiarEstrategiaRequest? request = null;

        // Act
        var result = _controller!.CambiarEstrategiaActiva(request!);

        // Assert
        Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
    }

    [TestMethod]
    public void CambiarEstrategiaActiva_EstrategiaNoExiste_RetornaBadRequest()
    {
        // Arrange
        var request = new CambiarEstrategiaRequest { NombreEstrategia = "NoExiste" };
        _serviceMock!.Setup(s => s.CambiarEstrategiaActiva("NoExiste"))
            .Throws(new ArgumentException("Estrategia 'NoExiste' no encontrada"));

        // Act
        var result = _controller!.CambiarEstrategiaActiva(request);

        // Assert
        Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        var badResult = result as BadRequestObjectResult;
        Assert.IsNotNull(badResult);
        Assert.AreEqual(400, badResult.StatusCode);
    }

    [TestMethod]
    public void CambiarEstrategiaActiva_ErrorInterno_RetornaStatusCode500()
    {
        // Arrange
        var request = new CambiarEstrategiaRequest { NombreEstrategia = "Combo" };
        _serviceMock!.Setup(s => s.CambiarEstrategiaActiva("Combo"))
            .Throws(new Exception("Error de base de datos"));

        // Act
        var result = _controller!.CambiarEstrategiaActiva(request);

        // Assert
        Assert.IsInstanceOfType(result, typeof(ObjectResult));
        var errorResult = result as ObjectResult;
        Assert.IsNotNull(errorResult);
        Assert.AreEqual(500, errorResult.StatusCode);
    }

    #endregion

    #region ObtenerEstrategiaActiva Tests

    [TestMethod]
    public void ObtenerEstrategiaActiva_RetornaNombreEstrategia()
    {
        var estrategiaActiva = "PorAtraccion";
        _serviceMock!.Setup(s => s.ObtenerEstrategiaActiva())
            .Returns(estrategiaActiva);

        // Act
        var result = _controller!.ObtenerEstrategiaActiva();

        // Assert
        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);
    }

    [TestMethod]
    public void ObtenerEstrategiaActiva_ErrorInterno_RetornaStatusCode500()
    {
        // Arrange
        _serviceMock!.Setup(s => s.ObtenerEstrategiaActiva())
            .Throws(new Exception("Error al obtener estrategia"));

        // Act
        var result = _controller!.ObtenerEstrategiaActiva();

        // Assert
        Assert.IsInstanceOfType(result, typeof(ObjectResult));
        var errorResult = result as ObjectResult;
        Assert.IsNotNull(errorResult);
        Assert.AreEqual(500, errorResult.StatusCode);
    }

    #endregion
}
