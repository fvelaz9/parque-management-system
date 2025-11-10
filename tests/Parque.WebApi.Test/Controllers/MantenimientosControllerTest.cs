using Microsoft.AspNetCore.Mvc;
using Moq;
using Parque.Aplicacion.DTOs;
using Parque.Aplicacion.Servicios.Mantenimiento;
using Parque.Dominio;
using Parque.WebApi.Controllers;

namespace Parque.WebApi.Test.Controllers;

[TestClass]
public class MantenimientosControllerTest
{
    private Mock<IServicioMantenimiento> _mockServicio = null!;
    private MantenimientosController _controller = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockServicio = new Mock<IServicioMantenimiento>();
        _controller = new MantenimientosController(_mockServicio.Object);
    }

    [TestMethod]
    public void GetAll_RetornaOkConListaDeMantenimientos()
    {
        // Arrange
        var mantenimientos = new List<MantenimientoPreventivo>
        {
            new MantenimientoPreventivo { Id = 1, Descripcion = "Mantenimiento 1" },
            new MantenimientoPreventivo { Id = 2, Descripcion = "Mantenimiento 2" }
        };
        _mockServicio.Setup(s => s.ListarMantenimientos()).Returns(mantenimientos);

        // Act
        var resultado = _controller.GetAll();

        // Assert
        Assert.IsInstanceOfType(resultado, typeof(OkObjectResult));
        var okResult = resultado as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);
        var lista = okResult.Value as IEnumerable<MantenimientoPreventivo>;
        Assert.IsNotNull(lista);
        Assert.AreEqual(2, lista.Count());
        _mockServicio.Verify(s => s.ListarMantenimientos(), Times.Once);
    }

    [TestMethod]
    public void Create_ConDatosValidos_RetornaCreated()
    {
        // Arrange
        var request = new CrearMantenimientoRequest
        {
            AtraccionId = 1,
            FechaProgramada = DateTime.Now.AddDays(1),
            HoraInicio = TimeSpan.FromHours(14),
            DuracionEstimada = TimeSpan.FromHours(2),
            Descripcion = "Mantenimiento de prueba"
        };

        var mantenimientoCreado = new MantenimientoPreventivo
        {
            Id = 1,
            AtraccionId = 1,
            Descripcion = "Mantenimiento de prueba"
        };

        _mockServicio.Setup(s => s.CrearMantenimiento(request)).Returns(mantenimientoCreado);

        // Act
        var resultado = _controller.Create(request);

        // Assert
        Assert.IsInstanceOfType(resultado, typeof(CreatedAtActionResult));
        var createdResult = resultado as CreatedAtActionResult;
        Assert.IsNotNull(createdResult);
        Assert.AreEqual(201, createdResult.StatusCode);
        Assert.AreEqual(nameof(_controller.GetAll), createdResult.ActionName);
        var mantenimiento = createdResult.Value as MantenimientoPreventivo;
        Assert.IsNotNull(mantenimiento);
        Assert.AreEqual(1, mantenimiento.Id);
        _mockServicio.Verify(s => s.CrearMantenimiento(request), Times.Once);
    }

    [TestMethod]
    public void Create_ConDatosInvalidos_RetornaBadRequest()
    {
        // Arrange
        var request = new CrearMantenimientoRequest
        {
            AtraccionId = 1,
            FechaProgramada = DateTime.Now.AddDays(1),
            HoraInicio = TimeSpan.FromHours(14),
            DuracionEstimada = TimeSpan.FromHours(2),
            Descripcion = " "
        };

        _mockServicio.Setup(s => s.CrearMantenimiento(request))
            .Throws(new ArgumentException("La descripción del mantenimiento es requerida"));

        // Act
        var resultado = _controller.Create(request);

        // Assert
        Assert.IsInstanceOfType(resultado, typeof(BadRequestObjectResult));
        var badRequestResult = resultado as BadRequestObjectResult;
        Assert.IsNotNull(badRequestResult);
        Assert.AreEqual(400, badRequestResult.StatusCode);
    }

    [TestMethod]
    public void Delete_MantenimientoExiste_RetornaNoContent()
    {
        // Arrange
        _mockServicio.Setup(s => s.EliminarMantenimiento(1));

        // Act
        var resultado = _controller.Delete(1);

        // Assert
        Assert.IsInstanceOfType(resultado, typeof(NoContentResult));
        var noContentResult = resultado as NoContentResult;
        Assert.IsNotNull(noContentResult);
        Assert.AreEqual(204, noContentResult.StatusCode);
        _mockServicio.Verify(s => s.EliminarMantenimiento(1), Times.Once);
    }

    [TestMethod]
    public void Delete_MantenimientoNoExiste_RetornaNotFound()
    {
        // Arrange
        _mockServicio.Setup(s => s.EliminarMantenimiento(999))
            .Throws(new ArgumentException("Mantenimiento no encontrado"));

        // Act
        var resultado = _controller.Delete(999);

        // Assert
        Assert.IsInstanceOfType(resultado, typeof(NotFoundObjectResult));
        var notFoundResult = resultado as NotFoundObjectResult;
        Assert.IsNotNull(notFoundResult);
        Assert.AreEqual(404, notFoundResult.StatusCode);
    }
}
