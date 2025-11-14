using System.Dynamic;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Parque.Aplicacion.DTOs.Gamificacion;
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
    public void ObtenerRankingDiario_TopNegativo_RetornaBadRequest()
    {
        // Arrange
        var topInvalido = -5;

        // Act
        var result = _controller!.ObtenerRankingDiario(null, topInvalido);

        // Assert
        Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        var badResult = result as BadRequestObjectResult;
        Assert.IsNotNull(badResult);
        Assert.AreEqual(400, badResult.StatusCode);
    }

    #endregion

    #region ListarEstrategias Tests

    [TestMethod]
    public void ListarEstrategias_RetornaListaDeEstrategias()
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
        var result = _controller!.ListarEstrategias();

        // Assert
        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);
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
    public void CambiarEstrategiaActiva_EstrategiaNoExiste_LanzaExcepcion()
    {
        // Arrange
        var request = new CambiarEstrategiaRequest { NombreEstrategia = "NoExiste" };
        _serviceMock!.Setup(s => s.CambiarEstrategiaActiva("NoExiste"))
            .Throws(new ArgumentException("Estrategia 'NoExiste' no encontrada"));

        // Act & Assert
        Assert.ThrowsException<ArgumentException>(() => _controller!.CambiarEstrategiaActiva(request));
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
    }

    #endregion

    [TestMethod]
    public void ObtenerHistorialPuntuaciones_VisitanteValido_RetornaHistorial()
    {
        // Arrange
        var visitanteId = Guid.NewGuid();
        var historialEsperado = new List<HistorialPuntuacionDto>
        {
            new() { EstrategiaActiva = "X", OrigenPuntos = "A", Puntos = 10, FechaHora = DateTime.UtcNow },
            new() { EstrategiaActiva = "Y", OrigenPuntos = "B", Puntos = 20, FechaHora = DateTime.UtcNow }
        };

        _serviceMock!
            .Setup(s => s.ObtenerHistorialVisitante(visitanteId))
            .Returns(historialEsperado);

        // Act
        var result = _controller!.ObtenerHistorialPuntuaciones(visitanteId);

        // Assert
        Assert.IsNotNull(result.Value);
        Assert.AreEqual(2, result.Value.Count);
    }

    [TestMethod]
    public void ObtenerRankingDiario_ValidaContenidoRetornado()
    {
        // Arrange
        var ranking = new List<RankingVisitanteDto>
        {
            new() { VisitanteId = Guid.NewGuid(), PuntosDiarios = 60, PuntosTotales = 200, Posicion = 1 }
        };

        _serviceMock!
            .Setup(s => s.ObtenerRankingDiario(null, 10))
            .Returns(ranking);

        // Act
        var result = _controller!.ObtenerRankingDiario(null, 10) as OkObjectResult;

        // Assert
        Assert.IsNotNull(result);

        var json = System.Text.Json.JsonSerializer.Serialize(result!.Value);
        dynamic payload = Newtonsoft.Json.JsonConvert.DeserializeObject<ExpandoObject>(json)!;

        Assert.AreEqual(DateTime.Today.Date, (DateTime)payload.fecha);

        Assert.AreEqual(1, (int)payload.totalVisitantes);
        Assert.AreEqual(60, (int)payload.ranking[0].PuntosDiarios);
    }
}
