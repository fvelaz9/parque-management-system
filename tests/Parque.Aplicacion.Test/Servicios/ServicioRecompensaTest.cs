using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Parque.Aplicacion.DTOs;
using Parque.Aplicacion.DTOs.RecompensasDtos;
using Parque.Aplicacion.Servicios;
using Parque.Aplicacion.Servicios.Gamificacion;
using Parque.Aplicacion.Servicios.Recompensas;
using Parque.Dominio.Gamificacion;
using Parque.Dominio.Usuarios;
using Parque.Infraestructura.Repositorios;

namespace Parque.Aplicacion.Tests.Servicios;

[TestClass]
public class ServicioRecompensaTest
{
    private Mock<IRepositorio<Recompensa>> _mockRepoRecompensa = null!;
    private Mock<IRepositorio<HistorialCanje>> _mockRepoHistorial = null!;
    private Mock<IRepositorio<PuntuacionVisitante>> _mockRepoPuntuacion = null!;
    private Mock<IRepositorio<Visitante>> _mockRepoVisitante = null!;
    private Mock<IServicioFechaHora> _mockServicioFechaHora = null!;
    private ServicioRecompensa _servicio = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockRepoRecompensa = new Mock<IRepositorio<Recompensa>>();
        _mockRepoHistorial = new Mock<IRepositorio<HistorialCanje>>();
        _mockRepoPuntuacion = new Mock<IRepositorio<PuntuacionVisitante>>();
        _mockRepoVisitante = new Mock<IRepositorio<Visitante>>();
        _mockServicioFechaHora = new Mock<IServicioFechaHora>();
        _mockServicioFechaHora.Setup(s => s.ObtenerFechaActual())
            .Returns(new DateTime(2025, 11, 11, 22, 0, 0));
        _servicio = new ServicioRecompensa(_mockRepoRecompensa.Object,_mockRepoHistorial.Object, _mockRepoPuntuacion.Object,_mockRepoVisitante.Object, _mockServicioFechaHora.Object );
    }

    [TestMethod]
    public void CrearRecompensa_ConDatosValidos_DebeRetornarRecompensaConId()
    {
        // Arrange
        var dto = new RecompensaDto
        {
            Nombre = "Entrada VIP",
            Descripcion = "Acceso prioritario",
            CostoEnPuntos = 500,
            CantidadDisponible = 10,
            NivelMembresiaRequerido = NivelMembresia.Premium
        };

        // Act
        var resultado = _servicio.CrearRecompensa(dto);

        // Assert
        Assert.IsNotNull(resultado);
        Assert.AreNotEqual(Guid.Empty, resultado.Id);
        Assert.AreEqual("Entrada VIP", resultado.Nombre);
        Assert.AreEqual(500, resultado.CostoEnPuntos);
        Assert.AreEqual(10, resultado.CantidadDisponible);
        Assert.IsNotNull(resultado.FechaCreacion);
        Assert.AreEqual(new DateTime(2025, 11, 11, 22, 0, 0), resultado.FechaCreacion);
        _mockRepoRecompensa.Verify(r => r.Agregar(It.IsAny<Recompensa>()), Times.Once);
    }

    [TestMethod]
    public void CrearRecompensa_ConNombreVacio_DebeLanzarExcepcion()
    {
        // Arrange
        var dto = new RecompensaDto
        {
            Nombre = " ",
            CostoEnPuntos = 100,
            CantidadDisponible = 5
        };

        // Act & Assert
        Assert.ThrowsException<InvalidOperationException>(() => _servicio.CrearRecompensa(dto));
    }

    [TestMethod]
    public void CrearRecompensa_ConNError_DebeLanzarExcepcion()
    {
        // Arrange
        var dto = new RecompensaDto
        {
            Descripcion = "a",
            CostoEnPuntos = 3,
            CantidadDisponible = -1
        };

        // Act & Assert
        Assert.ThrowsException<InvalidOperationException>(() => _servicio.CrearRecompensa(dto));
    }

    [TestMethod]
    public void CrearRecompensa_ConCostoCero_DebeLanzarExcepcion()
    {
        // Arrange
        var dto = new RecompensaDto
        {
            Nombre = "Test",
            CostoEnPuntos = 0,
            CantidadDisponible = 5
        };

        // Act & Assert
        Assert.ThrowsException<InvalidOperationException>(() => _servicio.CrearRecompensa(dto));
    }

    [TestMethod]
    public void CrearRecompensa_ConCantidadNegativa_DebeLanzarExcepcion()
    {
        // Arrange
        var dto = new RecompensaDto
        {
            Nombre = "Test",
            CostoEnPuntos = 100,
            CantidadDisponible = -1
        };

        // Act & Assert
        Assert.ThrowsException<InvalidOperationException>(() => _servicio.CrearRecompensa(dto));
    }

    [TestMethod]
    public void ActualizarRecompensa_ConDatosValidos_DebeRetornarRecompensaActualizada()
    {
        // Arrange
        var id = Guid.NewGuid();
        var recompensaExistente = new Recompensa
        {
            Id = id,
            Nombre = "Viejo",
            Descripcion = "Descripción vieja",
            CostoEnPuntos = 100,
            CantidadDisponible = 5,
            FechaCreacion = new DateTime(2025, 1, 1)
        };

        _mockRepoRecompensa.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Recompensa, bool>>>()))
            .Returns(recompensaExistente);

        var dtoActualizado = new RecompensaDto
        {
            Nombre = "Nuevo Nombre",
            Descripcion = "Nueva descripción",
            CostoEnPuntos = 200,
            CantidadDisponible = 10,
            NivelMembresiaRequerido = NivelMembresia.Premium
        };

        // Act
        var resultado = _servicio.ActualizarRecompensa(id, dtoActualizado);

        // Assert
        Assert.IsNotNull(resultado);
        Assert.AreEqual(id, resultado.Id);
        Assert.AreEqual("Nuevo Nombre", resultado.Nombre);
        Assert.AreEqual("Nueva descripción", resultado.Descripcion);
        Assert.AreEqual(200, resultado.CostoEnPuntos);
        Assert.AreEqual(10, resultado.CantidadDisponible);
        Assert.AreEqual(NivelMembresia.Premium, resultado.NivelMembresiaRequerido);
        _mockRepoRecompensa.Verify(r => r.Editar(recompensaExistente), Times.Once);
    }

    [TestMethod]
    public void ActualizarRecompensa_ConIdInexistente_DebeLanzarExcepcion()
    {
        // Arrange
        var id = Guid.NewGuid();
        _mockRepoRecompensa.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Recompensa, bool>>>()))
            .Returns((Recompensa)null!);

        var dto = new RecompensaDto
        {
            Nombre = "Test",
            CostoEnPuntos = 100,
            CantidadDisponible = 5
        };

        // Act & Assert
        Assert.ThrowsException<InvalidOperationException>(() =>
            _servicio.ActualizarRecompensa(id, dto));
    }

    [TestMethod]
    public void ActualizarRecompensa_ConNombreVacio_DebeLanzarExcepcion()
    {
        // Arrange
        var id = Guid.NewGuid();
        var recompensaExistente = new Recompensa
        {
            Id = id,
            Nombre = "Existente",
            CostoEnPuntos = 100,
            CantidadDisponible = 5,
            FechaCreacion = DateTime.UtcNow
        };

        _mockRepoRecompensa.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Recompensa, bool>>>()))
            .Returns(recompensaExistente);

        var dto = new RecompensaDto
        {
            Nombre = " ",
            CostoEnPuntos = 100,
            CantidadDisponible = 5
        };

        // Act & Assert
        Assert.ThrowsException<InvalidOperationException>(() =>
            _servicio.ActualizarRecompensa(id, dto));
    }

    [TestMethod]
    public void ObtenerRecompensas_DebeRetornarListaDeRecompensas()
    {
        // Arrange
        var recompensas = new List<Recompensa>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Nombre = "Recompensa 1",
                Descripcion = "Desc 1",
                CostoEnPuntos = 100,
                CantidadDisponible = 5,
                FechaCreacion = DateTime.UtcNow
            },
            new()
            {
                Id = Guid.NewGuid(),
                Nombre = "Recompensa 2",
                Descripcion = "Desc 2",
                CostoEnPuntos = 200,
                CantidadDisponible = 3,
                NivelMembresiaRequerido = NivelMembresia.Premium,
                FechaCreacion = DateTime.UtcNow
            }
        };

        _mockRepoRecompensa.Setup(r => r.ObtenerTodos()).Returns(recompensas);

        // Act
        var resultado = _servicio.ObtenerRecompensas();

        // Assert
        Assert.IsNotNull(resultado);
        Assert.AreEqual(2, resultado.Count);
        Assert.AreEqual("Recompensa 1", resultado[0].Nombre);
        Assert.AreEqual("Recompensa 2", resultado[1].Nombre);
    }

    [TestMethod]
    public void ObtenerRecompensas_SinRecompensas_DebeRetornarListaVacia()
    {
        // Arrange
        _mockRepoRecompensa.Setup(r => r.ObtenerTodos()).Returns(new List<Recompensa>());

        // Act
        var resultado = _servicio.ObtenerRecompensas();

        // Assert
        Assert.IsNotNull(resultado);
        Assert.AreEqual(0, resultado.Count);
    }

    [TestMethod]
    public void ObtenerRecompensaPorId_ConIdValido_DebeRetornarRecompensa()
    {
        // Arrange
        var id = Guid.NewGuid();
        var recompensa = new Recompensa
        {
            Id = id,
            Nombre = "Test Recompensa",
            Descripcion = "Descripción test",
            CostoEnPuntos = 150,
            CantidadDisponible = 10,
            NivelMembresiaRequerido = NivelMembresia.VIP,
            FechaCreacion = DateTime.UtcNow
        };

        _mockRepoRecompensa.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Recompensa, bool>>>()))
            .Returns(recompensa);

        // Act
        var resultado = _servicio.ObtenerRecompensaPorId(id);

        // Assert
        Assert.IsNotNull(resultado);
        Assert.AreEqual(id, resultado.Id);
        Assert.AreEqual("Test Recompensa", resultado.Nombre);
        Assert.AreEqual(150, resultado.CostoEnPuntos);
    }

    [TestMethod]
    public void ObtenerRecompensaPorId_ConIdInexistente_DebeLanzarExcepcion()
    {
        // Arrange
        var id = Guid.NewGuid();
        _mockRepoRecompensa.Setup(r => r.Encontrar(It.IsAny<Expression<Func<Recompensa, bool>>>()))
            .Returns((Recompensa)null!);

        // Act & Assert
        Assert.ThrowsException<InvalidOperationException>(() =>
            _servicio.ObtenerRecompensaPorId(id));
    }
}
