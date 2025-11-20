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
        var mantenimientos = new List<CrearMantenimientoRequest>
        {
            new CrearMantenimientoRequest
            {
                Id = 1,
                AtraccionId = 1,
                NombreAtraccion = "Montaña Rusa",
                FechaProgramada = DateTime.Now.AddDays(1),
                HoraInicio = TimeSpan.FromHours(10),
                DuracionEstimada = TimeSpan.FromHours(2),
                Descripcion = "Mantenimiento 1",
                IncidenciaId = 100
            },
            new CrearMantenimientoRequest
            {
                Id = 2,
                AtraccionId = 2,
                NombreAtraccion = "Rueda de la Fortuna",
                FechaProgramada = DateTime.Now.AddDays(2),
                HoraInicio = TimeSpan.FromHours(14),
                DuracionEstimada = TimeSpan.FromHours(3),
                Descripcion = "Mantenimiento 2",
                IncidenciaId = 101
            }
        };
        _mockServicio.Setup(s => s.ListarMantenimientos()).Returns(mantenimientos);

        var resultado = _controller.GetAll();

        Assert.IsInstanceOfType(resultado, typeof(OkObjectResult));
        var okResult = resultado as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);

        var lista = okResult.Value as IEnumerable<CrearMantenimientoRequest>;
        Assert.IsNotNull(lista);
        Assert.AreEqual(2, lista.Count());

        var primerMantenimiento = lista.First();
        Assert.AreEqual(1, primerMantenimiento.Id);
        Assert.AreEqual("Montaña Rusa", primerMantenimiento.NombreAtraccion);
        Assert.AreEqual("Mantenimiento 1", primerMantenimiento.Descripcion);

        _mockServicio.Verify(s => s.ListarMantenimientos(), Times.Once);
    }

    [TestMethod]
    public void Create_ConDatosValidos_RetornaCreated()
    {
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

        var resultado = _controller.Create(request);

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

        var resultado = _controller.Create(request);

        Assert.IsInstanceOfType(resultado, typeof(BadRequestObjectResult));
        var badRequestResult = resultado as BadRequestObjectResult;
        Assert.IsNotNull(badRequestResult);
        Assert.AreEqual(400, badRequestResult.StatusCode);
    }

    [TestMethod]
    public void Delete_MantenimientoExiste_RetornaNoContent()
    {
        _mockServicio.Setup(s => s.EliminarMantenimiento(1));
        var resultado = _controller.Delete(1);
        Assert.IsInstanceOfType(resultado, typeof(NoContentResult));
        var noContentResult = resultado as NoContentResult;
        Assert.IsNotNull(noContentResult);
        Assert.AreEqual(204, noContentResult.StatusCode);
        _mockServicio.Verify(s => s.EliminarMantenimiento(1), Times.Once);
    }

    [TestMethod]
    public void Delete_MantenimientoNoExiste_RetornaNotFound()
    {
        _mockServicio.Setup(s => s.EliminarMantenimiento(999)).Throws(new ArgumentException("Mantenimiento no encontrado"));
        var resultado = _controller.Delete(999);
        Assert.IsInstanceOfType(resultado, typeof(NotFoundObjectResult));
        var notFoundResult = resultado as NotFoundObjectResult;
        Assert.IsNotNull(notFoundResult);
        Assert.AreEqual(404, notFoundResult.StatusCode);
    }

    [TestMethod]
    public void ActualizarMantenimiento_DatosValidos_RetornaOkConMensajeYDto()
    {
        var id = 1;
        var request = new CrearMantenimientoRequest
        {
            AtraccionId = 1,
            FechaProgramada = DateTime.Today.AddDays(1),
            HoraInicio = TimeSpan.FromHours(10),
            DuracionEstimada = TimeSpan.FromHours(2),
            Descripcion = "Prueba"
        };

        var mantenimiento = new MantenimientoPreventivo
        {
            Id = id,
            AtraccionId = request.AtraccionId,
            FechaProgramada = request.FechaProgramada,
            HoraInicio = request.HoraInicio,
            Descripcion = request.Descripcion,
            DuracionEstimada = request.DuracionEstimada
        };

        _mockServicio.Setup(s => s.ActualizarMantenimiento(id, request)).Returns(mantenimiento);
        var result = _controller.ActualizarMantenimiento(id, request);
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.IsTrue(okResult.Value.ToString().Contains("Mantenimiento actualizado exitosamente"));
    }

    [TestMethod]
    public void ActualizarMantenimiento_MantenimientoNoExiste_RetornaBadRequest()
    {
        var id = 9;
        var request = new CrearMantenimientoRequest
        {
            AtraccionId = 1,
            FechaProgramada = DateTime.Today.AddDays(1),
            HoraInicio = TimeSpan.FromHours(10),
            DuracionEstimada = TimeSpan.FromHours(2),
            Descripcion = "Mantenimiento prueba"
        };
        _mockServicio.Setup(s => s.ActualizarMantenimiento(id, request))
            .Throws(new ArgumentException("No existe"));

        var result = _controller.ActualizarMantenimiento(id, request);

        var badRequest = result as BadRequestObjectResult;
        Assert.IsNotNull(badRequest);

        var rawError = badRequest.Value.ToString();
        Assert.IsTrue(rawError!.Contains("No existe"));
    }
}
