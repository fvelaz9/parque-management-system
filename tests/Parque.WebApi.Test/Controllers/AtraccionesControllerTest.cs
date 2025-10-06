using Microsoft.AspNetCore.Mvc;
using Moq;
using Parque.Aplicacion.DTOS;
using Parque.Aplicacion.Servicios.Atracciones;
using Parque.Dominio.Atracciones;
using Parque.WebApi.Controllers;

namespace Parque.WebApi.Test.Controllers;

[TestClass]
public class AtraccionesController_Test
{
    private Mock<IServicioAtracciones>? _serviceMock;
    private AtraccionesController? _controller;

    [TestInitialize]
    public void Initialize()
    {
        _serviceMock = new Mock<IServicioAtracciones>(MockBehavior.Strict);
        _controller = new AtraccionesController(_serviceMock.Object);
    }

    [TestMethod]
    public void GetAll_WhenAtraccionesExist_ShouldReturnOk()
    {
        // Arrange
        var atracciones = new List<AtraccionParque>
        {
            new AtraccionParque("Montaña Rusa", TipoAtraccion.MontañaRusa, 12, 20, "Rápida"),
            new AtraccionParque("Carrusel", TipoAtraccion.MontañaRusa, 0, 30, "Clásico")
        };
        _serviceMock!.Setup(s => s.ListarAtracciones()).Returns(atracciones);

        // Act
        var result = _controller!.GetAll();

        // Assert
        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        var okResult = result as OkObjectResult;
        Assert.AreEqual(atracciones, okResult?.Value);
        _serviceMock.VerifyAll();
    }

    [TestMethod]
    public void GetById_WhenAtraccionExists_ShouldReturnOk()
    {
        // Arrange
        var atraccion = new AtraccionParque("Montaña Rusa", TipoAtraccion.MontañaRusa, 12, 20, "Rápida") { Id = 1 };
        _serviceMock!.Setup(s => s.BuscarAtraccion(1)).Returns(atraccion);

        // Act
        var result = _controller!.GetById(1);

        // Assert
        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        var okResult = result as OkObjectResult;
        Assert.AreEqual(atraccion, okResult?.Value);
        _serviceMock.VerifyAll();
    }

    [TestMethod]
    public void GetById_WhenAtraccionNotExists_ShouldReturnNotFound()
    {
        // Arrange
        _serviceMock!.Setup(s => s.BuscarAtraccion(1)).Returns((AtraccionParque?)null);

        // Act
        var result = _controller!.GetById(1);

        // Assert
        Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        _serviceMock.VerifyAll();
    }

    [TestMethod]
    public void Create_WhenValidAtraccion_ShouldReturnCreated()
    {
        // Arrange
        var atraccionRequest = new AtraccionParque("Montaña Rusa", TipoAtraccion.MontañaRusa, 12, 20, "Rápida");
        var atraccionCreada = new AtraccionParque("Montaña Rusa", TipoAtraccion.MontañaRusa, 12, 20, "Rápida") { Id = 1 };
        _serviceMock!.Setup(s => s.CrearAtraccion(
            atraccionRequest.Nombre,
            atraccionRequest.Tipo,
            atraccionRequest.EdadMinima,
            atraccionRequest.Capacidad,
            atraccionRequest.Descripcion))
            .Returns(atraccionCreada);

        // Act
        var result = _controller!.Create(atraccionRequest);

        // Assert
        Assert.IsInstanceOfType(result, typeof(CreatedAtActionResult));
        var createdResult = result as CreatedAtActionResult;
        Assert.AreEqual(nameof(AtraccionesController.GetById), createdResult?.ActionName);
        Assert.AreEqual(atraccionCreada, createdResult?.Value);
        _serviceMock.VerifyAll();
    }

    [TestMethod]
    public void Update_WhenValidAtraccion_ShouldReturnNoContent()
    {
        // Arrange
        var atraccionRequest = new AtraccionParque("Nueva Montaña", TipoAtraccion.MontañaRusa, 14, 25, "Actualizada");
        _serviceMock!.Setup(s => s.ModificarAtraccion(
            1,
            atraccionRequest.Nombre,
            atraccionRequest.Tipo,
            atraccionRequest.EdadMinima,
            atraccionRequest.Capacidad,
            atraccionRequest.Descripcion));

        // Act
        var result = _controller!.Update(1, atraccionRequest);

        // Assert
        Assert.IsInstanceOfType(result, typeof(NoContentResult));
        _serviceMock.VerifyAll();
    }

    [TestMethod]
    public void Delete_WhenValidId_ShouldReturnNoContent()
    {
        // Arrange
        _serviceMock!.Setup(s => s.EliminarAtraccion(1));

        // Act
        var result = _controller!.Delete(1);

        // Assert
        Assert.IsInstanceOfType(result, typeof(NoContentResult));
        _serviceMock.VerifyAll();
    }

    [TestMethod]
    public void ObtenerAforo_AtraccionExiste_DeberiaRetornarOk()
    {
        // Arrange
        var aforoDto = new AforoAtraccionDto
        {
            AtraccionId = 1,
            NombreAtraccion = "Montaña Rusa",
            AforoActual = 10,
            CapacidadMaxima = 24,
            Disponible = 14
        };

        _serviceMock!.Setup(s => s.ObtenerAforoActual(1)).Returns(aforoDto);

        // Act
        var result = _controller!.ObtenerAforo(1);

        // Assert
        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(aforoDto, okResult.Value);
        _serviceMock.Verify(s => s.ObtenerAforoActual(1), Times.Once);
    }

    [TestMethod]
    public void ObtenerAforo_AtraccionNoExiste_DeberiaRetornarNotFound()
    {
        // Arrange
        _serviceMock!.Setup(s => s.ObtenerAforoActual(999))
            .Throws(new ArgumentException("Atracción no encontrada"));

        // Act
        var result = _controller!.ObtenerAforo(999);

        // Assert
        Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
        var notFoundResult = result as NotFoundObjectResult;
        Assert.IsNotNull(notFoundResult);
        _serviceMock.Verify(s => s.ObtenerAforoActual(999), Times.Once);
    }
}
