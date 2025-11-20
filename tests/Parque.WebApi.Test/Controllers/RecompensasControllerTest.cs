using Microsoft.AspNetCore.Mvc;
using Moq;
using Parque.Aplicacion.DTOs.RecompensasDtos;
using Parque.Aplicacion.Servicios.Recompensas;
using Parque.Dominio.Gamificacion;
using Parque.Dominio.Usuarios;
using Parque.WebApi.Controllers;

namespace Parque.WebApi.Test.Controllers;

[TestClass]
public class RecompensasController_Test
{
    private Mock<IServicioRecompensa>? _serviceMock;
    private RecompensasController? _controller;

    [TestInitialize]
    public void Initialize()
    {
        _serviceMock = new Mock<IServicioRecompensa>(MockBehavior.Strict);
        _controller = new RecompensasController(_serviceMock.Object);
    }

    [TestCleanup]
    public void Cleanup()
    {
        _serviceMock?.VerifyAll();
    }

    #region CrearRecompensa Tests

    [TestMethod]
    public void CrearRecompensa_DatosValidos_RetornaCreated()
    {
        // Arrange
        var dto = new RecompensaDto
        {
            Nombre = "Descuento 10%",
            Descripcion = "Descuento del 10% en tienda",
            CostoEnPuntos = 100,
            CantidadDisponible = 50,
            NivelMembresiaRequerido = NivelMembresia.Premium
        };

        var recompensaEsperada = new Recompensa
        {
            Id = Guid.NewGuid(),
            Nombre = "Descuento 10%",
            Descripcion = "Descuento del 10% en tienda",
            CostoEnPuntos = 100,
            CantidadDisponible = 50,
            NivelMembresiaRequerido = NivelMembresia.Premium,
            FechaCreacion = DateTime.Now
        };

        _serviceMock!.Setup(s => s.CrearRecompensa(dto))
            .Returns(recompensaEsperada);

        // Act
        var result = _controller!.CrearRecompensa(dto);

        // Assert
        Assert.IsInstanceOfType(result, typeof(CreatedAtActionResult));
        var createdResult = result as CreatedAtActionResult;
        Assert.IsNotNull(createdResult);
        Assert.AreEqual(201, createdResult.StatusCode);
    }

    [TestMethod]
    public void CrearRecompensa_NombreVacio_LanzaExcepcion()
    {
        // Arrange
        var dto = new RecompensaDto
        {
            Nombre = " ",
            CostoEnPuntos = 100,
            CantidadDisponible = 10
        };

        _serviceMock!.Setup(s => s.CrearRecompensa(dto))
            .Throws(new InvalidOperationException("El nombre es obligatorio"));

        // Act & Assert
        Assert.ThrowsException<InvalidOperationException>(() => _controller!.CrearRecompensa(dto));
    }

    #endregion

    #region ActualizarRecompensa Tests

    [TestMethod]
    public void ActualizarRecompensa_DatosValidos_RetornaOk()
    {
        // Arrange
        var id = Guid.NewGuid();
        var dto = new RecompensaDto
        {
            Nombre = "Descuento Actualizado",
            Descripcion = "Nueva descripción",
            CostoEnPuntos = 150,
            CantidadDisponible = 30,
            NivelMembresiaRequerido = NivelMembresia.VIP
        };

        var recompensaActualizada = new Recompensa
        {
            Id = id,
            Nombre = "Descuento Actualizado",
            Descripcion = "Nueva descripción",
            CostoEnPuntos = 150,
            CantidadDisponible = 30,
            NivelMembresiaRequerido = NivelMembresia.VIP
        };

        _serviceMock!.Setup(s => s.ActualizarRecompensa(id, dto))
            .Returns(recompensaActualizada);

        // Act
        var result = _controller!.ActualizarRecompensa(id, dto);

        // Assert
        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);
    }

    #endregion

    #region ObtenerRecompensas Tests

    [TestMethod]
    public void ObtenerRecompensas_RetornaListaDeRecompensas()
    {
        // Arrange
        var recompensasEsperadas = new List<Recompensa>
        {
            new Recompensa
            {
                Id = Guid.NewGuid(),
                Nombre = "Recompensa 1",
                Descripcion = "Descripción 1",
                CostoEnPuntos = 100,
                CantidadDisponible = 50,
                NivelMembresiaRequerido = NivelMembresia.Estandar,
                FechaCreacion = DateTime.Now
            },
            new Recompensa
            {
                Id = Guid.NewGuid(),
                Nombre = "Recompensa 2",
                Descripcion = "Descripción 2",
                CostoEnPuntos = 200,
                CantidadDisponible = 30,
                NivelMembresiaRequerido = NivelMembresia.Premium,
                FechaCreacion = DateTime.Now
            }
        };

        _serviceMock!.Setup(s => s.ObtenerRecompensas())
            .Returns(recompensasEsperadas);

        // Act
        var result = _controller!.ObtenerRecompensas();

        // Assert
        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);
    }

    #endregion

    #region ObtenerRecompensaPorId Tests

    [TestMethod]
    public void ObtenerRecompensaPorId_IdValido_RetornaRecompensa()
    {
        // Arrange
        var id = Guid.NewGuid();
        var recompensaEsperada = new Recompensa
        {
            Id = id,
            Nombre = "Recompensa Test",
            Descripcion = "Descripción test",
            CostoEnPuntos = 150,
            CantidadDisponible = 20,
            NivelMembresiaRequerido = NivelMembresia.VIP,
            FechaCreacion = DateTime.Now
        };

        _serviceMock!.Setup(s => s.ObtenerRecompensaPorId(id))
            .Returns(recompensaEsperada);

        // Act
        var result = _controller!.ObtenerRecompensaPorId(id);

        // Assert
        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);
    }

    #endregion

    #region CanjearRecompensa Tests

    [TestMethod]
    public void CanjearRecompensa_DatosValidos_RetornaOk()
    {
        // Arrange
        var request = new CanjearRecompensaRequest
        {
            VisitanteId = Guid.NewGuid(),
            RecompensaId = Guid.NewGuid()
        };

        var historialEsperado = new HistorialCanje
        {
            Id = Guid.NewGuid(),
            VisitanteId = request.VisitanteId,
            RecompensaId = request.RecompensaId,
            PuntosCanjeados = 100,
            FechaCanje = DateTime.Now
        };

        _serviceMock!.Setup(s => s.CanjearRecompensa(request))
            .Returns(historialEsperado);

        // Act
        var result = _controller!.CanjearRecompensa(request);

        // Assert
        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);
    }

    [TestMethod]
    public void CanjearRecompensa_PuntosInsuficientes_LanzaExcepcion()
    {
        // Arrange
        var request = new CanjearRecompensaRequest
        {
            VisitanteId = Guid.NewGuid(),
            RecompensaId = Guid.NewGuid()
        };

        _serviceMock!.Setup(s => s.CanjearRecompensa(request))
            .Throws(new InvalidOperationException("Puntos insuficientes para canjear esta recompensa"));

        // Act & Assert
        Assert.ThrowsException<InvalidOperationException>(() => _controller!.CanjearRecompensa(request));
    }

    #endregion

    #region ObtenerHistorialCanjes Tests

    [TestMethod]
    public void ObtenerHistorialCanjes_VisitanteConCanjes_RetornaHistorial()
    {
        // Arrange
        var visitanteId = Guid.NewGuid();
        var historialEsperado = new List<HistorialCanjeDto>
        {
            new HistorialCanjeDto
            {
                Id = Guid.NewGuid(),
                VisitanteId = visitanteId,
                RecompensaId = Guid.NewGuid(),
                NombreRecompensa = "Recompensa 1",
                PuntosCanjeados = 100,
                FechaCanje = DateTime.Now.AddDays(-2)
            },
            new HistorialCanjeDto
            {
                Id = Guid.NewGuid(),
                VisitanteId = visitanteId,
                RecompensaId = Guid.NewGuid(),
                NombreRecompensa = "Recompensa 2",
                PuntosCanjeados = 150,
                FechaCanje = DateTime.Now.AddDays(-1)
            }
        };

        _serviceMock!.Setup(s => s.ObtenerHistorialCanjes(visitanteId))
            .Returns(historialEsperado);

        // Act
        var result = _controller!.ObtenerHistorialCanjes(visitanteId);

        // Assert
        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);
    }

    #endregion
    #region EliminarRecompensa Tests

    [TestMethod]
    public void EliminarRecompensa_IdValido_RetornaOk()
    {
        // Arrange
        var id = Guid.NewGuid();
        _serviceMock!.Setup(s => s.EliminarRecompensa(id));

        // Act
        var result = _controller!.EliminarRecompensa(id);

        // Assert
        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);
        Assert.AreEqual("Recompensa eliminada exitosamente", ((dynamic)okResult.Value).mensaje);
    }

    [TestMethod]
    public void EliminarRecompensa_IdInvalido_RetornaNotFound()
    {
        // Arrange
        var id = Guid.NewGuid();
        var mensajeError = "No se encontró la recompensa";
        _serviceMock!.Setup(s => s.EliminarRecompensa(id)).Throws(new InvalidOperationException(mensajeError));

        // Act
        var result = _controller!.EliminarRecompensa(id);

        // Assert
        Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
        var notFoundResult = result as NotFoundObjectResult;
        Assert.IsNotNull(notFoundResult);
        Assert.AreEqual(404, notFoundResult.StatusCode);
        Assert.AreEqual(mensajeError, ((dynamic)notFoundResult.Value).mensaje);
    }

    #endregion

    #region ObtenerPuntos Tests

    [TestMethod]
    public void ObtenerPuntos_VisitanteExiste_RetornaPuntos()
    {
        // Arrange
        var visitanteId = Guid.NewGuid();
        var puntosEsperados = 350;
        _serviceMock!.Setup(s => s.ObtenerPuntosTotalesVisitante(visitanteId)).Returns(puntosEsperados);

        // Act
        var result = _controller!.ObtenerPuntos(visitanteId);

        // Assert
        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);
        Assert.AreEqual(puntosEsperados, ((dynamic)okResult.Value).puntos);
    }

    #endregion
}
